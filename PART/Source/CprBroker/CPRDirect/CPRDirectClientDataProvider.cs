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
 * Dennis Isaksen
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
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectClientDataProvider : IPartReadDataProvider, IPutSubscriptionDataProvider2, IExternalDataProvider, IPerCallDataProvider, ICprDirectPersonDataProvider
    {
        #region IPartReadDataProvider members

        public RegistreringType1 Read(CprBroker.Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            ql = QualityLevel.Cpr;
            var response = GetPerson(uuid.CprNumber);
            if (response != null)
            {
                UuidCache cache = new UuidCache();
                cache.FillCache(response.RelatedPnrs);

                var ret = response.ToRegistreringType1(cpr2uuidFunc);
                // Do not save the result if subscriptions are disabled
                ret.IsUpdatableLocally = !DisableSubscriptions;
                return ret;
            }
            else
            {
                return null;
            }
        }

        public bool PutSubscription(PersonIdentifier personIdentifier)
        {
            if (DisableSubscriptions)
                return false;

            if (IPartPerCallDataProviderHelper.CanCallOnline(personIdentifier.CprNumber))
            {
                IndividualRequestType request = new IndividualRequestType(
                    putSubscription: true,
                    dataType: DataType.NoData,
                    pnr: decimal.Parse(personIdentifier.CprNumber));
                IndividualResponseType response = this.GetResponse(request);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveSubscription(PersonIdentifier personIdentifier)
        {
            //if (DisableSubscriptions)
            //    return false;

            if (IPartPerCallDataProviderHelper.CanCallOnline(personIdentifier.CprNumber))
            {
                IndividualRequestType request = new IndividualRequestType(
                    subscriptionType: SubscriptionType.DeleteSubscription,
                    dataType: DataType.NoData,
                    pnr: decimal.Parse(personIdentifier.CprNumber));

                // This commented code does not work for subscription removal, as there is no data in the request.
                //IndividualResponseType response = this.GetResponse(request);


                string response;
                string error;
                string operation = string.Format("{0}-{1}{2}", Constants.OnlineOperationName, request.DataType, request.SubscriptionType);
                if(this.Send(operation, request.PNR, request.Contents, out response, out error))
                {
                    return true;
                } else
                {
                    throw new Exception(error);
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsSharingSubscriptions
        {
            get
            {
                return DataProviderConfigPropertyInfo.GetBoolean(ConfigurationProperties, Constants.PropertyNames.IsSharingSubscriptions);
            }
            set
            {
                this.ConfigurationProperties[Constants.PropertyNames.IsSharingSubscriptions] = value.ToString();
            }
        }

        #endregion

        #region IDataProvider members
        public bool IsAlive()
        {
            System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
            try
            {
                client.Connect(this.Address, this.Port);
                client.GetStream().Close();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                client.Close();
            }
        }

        public Version Version
        {
            get { return new Version(Utilities.Constants.Versioning.Major, Utilities.Constants.Versioning.Minor); }
        }
        #endregion


        #region IExternalDataProvider Members
        public virtual DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] {
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.Address, Type= DataProviderConfigPropertyInfoTypes.String, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.Port, Type= DataProviderConfigPropertyInfoTypes.Integer, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.DisableSubscriptions, Type= DataProviderConfigPropertyInfoTypes.Boolean, Required=true, Confidential=false},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.IsSharingSubscriptions, Type= DataProviderConfigPropertyInfoTypes.Boolean, Required=true, Confidential=false}
                };
            }
        }

        public string[] OperationKeys
        {
            get
            {
                return new string[] {
                    Constants.OnlineOperationName
                };
            }
        }

        public Dictionary<string, string> ConfigurationProperties
        {
            get;
            set;
        }
        #endregion

        #region Specific members
        public string Address
        {
            get { return ConfigurationProperties[Constants.PropertyNames.Address]; }
            set { ConfigurationProperties[Constants.PropertyNames.Address] = value; }
        }

        public int Port
        {
            get { return Convert.ToInt32(ConfigurationProperties[Constants.PropertyNames.Port]); }
            set { ConfigurationProperties[Constants.PropertyNames.Port] = value.ToString(); }
        }

        public bool DisableSubscriptions
        {
            get { return this.GetBoolean(Constants.PropertyNames.DisableSubscriptions, false); }
            set { ConfigurationProperties[Constants.PropertyNames.DisableSubscriptions] = value.ToString(); }
        }

        #endregion

        #region ICprDirectPersonDataProvider members

        public virtual IndividualResponseType GetPerson(string cprNumber)
        {
            if (IPartPerCallDataProviderHelper.CanCallOnline(cprNumber))
            {
                IndividualRequestType request = new IndividualRequestType(
                    putSubscription: !this.DisableSubscriptions,
                    dataType: DataType.DefinedByTask,
                    pnr: decimal.Parse(cprNumber));
                IndividualResponseType response = this.GetResponse(request);

                return response;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
