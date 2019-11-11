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
using CprBroker.Engine;
using System.Net.Sockets;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Base class for all DPR data providers
    /// </summary>
    public partial class DprDatabaseDataProvider :
        IExternalDataProvider, IDataProvider, IPerCallDataProvider,
        IPutSubscriptionDataProvider, IPartReadDataProvider,
        IChangePuller<Queues.T_DPRUpdateStaging>, IAutoUpdateDataProvider
    {
        /// <summary>
        /// Map for error codes that are returned fromDate DPR. Each provider fills its own list
        /// </summary>
        protected static Dictionary<string, string> ErrorCodes = new Dictionary<string, string>();

        #region IExternalDataProvider Members

        private Dictionary<string, string> _ConfigurationProperties = new Dictionary<string, string>();
        public Dictionary<string, string> ConfigurationProperties
        {
            get { return _ConfigurationProperties; }
            set { _ConfigurationProperties = value; }
        }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return
                    DataProviderConfigPropertyInfo.Templates.ConnectionStringKeys
                    .Union(new DataProviderConfigPropertyInfo[] {
                        new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Boolean, Name="Disable Diversion", Required=false,Confidential=false},
                        new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Boolean, Name="Is Sharing Subscriptions", Required=true, Confidential=false},
                        new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.String, Name="Address", Required=false, Confidential=false},
                        new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Integer, Name="Port", Required=false, Confidential=false},
                        new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Integer, Name="TCP Read Timeout (ms)" , Required=true, Confidential=false},
                        new DataProviderConfigPropertyInfo(){Type = DataProviderConfigPropertyInfoTypes.Boolean, Name="Auto Update" , Required=true, Confidential=false}
                    })
                    .ToArray();
            }
        }

        public string[] OperationKeys
        {
            get
            {
                return new string[] {
                    Constants.DiversionOperationName
                };
            }
        }

        #endregion

        public string Address
        {
            get { return this.GetString("Address"); }
            set { this.ConfigurationProperties["Address"] = value; }
        }

        public int Port
        {
            get { return this.GetInteger("Port"); }
            set { this.ConfigurationProperties["Port"] = Convert.ToString(value); }
        }

        public string ConnectionString
        {
            get { return DataProviderConfigPropertyInfo.Templates.GetConnectionString(this.ConfigurationProperties); }
            set { DataProviderConfigPropertyInfo.Templates.SetConnectionString(value, this.ConfigurationProperties); }
        }

        public int TcpReadTimeout
        {
            get { return this.GetInteger("TCP Read Timeout (ms)"); }
            set { this.ConfigurationProperties["TCP Read Timeout (ms)"] = Convert.ToString(value); }
        }

        public bool DisableDiversion
        {
            get { return this.GetBoolean("Disable Diversion"); }
            set { ConfigurationProperties["Disable Diversion"] = Convert.ToString(value); }
        }

        public bool IsSharingSubscriptions
        {
            get { return this.GetBoolean("Is Sharing Subscriptions"); }
            set { this.ConfigurationProperties["Is Sharing Subscriptions"] = Convert.ToString(value); }
        }

        public bool AutoUpdate
        {
            get { return this.GetBoolean("Auto Update"); }
            set { ConfigurationProperties["Auto Update"] = Convert.ToString(value); }
        }

        public string AutoUpdateHint
        {
            get
            {
                var ret = new List<string>();
                ret.Add(Properties.Resources.CreateTrackingTables);
                ret.Add(Properties.Resources.CreateTrackingTriggers);

                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(this.ConnectionString);
                ret.Add(Properties.Resources.InitTrackingPermissions.Replace("<userId>", builder.UserID));

                return string.Join("\r\nGO\r\n", ret.ToArray());
            }
        }

        public bool InitAutoUpdate()
        {
            try
            {
                CprBroker.Installers.DatabaseCustomAction.ExecuteDDL(AutoUpdateHint, this.ConnectionString, false);
                return true;
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
                return false;
            }
        }


        #region IDataProvider Members

        public Version Version
        {
            get
            {
                return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor);
            }
        }

        #endregion

        #region Protected Members

        protected string Send(string message, string operation, string cprNumber)
        {
            string response;
            string error;
            if (Send(message, operation, cprNumber, out response, out error))
            {
                return response;
            }
            else
            {
                throw new Exception(error);
            }
        }

        public static readonly object DiversionLockObject = new object();

        /// <summary>
        /// Sends a message through TCP to the server
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="response">Response to message</param>
        /// <param name="error">Error text (if any)</param>
        /// <returns>True if no error, false otherwise</returns>
        protected bool Send(string message, string operation, string cprNumber, out string response, out string error)
        {
            int bytes = 0;
            error = null;

            using (var callContext = this.BeginCall(operation, cprNumber))
            {
                try
                {
                    Byte[] data = Constants.DiversionEncoding.GetBytes(message);
                    lock (DiversionLockObject)
                    {
                        using (TcpClient client = new TcpClient(Address, Port))
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                stream.Write(data, 0, data.Length);
                                stream.ReadTimeout = this.TcpReadTimeout;
                                data = new Byte[3500];
                                bytes = stream.Read(data, 0, data.Length);
                            }
                        }
                    }
                    response = Constants.DiversionEncoding.GetString(data, 0, bytes);

                    string errorCode = response.Substring(2, 2);
                    if (ErrorCodes.ContainsKey(errorCode))
                    {
                        // We log the call and set the success parameter to false
                        callContext.Fail();
                        error = ErrorCodes[errorCode];
                        return false;
                    }
                    else
                    {
                        // We log the call and set the success parameter to true
                        callContext.Succeed();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    // We log the call and set the success parameter to false
                    // TODO: Shall we just rely on the exception logging?
                    callContext.Fail();
                    response = null;
                    error = "Exception: " + e.Message;
                    return false;
                }
            }

        }

        #endregion

    }
}
