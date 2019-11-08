/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Dennis Amdi Skov Isaksen
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using CprBroker.Engine.Queues;
using CprBroker.Utilities;
using CprBroker.Engine;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;

namespace CprBroker.DBR
{
    public class DbrQueue : CprBroker.Engine.Queues.Queue<ExtractQueueItem>
    {
        public DbrQueue()
        { }

        public string ConnectionString
        {
            get { return DataProviderConfigProperty.Templates.GetConnectionString(this.ConfigurationProperties); }
            set { DataProviderConfigProperty.Templates.SetConnectionString(value, this.ConfigurationProperties); }
        }

        public string Address
        {
            get
            {
                return this.ConfigurationProperties["Address"];
            }
            set
            {
                this.ConfigurationProperties["Address"] = value;
            }
        }

        public int? Port
        {
            get
            {
                var s = this.ConfigurationProperties["Port"];
                int ret;
                if (int.TryParse(s, out ret))
                    return ret;
                else
                    return null;
            }
            set
            {
                this.ConfigurationProperties["Port"] = value.HasValue ? "" : value.Value.ToString();
            }
        }
        public bool IgnoreAddressOfDeadPeople
        {
            get
            {
                return this.GetBoolean("Ignore address of dead people", false);
            }
            set
            {
                this.ConfigurationProperties["Ignore address of dead people"] = value.ToString();
            }
        }

        public int? MaxWaitMilliseconds
        {
            get
            {
                var s = this.ConfigurationProperties["MaxWaitMilliseconds"];
                int ret;
                if (int.TryParse(s, out ret))
                    return ret;
                else
                    return null;
            }
            set
            {
                this.ConfigurationProperties["MaxWaitMilliseconds"] = value.HasValue ? "" : value.Value.ToString();
            }
        }

        public bool DiversionEnabled
        {
            get { return Port.HasValue; }
        }

        class Batch
        {
            public List<ExtractQueueItem> Items = new List<ExtractQueueItem>();
            public List<Object> Inserts = new List<object>();
        }

        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            var totalsItems = items.Where(i => string.IsNullOrEmpty(i.PNR)).ToArray();
            var personItems = items.Except(totalsItems).ToArray();

            return ProcessPersons(personItems)
                .Union(ProcessTotals(totalsItems))
                .ToArray();
        }

