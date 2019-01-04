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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using System.Web;
using System.Web.Services;

namespace CprBroker.Engine
{
    /// <summary>
    /// Used to guarantee that a StandardReturType object is always returned from web methods, even if the request is wrong
    /// </summary>
    public class SchemaSoapExtension : System.Web.Services.Protocols.SoapExtension
    {
        #region Initialization

        Stream oldStream;
        Stream newStream;


        public override object GetInitializer(Type serviceType)
        {
            return "";
        }

        public override object GetInitializer(System.Web.Services.Protocols.LogicalMethodInfo methodInfo, System.Web.Services.Protocols.SoapExtensionAttribute attribute)
        {
            return "";
        }

        public override void Initialize(object initializer)
        {

        }

        bool IsServerSide()
        {
            return oldStream!=null;
        }

        bool FirstPhase = true;

        public override System.IO.Stream ChainStream(System.IO.Stream stream)
        {
            Stream ret;

            if (FirstPhase && stream.GetType().Name == "SoapExtensionStream")
            {
                ret = base.ChainStream(stream);
            }
            else if (!FirstPhase && oldStream == null)
            {
                ret = base.ChainStream(stream);
            }
            else
            {
                oldStream = stream;
                newStream = new MemoryStream();
                ret = newStream;
            }

            FirstPhase = false;
            return ret;
        }

        #endregion

        #region Utility classes & methods

        class HeaderException : Exception
        {
        }
        class MissingRequestException : Exception
        {
        }

        private void Copy(Stream from, Stream to)
        {
            if (from.CanSeek)
            {
                from.Position = 0;
            }
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
            if (to.CanSeek)
            {
                to.Seek(0, SeekOrigin.Begin);
            }
        }

        private IBasicOutput CreateReturnValue(SoapMessage message)
        {
            Exception ex = Misc.GetDeepestInnerException(message.Exception);
            var ret = Utilities.Reflection.CreateInstance(message.MethodInfo.ReturnType);
            var output = ret as Schemas.Part.IBasicOutput;
            if (ex is XmlException)
            {
                output.StandardRetur = StandardReturType.MalformedXml();
            }
            else if (ex is HeaderException)
            {
                output.StandardRetur = StandardReturType.NullInput("applicationHeader");
            }
            else if (ex is MissingRequestException)
            {
                output.StandardRetur = StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Missing request element: {0}", message.MethodInfo.Name));
            }
            else if (ex is InvalidOperationException && Misc.ExceptionTreeContainsText(message.Exception, "XML"))
            {
                output.StandardRetur = StandardReturType.RequestUnreadable(ex.Message);
            }
            else if(ex is FormatException)
            {
                output.StandardRetur = StandardReturType.RequestUnreadable(ex.Message);
            }
            else 
            {
                output.StandardRetur = StandardReturType.UnspecifiedError();
            }
            return output;
        }

        private void GetWebMethodNames(SoapMessage message, out string messageName, out string serviceNamespace)
        {
            messageName = message.MethodInfo.Name;
            serviceNamespace = "http://tempuri.org";
            if (message.MethodInfo.MethodInfo.IsDefined(typeof(WebMethodAttribute), true))
            {
                var methodAttr = message.MethodInfo.MethodInfo.GetCustomAttributes(typeof(WebMethodAttribute), true)[0] as WebMethodAttribute;
                if (!string.IsNullOrEmpty(methodAttr.MessageName))
                {
                    messageName = methodAttr.MessageName;
                }

                if (message.MethodInfo.MethodInfo.DeclaringType.IsDefined(typeof(WebServiceAttribute), true))
                {
                    var serviceAttr = message.MethodInfo.MethodInfo.DeclaringType.GetCustomAttributes(typeof(WebServiceAttribute), true)[0] as WebServiceAttribute;
                    if (!string.IsNullOrEmpty(serviceAttr.Namespace))
                    {
                        serviceNamespace = serviceAttr.Namespace;
                    }
                }
            }
        }

