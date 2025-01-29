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
using CprBroker.PartInterface;
using CprBroker.Engine;
using CprBroker.Engine.Local;
using CprBroker.Engine.Part;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CprServices.Responses;

namespace CprBroker.Providers.CprServices
{
    public partial class CprServicesDataProvider : IPartSearchListDataProvider, IExternalDataProvider, IPerCallDataProvider
    {
        public CprServicesDataProvider()
        {
            this.ConfigurationProperties = new Dictionary<string, string>();
        }

        public virtual string[] OperationKeys
        {
            get
            {
                return new string[] { 
                    Constants.OperationKeys.signon,
                    Constants.OperationKeys.newpass,
                    Constants.OperationKeys.ADRSOG1,
                    Constants.OperationKeys.NVNSOG2,
                };
            }
        }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] { 
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.Address, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.UserId, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.Password, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = true, Required=true},                    
                };
            }
        }

        public string Address
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.Address]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.Address] = value; }
        }

        public string UserId
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.UserId]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.UserId] = value; }
        }

        public string Password
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.Password]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.Password] = value; }
        }

        public Version Version
        {
            get { return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor); }
        }

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public bool IsAlive()
        {
            var token = SignonAndGetToken();
            return !string.IsNullOrEmpty(token);
        }

        public LaesResultatType[] SearchList(SoegInputType1 input)
        {
            var cache = new UuidCache();
            return SearchList(input, cache);
        }

        public LaesResultatType[] SearchList(SoegInputType1 input, UuidCache cache)
        {
            var request = new SearchRequest(input.SoegObjekt.SoegAttributListe);
            var availableMethods = new List<SearchMethod>();
            availableMethods.Add(new SearchMethod(Properties.Resources.ADRSOG1));
            availableMethods.Add(new SearchMethod(Properties.Resources.NVNSOG2));

            var plan = new SearchPlan(request, availableMethods.ToArray());

            List<SearchPerson> ret = null;

            if (request.IsUnique)
            {
                if (plan.IsSatisfactory)
                {
                    bool searchOk = true;
                    // TODO: See if tokens could be saved an reused
                    string token = this.SignonAndGetToken();

                    foreach (var call in plan.PlannedCalls)
                    {
                        if (ret != null && ret.Count == 0)// Discontinue search if a previous search returned zero results
                        {
                            searchOk = false;
                            break;
                        }
                        var xml = call.ToRequestXml(Properties.Resources.SearchTemplate);
                        var xmlOut = "";
                        var kvit = Send(call.Name, xml, ref token, out xmlOut);
                        if (kvit.OK)
                        {
                            var persons = new SearchResponse(xmlOut).RowItems.ToList();
                            if (ret == null)
                                ret = persons;
                            else
                                ret = ret.Intersect(persons).ToList();
                        }
                        else
                        {
                            string callInput = string.Join(",", call.InputFields.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)).ToArray());
                            Admin.LogFormattedError("GCTP <{0}> Failed with <{1}><{2}><{3}>. Input <{4}>", call.Name, kvit.ReturnCode, kvit.ReturnText, kvit.ReturnText2, callInput);
                            searchOk = false;
                        }
                    }

                    if (searchOk)
                    {
                        // TODO: Can this break the result? is UUID assignment necessary?
                        var pnrs = ret.Select(p => p.ToPnr()).ToArray();
                        cache.FillCache(pnrs);

                        return ret.Select(p => p.ToLaesResultatType(cache.GetUuid, input)).ToArray();
                    }
                    else
                    {
                        Admin.LogFormattedError("##### Search failed for some reason. Investigate CprBroker.CprServices.CprServicesDataProvider.SearchList() and subsequent calls CprBroker.CprServices.CprServicesDataProvider.https.cs and CprBroker.CprServices.CprServicesDataProvider.Kvit.cs #####");
                        // TODO: What to do if search fails??
                    }
                }
                else
                {
                    string searchFields = string.Join(",", request.CriteriaFields.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)).ToArray());
                    Admin.LogFormattedError("Insufficient GCTP search criteria <{0}>", searchFields);
                }
            }
            else
            {
                string searchFields = string.Join(",", request.CriteriaFields.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)).ToArray());
                Admin.LogFormattedError("Contradicting GCTP search criteria <{0}>", searchFields);
            }
            return null;
        }

    }
}
