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
 * Thomas Kristensen
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

using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Engine.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using CprBroker.Utilities.Config;
using CprBroker.Config;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider : IPutSubscriptionDataProvider, ISubscriptionManagerDataProvider
    {

        public enum ReturnCodePNR { ADDED, REMOVED, ALREADY_EXISTED, NON_EXISTING_PNR, Other };

        public bool PutSubscription(PersonIdentifier personIdentifier)
        {

            // This is a Blacklist feature, where we check if the person is "blacklisted" and should not be subscriped to, when called through CPR Broker
            using (var conn = new SqlConnection(ConfigManager.Current.Settings.CprBrokerConnectionString))
            {
                // Count the number of Cpr numbers that match the cpr number being processed 
                // - we just need to know if it is in the db, but this is the easiest way
                var cmd = new SqlCommand("SELECT COUNT(1) FROM [Blacklist] WHERE CprNr = @cpr", conn);

                // Should be safer and more effecient than inserting it in the string above
                cmd.Parameters.AddWithValue("@cpr", personIdentifier.CprNumber);
                conn.Open();

                bool exists = 0 < (int)cmd.ExecuteScalar();
                
                if (exists) 
                {
                    Admin.LogSuccess("Subscription of person<"+personIdentifier.UUID+"> blocked, because person was Black-Listed ");
                    return false;
                }
            }

            var service = CreateService<CprSubscriptionService.CprSubscriptionWebServicePortType, CprSubscriptionService.CprSubscriptionWebServicePortTypeClient>(ServiceInfo.CPRSubscription);

            using (var callContext = this.BeginCall("AddPNRSubscription", personIdentifier.CprNumber))
            {
                try
                {
                    var request = new CprSubscriptionService.AddPNRSubscriptionType()
                    {
                        InvocationContext = GetInvocationContext<CprSubscriptionService.InvocationContextType>(ServiceInfo.CPRSubscription.UUID),
                        PNR = personIdentifier.CprNumber
                    };

                    var resultWrp = service.AddPNRSubscription(request);
                    if (resultWrp != null)
                    {
                        ReturnCodePNR returnCode = (ReturnCodePNR)Enum.Parse(typeof(ReturnCodePNR), resultWrp.Result); //will throw an overflow exception in case of unknown value.
                        switch (returnCode)
                        {
                            case ReturnCodePNR.ADDED:
                                //success
                                break;
                            case ReturnCodePNR.ALREADY_EXISTED:
                                //success
                                break;
                            case ReturnCodePNR.NON_EXISTING_PNR:
                                throw new Exception(String.Format("Error placing subscription for UUID <{0}>, service platform returns NON_EXISTING_PNR.", personIdentifier.UUID));
                            default:
                                throw new Exception(String.Format("Error placing subscription for UUID <{0}>, service platform returns unexpected code <{1}>.", personIdentifier.UUID, returnCode));
                        }
                        callContext.Succeed();
                        return true;
                    }
                    else
                    {
                        throw new Exception(String.Format("Null value returned by service api call AddPNRSubscription, when trying to place subscription for UUID: <{0}>", personIdentifier.UUID));
                    }
                }
                catch (Exception ex)
                {
                    Admin.LogException(ex);
                    callContext.Fail();
                    return false;
                }
            }
        }

        public bool RemoveSubscription(PersonIdentifier personIdentifier)
        {
            var service = CreateService<CprSubscriptionService.CprSubscriptionWebServicePortType, CprSubscriptionService.CprSubscriptionWebServicePortTypeClient>(ServiceInfo.CPRSubscription);

            using (var callContext = this.BeginCall("RemovePNRSubscription", personIdentifier.CprNumber))
            {
                try
                {
                    var request = new CprSubscriptionService.RemovePNRSubscriptionType
                    {
                        InvocationContext = GetInvocationContext<CprSubscriptionService.InvocationContextType>(ServiceInfo.CPRSubscription.UUID),
                        PNR = personIdentifier.CprNumber
                    };

                    var resultWrp = service.RemovePNRSubscription(request);
                    if (resultWrp != null)
                    {
                        ReturnCodePNR returnCode = (ReturnCodePNR)Enum.Parse(typeof(ReturnCodePNR), resultWrp.Result); //will throw an overflow exception in case of unknown value.
                        switch (returnCode)
                        {
                            case ReturnCodePNR.REMOVED:
                                //success
                                break;
                            case ReturnCodePNR.NON_EXISTING_PNR:
                                throw new Exception(String.Format("Error removing subscription for UUID <{0}>, service platform returns NON_EXISTING_PNR.", personIdentifier.UUID));
                            default:
                                throw new Exception(String.Format("Error removing subscription for UUID <{0}>, service platform returns unexpected code <{1}>.", personIdentifier.UUID, returnCode));
                        }
                        callContext.Succeed();
                        return true;
                    }
                    else
                    {
                        throw new Exception(String.Format("Null value returned by service api call RemovePNRSubscription, when trying to place subscription for UUID: <{0}>", personIdentifier.UUID));
                    }
                }
                catch (Exception ex)
                {
                    Admin.LogException(ex);
                    callContext.Fail();
                    return false;
                }
            }
        }

        public string[] SubscriptionFields
        {
            get
            {
                return new string[] {
                    Constants.SubscriptionFields.MunicipalityCode,
                    Constants.SubscriptionFields.ChangeCode,
                    Constants.SubscriptionFields.PNR,
                    //Constants.SubscriptionFields.AgeRange
                };
            }
        }

        public string[] GetSubscriptions(string field)
        {
            if (SubscriptionFields.Contains(field))
            {
                var service = CreateService<CprSubscriptionService.CprSubscriptionWebServicePortType, CprSubscriptionService.CprSubscriptionWebServicePortTypeClient>(ServiceInfo.CPRSubscription);
                var invocationContext = this.GetInvocationContext<CprSubscriptionService.InvocationContextType>(ServiceInfo.CPRSubscription.UUID);

                using (var callContext = this.BeginCall("GetAllFilters", ""))
                {
                    var filters = service.GetAllFilters(new CprSubscriptionService.GetAllFiltersType() { InvocationContext = invocationContext });
                    callContext.Succeed();

                    Func<string[], string[]> isNull = s => (s != null) ? s : new string[] { };

                    switch (field)
                    {
                        case Constants.SubscriptionFields.PNR:
                            return isNull(filters.PNR);

                        case Constants.SubscriptionFields.MunicipalityCode:
                            return isNull(filters.MunicipalityCode).Select(c => c.PadLeft(4, '0')).ToArray();

                        case Constants.SubscriptionFields.ChangeCode:
                            return isNull(filters.ChangeCode);

                            //case Constants.SubscriptionFields.AgeRange:
                            //    return filters.AgeRange;
                    }
                }
            }
            return null;
        }

        public bool PutSubscription(string field, string value)
        {
            ReturnCodePNR retCode;
            return PutSubscription(field, value, out retCode);
        }

        public bool PutSubscription(string field, string value, out ReturnCodePNR retCode)
        {
            if (SubscriptionFields.Contains(field))
            {
                var service = CreateService<CprSubscriptionService.CprSubscriptionWebServicePortType, CprSubscriptionService.CprSubscriptionWebServicePortTypeClient>(ServiceInfo.CPRSubscription);
                var invocationContext = this.GetInvocationContext<CprSubscriptionService.InvocationContextType>(ServiceInfo.CPRSubscription.UUID);
                string ret = null;

                using (var callContext = this.BeginCall("PutSubscription", value))
                {
                    switch (field)
                    {
                        case Constants.SubscriptionFields.PNR:
                            ret = service.AddPNRSubscription(
                                new CprSubscriptionService.AddPNRSubscriptionType()
                                {
                                    InvocationContext = invocationContext,
                                    PNR = value
                                }).Result;
                            break;

                        case Constants.SubscriptionFields.MunicipalityCode:
                            ret = service.AddMunicipalityCodeSubscription(
                                new CprSubscriptionService.AddMunicipalityCodeSubscriptionType()
                                {
                                    InvocationContext = invocationContext,
                                    MunicipalityCode = value
                                }).Result;
                            break;

                        case Constants.SubscriptionFields.ChangeCode:
                            ret = service.AddChangeCodeSubscription(
                                new CprSubscriptionService.AddChangeCodeSubscriptionType()
                                {
                                    InvocationContext = invocationContext,
                                    ChangeCode = value
                                }).Result;
                            break;

                            //case Constants.SubscriptionFields.AgeRange:
                            //    return filters.AgeRange;
                    }
                    if (!string.IsNullOrEmpty(ret))
                    {
                        retCode = Utilities.Reflection.ParseEnum<ReturnCodePNR>(ret);
                        var result = retCode == ReturnCodePNR.ADDED || retCode == ReturnCodePNR.ALREADY_EXISTED;
                        if (result)
                            callContext.Succeed();
                        else
                            callContext.Fail();

                        return result;
                    }
                }
            }
            retCode = ReturnCodePNR.Other;
            return false;
        }

        public bool RemoveSubscription(string field, string value)
        {
            if (SubscriptionFields.Contains(field))
            {
                var service = CreateService<CprSubscriptionService.CprSubscriptionWebServicePortType, CprSubscriptionService.CprSubscriptionWebServicePortTypeClient>(ServiceInfo.CPRSubscription);
                var invocationContext = this.GetInvocationContext<CprSubscriptionService.InvocationContextType>(ServiceInfo.CPRSubscription.UUID);
                string ret = null;

                using (var callContext = this.BeginCall("RemoveSubscription", ""))
                {
                    switch (field)
                    {
                        case Constants.SubscriptionFields.PNR:
                            ret = service.RemovePNRSubscription(
                                new CprSubscriptionService.RemovePNRSubscriptionType()
                                {
                                    InvocationContext = invocationContext,
                                    PNR = value
                                }).Result;
                            break;

                        case Constants.SubscriptionFields.MunicipalityCode:
                            ret = service.RemoveMunicipalityCodeSubscription(
                                new CprSubscriptionService.RemoveMunicipalityCodeSubscriptionType()
                                {
                                    InvocationContext = invocationContext,
                                    MunicipalityCode = value
                                }).Result;
                            break;

                        case Constants.SubscriptionFields.ChangeCode:
                            ret = service.RemoveChangeCodeSubscription(
                                new CprSubscriptionService.RemoveChangeCodeSubscriptionType()
                                {
                                    InvocationContext = invocationContext,
                                    ChangeCode = value
                                }).Result;
                            break;

                            //case Constants.SubscriptionFields.AgeRange:
                            //    return filters.AgeRange;
                    }
                    if (!string.IsNullOrEmpty(ret))
                    {
                        var retCode = Utilities.Reflection.ParseEnum<ReturnCodePNR>(ret);
                        var result = retCode == ReturnCodePNR.REMOVED;
                        if (result)
                            callContext.Succeed();
                        else
                            callContext.Fail();
                        return result;
                    }
                }
            }
            return false;
        }

        public Dictionary<string, string> EnumerateField(string field)
        {
            switch (field)
            {
                case Constants.SubscriptionFields.MunicipalityCode:
                    return CprBroker.Providers.CPRDirect.Authority.GetAuthorities(
                        CprBroker.Providers.CPRDirect.Constants.AuthorityTypes.Municipality)
                        .ToDictionary(auth => auth.AuthorityCode, auth => auth.AuthorityName);

                case Constants.SubscriptionFields.ChangeCode:
                    return Properties.Resources.CprEvents
                        .Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Split(' '))
                        .ToDictionary(arr => arr.First(), arr => string.Join(" ", arr.Skip(1).ToArray()));

                default:
                    return null;
            }
        }

        public bool IsSharingSubscriptions
        {
            get
            {
                return DataProviderConfigPropertyInfo.GetBoolean(ConfigurationProperties, Constants.ConfigProperties.IsSharingSubscriptions);
            }
            set
            {
                this.ConfigurationProperties[Constants.ConfigProperties.IsSharingSubscriptions] = value.ToString();
            }
        }

    }
}