        private XmlNode GetBodyNode(Stream sourceStream)
        {
            // Now modify the response
            // Beemen
            if (sourceStream.CanSeek)
            {
                sourceStream.Position = 0;
            }
            TextReader reader = new StreamReader(sourceStream);
            string xml = reader.ReadToEnd();

            if (string.IsNullOrEmpty(xml))
            {
                xml = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"> <soap:Body></soap:Body></soap:Envelope>";
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");

            var bodyNode = doc.SelectSingleNode("//soap:Body", nsmgr);
            return bodyNode;
        }

        private XmlNode GetMethodNode(XmlNode bodyNode, string messageName, string serviceNamespace)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(bodyNode.OwnerDocument.NameTable);
            nsmgr.AddNamespace("rt", serviceNamespace);
            var methodNode = bodyNode.SelectSingleNode(string.Format("rt:{0}", messageName), nsmgr);
            return methodNode;
        }

        private void WriteResponse(XmlDocument responseSoap)
        {
            StreamWriter writer = new StreamWriter(oldStream);
            writer.Write(responseSoap.OuterXml);
            writer.Flush();
        }

        #endregion

        public override void ProcessMessage(SoapMessage message)
        {
            if (IsServerSide())
            {
                switch (message.Stage)
                {
                    case SoapMessageStage.BeforeDeserialize:
                        OnBeforeDeserialize(message);
                        break;

                    case SoapMessageStage.AfterDeserialize:
                        OnAfterDeserialize(message);
                        break;

                    case SoapMessageStage.BeforeSerialize:
                        break;

                    case SoapMessageStage.AfterSerialize:
                        OnAfterSerialize(message);
                        break;
                }
            }
        }


        void OnBeforeDeserialize(SoapMessage message)
        {
            // Validate that there is a node that matched the message action
            string messageName, serviceNamespace;
            GetWebMethodNames(message, out messageName, out serviceNamespace);

            var bodyNode = GetBodyNode(oldStream);
            var methodNode = GetMethodNode(bodyNode, messageName, serviceNamespace);
            if (methodNode == null)
            {
                throw new MissingRequestException();
            }
            Copy(oldStream, newStream);
        }

        void OnAfterDeserialize(SoapMessage message)
        {
            var soapHeaderAttributes = message.MethodInfo.MethodInfo.GetCustomAttributes(typeof(SoapHeaderAttribute), true);
            foreach (SoapHeaderAttribute attr in soapHeaderAttributes)
            {
                string headerName = attr.MemberName;
                var headerMember = message.MethodInfo.MethodInfo.DeclaringType.GetMember
                    (headerName,
                    System.Reflection.MemberTypes.Field,
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).FirstOrDefault();
                if (typeof(Schemas.ApplicationHeader).IsAssignableFrom((headerMember as System.Reflection.FieldInfo).FieldType))
                {
                    foreach (var header in message.Headers)
                    {
                        if (header is Schemas.ApplicationHeader)
                        {
                            return;
                        }
                    }
                    throw new HeaderException();
                }
            }
        }

        void OnAfterSerialize(SoapMessage message)
        {
            if (message.Exception != null && typeof(IBasicOutput).IsAssignableFrom(message.MethodInfo.ReturnType))
            {
                BrokerContext.Initialize(Constants.BaseApplicationToken.ToString(), Constants.UserToken);
                Local.Admin.LogException(message.Exception);
                var output = CreateReturnValue(message);

                string messageName, serviceNamespace;
                GetWebMethodNames(message, out messageName, out serviceNamespace);

                var bodyNode = GetBodyNode(newStream);
                var outNode = bodyNode.OwnerDocument.CreateElement("", messageName + "Response", serviceNamespace);

                string bodyInnerXml = Strings.SerializeObject(output, true);
                outNode.InnerXml = bodyInnerXml;

                bodyNode.RemoveAll();
                bodyNode.AppendChild(outNode);

                WriteResponse(bodyNode.OwnerDocument);
                return;
            }

            Copy(newStream, oldStream);
        }

    }
}
