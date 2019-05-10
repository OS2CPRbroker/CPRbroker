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
using CprBroker.Engine;
using CprBroker.Data.DataProviders;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Utilities.Config;
using CprBroker.Utilities;
using CprBroker.Schemas;
using CprBroker.Schemas.Wrappers;

namespace CprBroker.DBR
{
    public partial class NewRequestType
    {
        protected virtual void ValidateOperationMode()
        {
            if (this.LargeData != Providers.DPR.DetailType.ExtendedData || this.Type != Providers.DPR.InquiryType.DataUpdatedAutomaticallyFromCpr)
            {
                // TODO: unimplemented mode of operation
                throw new NotImplementedException();
            }
        }

        public virtual IndividualResponseType GetPerson(out ICprDirectPersonDataProvider usedProvider)
        {
            var cprDirectProviders = LoadDataProviders<CPRDirectClientDataProvider>();
            return GetPerson(cprDirectProviders, out usedProvider);
        }

        public virtual IndividualResponseType GetPerson(IEnumerable<ICprDirectPersonDataProvider> dataProviders, out ICprDirectPersonDataProvider usedProvider)
        {
            foreach (var prov in dataProviders)
            {
                var resp = prov.GetPerson(this.PNR);
                if (resp != null)
                {
                    usedProvider = prov;
                    return resp;
                }
            }
            usedProvider = null;
            return null;
        }

        public bool CanPutSubscription(ICprDirectPersonDataProvider usedProvider)
        {
            return
                (usedProvider is IPutSubscriptionDataProvider && !(usedProvider is IPutSubscriptionDataProvider2)) // Always puts a subscription with no way to disable it
                || (usedProvider is IPutSubscriptionDataProvider2 && !(usedProvider as IPutSubscriptionDataProvider2).DisableSubscriptions); // Can disable subscriptions, but they are NOT disabled
        }

        public virtual bool PutSubscription()
        {
            var subscriptionDataProviders = LoadDataProviders<IPutSubscriptionDataProvider>();
            var pId = new PersonIdentifier() { CprNumber = this.PNR, UUID = null };
            var putSubscriptionRet = subscriptionDataProviders
                .FirstOrDefault(sdp => sdp.PutSubscription(pId));
            return putSubscriptionRet != null;
        }

        public virtual void SaveAsExtract(IndividualResponseType response)
        {
            response.SaveAsExtract(enqueueConversion: true, enqueueExtractTotals: false);
        }

        public virtual IList<object> GetDatabaseInserts(string dprConnectionString, IndividualResponseType response, bool skipAddressIfDead = false)
        {
            if (response == null)
            {
                response = ExtractManager.GetPerson(this.PNR);
            }

            using (var dataContext = new DPRDataContext(dprConnectionString))
            {
                CprConverter.AppendPerson(response, dataContext, CprBroker.Providers.DPR.DataRetrievalTypes.CprDirectWithSubscription,null, skipAddressIfDead);
                return dataContext.GetChangeSet().Inserts;
            }
        }

        public virtual void UpdateDprDatabase(string dprConnectionString, IList<object> objectsToInsert)
        {
            using (var conn = new SqlConnection(dprConnectionString))
            {
                using (var dataContext = new DPRDataContext(conn))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        dataContext.Transaction = trans;
                        CprConverter.DeletePersonRecords(this.PNR, dataContext);
                        conn.BulkInsertChanges(objectsToInsert, trans);
                        trans.Commit();
                    }
                }
            }
        }

        public virtual IEnumerable<T> LoadDataProviders<T>()
            where T : class, IDataProvider
        {
            DataProvidersConfigurationSection section = ConfigManager.Current.DataProvidersSection;
            var providerFactory = new DataProviderFactory();
            DataProvider[] dbProviders = providerFactory.ReadDatabaseDataProviders();

            var providers = providerFactory
                .GetDataProviderList(section, dbProviders, typeof(T), Schemas.SourceUsageOrder.LocalThenExternal)
                .Select(p => p as T);
            return providers;
        }
    }
}
