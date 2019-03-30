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
using System.IO;
using System.Xml;
using CprBroker.Schemas.Part;
using CprBroker.Engine;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.CprServices
{
    public partial class CprServicesDataProvider
    {
        public Kvit Send(string op, string input, ref string token, out string ret)
        {
            if (string.IsNullOrEmpty(token))
                token = Constants.DefaultToken;

            //var baseUrl = "";
            //baseUrl = "https://gctp-demo.cpr.dk/cpr-online-gctp/gctp";
            //var logOnPath = "/cics/dmwg/cscwbsgn";
            //var cprApplicationPath = "/cpcacpra/ajou/xyz";

            using (var callContext = this.BeginCall(op, op))
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Prepare the request object
                HttpWebRequest req = WebRequest.Create(this.Address) as HttpWebRequest;
                req.Method = WebRequestMethods.Http.Post;
                req.UserAgent = Constants.UserAgent;
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(new Uri(this.Address), new Cookie(Constants.TokenCookieName, token));
                req.ContentType = "text/xml";
                req.ContentLength = input.Length;

                // Fill the input and get response
                using (var s = req.GetRequestStream())
                {
                    // Fill the posted data
                    StreamWriter w = new StreamWriter(s, Constants.XmlEncoding);
                    w.Write(input);
                    w.Flush();

                    // Get the response
                    var resp = req.GetResponse();
                    using (var rd = new StreamReader(resp.GetResponseStream(), Constants.XmlEncoding))
                    {
                        ret = rd.ReadToEnd();

                        // Update token if possible
                        var t = Utils.GetToken(resp.Headers);
                        if (!string.IsNullOrEmpty(t))
                            token = t;

                        // Final return
                        var kvit = Kvit.FromResponseXml(ret);
                        if (kvit.OK)
                        {
                            callContext.Succeed();
                        }
                        else
                        {
                            callContext.Fail();
                        }
                        return kvit;
                    }
                }
            }
        }

        public string SignonAndGetToken()
        {
            var inp = Properties.Resources.OrdinaryLogon;
            var doc = new XmlDocument();
            doc.LoadXml(inp);
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("cpr", Constants.XmlNamespace);
            var node = doc.SelectSingleNode("//cpr:Sik", mgr);
            node.Attributes["userid"].Value = this.UserId;
            node.Attributes["password"].Value = this.Password;

            string token = null;
            string ret;
            var kvit = Send(Constants.OperationKeys.signon, doc.OuterXml, ref token, out ret);
            if (kvit.OK)
            {
                return token;
            }
            else
            {
                Admin.LogFormattedError("Signon failed: <{0}><{1}>", kvit.ReturnCode, kvit.ReturnText);
                return null;
            }
        }

        public Kvit ChangePassword(string newPassword)
        {
            var inp = Properties.Resources.ChangePassword;
            var doc = new XmlDocument();
            doc.LoadXml(inp);
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("cpr", Constants.XmlNamespace);
            var node = doc.SelectSingleNode("//cpr:Sik", mgr);
            node.Attributes["userid"].Value = this.UserId;
            node.Attributes["password"].Value = this.Password;
            node.Attributes["newpass1"].Value = newPassword;


            string token = null;
            var ret = "";
            return Send(Constants.OperationKeys.newpass, doc.OuterXml, ref token, out ret);
        }


    }
}
