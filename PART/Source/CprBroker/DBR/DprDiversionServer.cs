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
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using CprBroker.Engine;

namespace CprBroker.DBR
{
    public class DprDiversionServer : TcpServer
    {
        public DbrQueue DbrQueue;

        public DprDiversionServer()
        {
            this.InputMessageSize = 40;
            this.MaxWait = TimeSpan.FromMilliseconds(500);
            this.TextEncoding = CprBroker.Providers.DPR.Constants.DiversionEncoding;
        }

        public override byte[] ProcessMessage(byte[] message)
        {
            try
            {
                var req = DiversionRequest.Parse(message);
                if (req != null)
                {
                    bool skipAddressIfDead = false;
                    try
                    {
                        skipAddressIfDead = DbrQueue.IgnoreAddressOfDeadPeople;
                    }
                    catch (Exception ex)
                    {
                        CprBroker.Engine.Local.Admin.LogError(String.Format("DBR Diversion could not get configuration, the following exception happened:{1}",ex.ToString()));
                    }
                    var ret = req.Process(ConnectionString, skipAddressIfDead);
                    return ret.ToBytes();
                }
                else
                {
                    // Invalid request. This should never be reached                    
                    return new byte[0];
                }
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
                return new byte[0];
            }
        }

        public virtual string ConnectionString
        {
            get
            {
                return this.DbrQueue.ConnectionString;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} : queue<{1}> port<{2}>",
                GetType().Name,
                this.DbrQueue.QueueId,
                this.Port
                );
        }

        public class EqualityComparer : IEqualityComparer<DprDiversionServer>
        {
            public bool Equals(DprDiversionServer x, DprDiversionServer y)
            {
                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the products' properties are equal.
                return x.DbrQueue.QueueId == y.DbrQueue.QueueId
                    && x.Port == y.Port;
            }

            public int GetHashCode(DprDiversionServer x)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(x, null))
                    return 0;

                return x.DbrQueue.QueueId.GetHashCode();
            }
        }
    }


}