        public ExtractQueueItem[] ProcessPersons(ExtractQueueItem[] items)
        {
            var ret = new List<ExtractQueueItem>();

            using (var cprDataContext = new ExtractDataContext())
            {
                // TODO: What if extract is from CPR Direkte ?
                items.LoadExtractAndItems(cprDataContext);

                var itemGroups = new List<Batch>();
                itemGroups.Add(new Batch());

                // Group in batches
                foreach (var item in items)
                {
                    try
                    {
                        if (item.Extract != null)
                        {
                            using (var dprDataContext = new DPRDataContext(this.ConnectionString))
                            {
                                var response = Extract.ToIndividualResponseType(item.Extract, item.ExtractItems, CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                                CprConverter.AppendPerson(response,
                                    dprDataContext,
                                    dataRetrievalType: DataRetrievalTypes.Extract,
                                    updatingProgram: UpdatingProgram.DprUpdate,
                                    skipAddressIfDead: IgnoreAddressOfDeadPeople);

                                var currentGroup = itemGroups.Last();
                                if (currentGroup.Items.FirstOrDefault(gi => gi.PNR == item.PNR) != null)
                                {
                                    currentGroup = new Batch();
                                    itemGroups.Add(currentGroup);
                                }
                                currentGroup.Items.Add(item);
                                currentGroup.Inserts.AddRange(dprDataContext.GetChangeSet().Inserts);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CprBroker.Engine.Local.Admin.LogException(ex, string.Format("<{0}>", item.PNR));
                    }
                }
                itemGroups = itemGroups.Where(g => g.Items.Count > 0).ToList();

                // Now run the batches
                foreach (var itemGroup in itemGroups)
                {
                    try
                    {
                        using (var conn = new SqlConnection(this.ConnectionString))
                        {
                            using (var dprDataContext = new DPRDataContext(conn))
                            {
                                conn.Open();
                                using (var trans = conn.BeginTransaction())
                                {
                                    dprDataContext.Transaction = trans;

                                    CprConverter.DeletePersonRecords(itemGroup.Items.Select(i => i.PNR).ToArray(), dprDataContext);
                                    conn.BulkInsertChanges(itemGroup.Inserts, trans);

                                    trans.Commit();
                                    ret.AddRange(itemGroup.Items);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CprBroker.Engine.Local.Admin.LogException(ex);
                    }
                }
            }
            return ret.ToArray();
        }

        public ExtractQueueItem[] ProcessTotals(ExtractQueueItem[] items)
        {
            var ret = new List<ExtractQueueItem>();

            foreach (var item in items)
            {
                try
                {
                    using (var extractDataContext = new ExtractDataContext())
                    {
                        var extract = extractDataContext.Extracts.SingleOrDefault(e => e.ExtractId == item.ExtractId);

                        if (extract == null)
                        {
                            CprBroker.Engine.Local.Admin.LogFormattedError("Extract with ID <{0}> was not found when updating DTAJOUR at <{1}>", item.ExtractId, this.QueueId);
                        }
                        else
                        {
                            using (var dprDataContext = new AdminDataContext(this.ConnectionString))
                            {
                                // Get the update record - if it does not exist, raise an exception
                                var update = dprDataContext.Updates.SingleOrDefault();
                                if (update == null)
                                {
                                    CprBroker.Engine.Local.Admin.LogFormattedSuccess("DTAJOUR row was not found at <{0}>. Filling with default values", this.QueueId);
                                    update = this.ToUpdateRecord();
                                    dprDataContext.Updates.InsertOnSubmit(update);
                                }

                                update.ANTAL = extract.ExtractItems.Select(ei => ei.PNR).Distinct().Count();
                                update.DPRAJDTO = CprBroker.Utilities.Dates.DateToDecimal(extract.ExtractDate, 8);
                                update.LEVAJDTO = CprBroker.Utilities.Dates.DateToDecimal(extract.ExtractDate, 8);
                                dprDataContext.SubmitChanges();
                                ret.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CprBroker.Engine.Local.Admin.LogException(ex);
                }
            }

            return ret.ToArray();
        }

        public Update ToUpdateRecord()
        {
            decimal komKod;

            using (var dprDataContext = new DPRDataContext(this.ConnectionString))
            {
                komKod = dprDataContext.PersonTotals
                    .Where(pt => pt.MunicipalityCode > 0)
                    .Select(pt => pt.MunicipalityCode)
                    .GroupBy(kk => kk)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?
                    .Key ?? 0;
            }

            return new Update()
            {
                DB_VERSION = "2013-02",
                HISMDR = 60,
                KOMKOD = komKod,
                KOMNVN = Authority.GetAuthorityNameByCode(komKod.ToString()) ?? "",
                // Non used columns
                CICSID = null,
                KUNDENR = null,
                LU62MRK = null,
                LUNAVN = null,
                SOEGMRK = null,
            };
        }

        public override Engine.DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                var ret = new List<Engine.DataProviderConfigPropertyInfo>(DataProviderConfigPropertyInfo.Templates.ConnectionStringKeys);
                ret.Add(new DataProviderConfigPropertyInfo() { Confidential = false, Name = "Address", Type = DataProviderConfigPropertyInfoTypes.String, Required = false });
                ret.Add(new DataProviderConfigPropertyInfo() { Confidential = false, Name = "Port", Type = DataProviderConfigPropertyInfoTypes.Integer, Required = false });
                ret.Add(new DataProviderConfigPropertyInfo() { Confidential = false, Name = "MaxWaitMilliseconds", Type = DataProviderConfigPropertyInfoTypes.Integer, Required = false });
                ret.Add(new DataProviderConfigPropertyInfo() { Confidential = false, Name = "Ignore address of dead people", Type = DataProviderConfigPropertyInfoTypes.Boolean, Required = false });

                return ret.ToArray();
            }
        }

        public bool IsAlive()
        {
            try
            {
                using (var conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
                return false;
            }
        }

        public DprDiversionServer CreateListener()
        {
            if (DiversionEnabled)
            {
                var listener = new DprDiversionServer()
                {
                    Address = this.Address,
                    Port = this.Port.Value,
                    DbrQueue = this,
                    //InputMessageSize = 40, // Populated in the constructor
                    MaxWait = TimeSpan.FromMilliseconds(MaxWaitMilliseconds.HasValue ? MaxWaitMilliseconds.Value : 1000)
                };
                return listener;
            }
            return null;
        }

    }
}
