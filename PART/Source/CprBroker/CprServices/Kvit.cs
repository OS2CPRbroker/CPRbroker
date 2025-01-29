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
using System.Xml;
using System.Xml.Serialization;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.CprServices
{
    public partial class Kvit
    {
        [XmlAttribute("r")]
        public string Row { get; set; }

        [XmlAttribute("t")]
        public string ReturnText { get; set; }

        [XmlAttribute("v")]
        public string ReturnCode { get; set; }

        [XmlIgnore]
        public string ReturnText2 { get; set; }

        public static Kvit FromResponseXml(string responseText)
        {
            var doc = new XmlDocument();
            doc.LoadXml(responseText);
            return FromXmlDocument(doc);
        }

        public static Kvit FromXmlDocument(XmlDocument doc)
        {
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cpr", Constants.XmlNamespace);
            var kvitNode = doc.SelectSingleNode("//cpr:Kvit", ns);
            if (kvitNode != null)
            {
                var ret = new Kvit();

                try
                {
                    ret.Row = kvitNode.Attributes["r"].Value;
                }
                catch (Exception ex)
                {
                    Admin.LogFormattedError("Error setting ret.Row: {0}", ex.Message);
                    ret.Row = "Unknown";
                }

                try
                {
                    ret.ReturnCode = kvitNode.Attributes["v"].Value;
                }
                catch (Exception ex)
                {
                    Admin.LogFormattedError("Error setting ret.ReturnCode: {0}", ex.Message);
                    ret.ReturnCode = "Unknown";
                }

                try
                {
                    ret.ReturnText = kvitNode.Attributes["t"].Value;
                }
                catch (Exception ex)
                {
                    Admin.LogFormattedError("Error setting ret.ReturnText: {0}", ex.Message);
                    ret.ReturnText = "No text";
                }


                if (kvitNode.ChildNodes.Count > 0)
                {
                    ret.ReturnText2 = string.Join(Environment.NewLine, new Responses.ErrorResponse(kvitNode.OuterXml).ToErrorLines());
                }

                return ret;
            }
            return null;
        }

        public bool OK
        {
            get { return ReturnCode == "900" || Row == "Ok"; }
        }

    }
}
