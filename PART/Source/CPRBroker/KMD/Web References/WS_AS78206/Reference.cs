﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.3603.
// 
#pragma warning disable 1591

namespace CPRBroker.Providers.KMD.WS_AS78206 {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="SoapBinding", Namespace="http://zsrsoap.kmd.dk/AS78206")]
    public partial class WS_AS78206 : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private userinfo userinfoValueField;
        
        private System.Threading.SendOrPostCallback SubmitAS78206OperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public WS_AS78206() {
            this.Url = "http://195.50.36.114/bccicste.asp?zservice=AS78206";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public userinfo userinfoValue {
            get {
                return this.userinfoValueField;
            }
            set {
                this.userinfoValueField = value;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event SubmitAS78206CompletedEventHandler SubmitAS78206Completed;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("userinfoValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("AS78206", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("AS78206Response", Namespace="http://zsrsoap.kmd.dk/AS78206")]
        public AS78206Response SubmitAS78206([System.Xml.Serialization.XmlElementAttribute(Namespace="http://zsrsoap.kmd.dk/AS78206")] AS78206 AS78206) {
            object[] results = this.Invoke("SubmitAS78206", new object[] {
                        AS78206});
            return ((AS78206Response)(results[0]));
        }
        
        /// <remarks/>
        public void SubmitAS78206Async(AS78206 AS78206) {
            this.SubmitAS78206Async(AS78206, null);
        }
        
        /// <remarks/>
        public void SubmitAS78206Async(AS78206 AS78206, object userState) {
            if ((this.SubmitAS78206OperationCompleted == null)) {
                this.SubmitAS78206OperationCompleted = new System.Threading.SendOrPostCallback(this.OnSubmitAS78206OperationCompleted);
            }
            this.InvokeAsync("SubmitAS78206", new object[] {
                        AS78206}, this.SubmitAS78206OperationCompleted, userState);
        }
        
        private void OnSubmitAS78206OperationCompleted(object arg) {
            if ((this.SubmitAS78206Completed != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SubmitAS78206Completed(this, new SubmitAS78206CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://soap.zsroer.kmd.dk")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://soap.zsroer.kmd.dk", IsNullable=false)]
    public partial class userinfo : System.Web.Services.Protocols.SoapHeader {
        
        private string useridField;
        
        private string passwordField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string userid {
            get {
                return this.useridField;
            }
            set {
                this.useridField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://zsrsoap.kmd.dk/AS78206")]
    public partial class AS782_2 {
        
        private string rETURKODEField;
        
        private string rETURTEXTField;
        
        private string eKOMField;
        
        private string eINRKMDField;
        
        private string ePNRField;
        
        private string dFOEDSField;
        
        private string cSTATUKField;
        
        private string cSTATUCField;
        
        private string dSTATUSField;
        
        private string cPNRMRKField;
        
        private string cTRANSField;
        
        private string eFOEDREGField;
        
        private string aFOEDREGField;
        
        private string aSTILField;
        
        private string aSTIL34Field;
        
        private string cSTILField;
        
        private string aNAVNField;
        
        private string aNAVN34Field;
        
        private string cNVMRKField;
        
        private string dNAVNField;
        
        private string eNVNMYNField;
        
        private string eSTATField;
        
        private string dADRField;
        
        private string cADRBSKField;
        
        private string cVEJField;
        
        private string aHUSNRField;
        
        private string aBOGSTVField;
        
        private string aETAGEField;
        
        private string aSIDOERField;
        
        private string eBYGField;
        
        private string dTILFLField;
        
        private string cFRAADRField;
        
        private string eFRASTDField;
        
        private string aFRAADRField;
        
        private string cKIRKEField;
        
        private string cUMYNDField;
        
        private string cKOMF1Field;
        
        private string cKOMF2Field;
        
        private string cCIVSField;
        
        private string dCIVSField;
        
        private string eCIVMYNField;
        
        private string aCIVMYNField;
        
        private string ePNRAEGTField;
        
        private string ePNRMORField;
        
        private string cVERMORField;
        
        private string ePNRFARField;
        
        private string cVERFARField;
        
        private string fBOERNField;
        
        private string[] ePNRBRNField;
        
        private string[] cVERBRNField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RETURKODE {
            get {
                return this.rETURKODEField;
            }
            set {
                this.rETURKODEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RETURTEXT {
            get {
                return this.rETURTEXTField;
            }
            set {
                this.rETURTEXTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EKOM {
            get {
                return this.eKOMField;
            }
            set {
                this.eKOMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EINRKMD {
            get {
                return this.eINRKMDField;
            }
            set {
                this.eINRKMDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EPNR {
            get {
                return this.ePNRField;
            }
            set {
                this.ePNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DFOEDS {
            get {
                return this.dFOEDSField;
            }
            set {
                this.dFOEDSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CSTATUK {
            get {
                return this.cSTATUKField;
            }
            set {
                this.cSTATUKField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CSTATUC {
            get {
                return this.cSTATUCField;
            }
            set {
                this.cSTATUCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DSTATUS {
            get {
                return this.dSTATUSField;
            }
            set {
                this.dSTATUSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CPNRMRK {
            get {
                return this.cPNRMRKField;
            }
            set {
                this.cPNRMRKField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CTRANS {
            get {
                return this.cTRANSField;
            }
            set {
                this.cTRANSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EFOEDREG {
            get {
                return this.eFOEDREGField;
            }
            set {
                this.eFOEDREGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AFOEDREG {
            get {
                return this.aFOEDREGField;
            }
            set {
                this.aFOEDREGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ASTIL {
            get {
                return this.aSTILField;
            }
            set {
                this.aSTILField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ASTIL34 {
            get {
                return this.aSTIL34Field;
            }
            set {
                this.aSTIL34Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CSTIL {
            get {
                return this.cSTILField;
            }
            set {
                this.cSTILField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ANAVN {
            get {
                return this.aNAVNField;
            }
            set {
                this.aNAVNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ANAVN34 {
            get {
                return this.aNAVN34Field;
            }
            set {
                this.aNAVN34Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CNVMRK {
            get {
                return this.cNVMRKField;
            }
            set {
                this.cNVMRKField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DNAVN {
            get {
                return this.dNAVNField;
            }
            set {
                this.dNAVNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ENVNMYN {
            get {
                return this.eNVNMYNField;
            }
            set {
                this.eNVNMYNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ESTAT {
            get {
                return this.eSTATField;
            }
            set {
                this.eSTATField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DADR {
            get {
                return this.dADRField;
            }
            set {
                this.dADRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CADRBSK {
            get {
                return this.cADRBSKField;
            }
            set {
                this.cADRBSKField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CVEJ {
            get {
                return this.cVEJField;
            }
            set {
                this.cVEJField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AHUSNR {
            get {
                return this.aHUSNRField;
            }
            set {
                this.aHUSNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ABOGSTV {
            get {
                return this.aBOGSTVField;
            }
            set {
                this.aBOGSTVField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AETAGE {
            get {
                return this.aETAGEField;
            }
            set {
                this.aETAGEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ASIDOER {
            get {
                return this.aSIDOERField;
            }
            set {
                this.aSIDOERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EBYG {
            get {
                return this.eBYGField;
            }
            set {
                this.eBYGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DTILFL {
            get {
                return this.dTILFLField;
            }
            set {
                this.dTILFLField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CFRAADR {
            get {
                return this.cFRAADRField;
            }
            set {
                this.cFRAADRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EFRASTD {
            get {
                return this.eFRASTDField;
            }
            set {
                this.eFRASTDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AFRAADR {
            get {
                return this.aFRAADRField;
            }
            set {
                this.aFRAADRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CKIRKE {
            get {
                return this.cKIRKEField;
            }
            set {
                this.cKIRKEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CUMYND {
            get {
                return this.cUMYNDField;
            }
            set {
                this.cUMYNDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CKOMF1 {
            get {
                return this.cKOMF1Field;
            }
            set {
                this.cKOMF1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CKOMF2 {
            get {
                return this.cKOMF2Field;
            }
            set {
                this.cKOMF2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CCIVS {
            get {
                return this.cCIVSField;
            }
            set {
                this.cCIVSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DCIVS {
            get {
                return this.dCIVSField;
            }
            set {
                this.dCIVSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ECIVMYN {
            get {
                return this.eCIVMYNField;
            }
            set {
                this.eCIVMYNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ACIVMYN {
            get {
                return this.aCIVMYNField;
            }
            set {
                this.aCIVMYNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EPNRAEGT {
            get {
                return this.ePNRAEGTField;
            }
            set {
                this.ePNRAEGTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EPNRMOR {
            get {
                return this.ePNRMORField;
            }
            set {
                this.ePNRMORField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CVERMOR {
            get {
                return this.cVERMORField;
            }
            set {
                this.cVERMORField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EPNRFAR {
            get {
                return this.ePNRFARField;
            }
            set {
                this.ePNRFARField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CVERFAR {
            get {
                return this.cVERFARField;
            }
            set {
                this.cVERFARField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FBOERN {
            get {
                return this.fBOERNField;
            }
            set {
                this.fBOERNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("EPNRBRN", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] EPNRBRN {
            get {
                return this.ePNRBRNField;
            }
            set {
                this.ePNRBRNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CVERBRN", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] CVERBRN {
            get {
                return this.cVERBRNField;
            }
            set {
                this.cVERBRNField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://zsrsoap.kmd.dk/AS78206")]
    public partial class PARM {
        
        private string eKOMField;
        
        private string ePNRField;
        
        private string cOMRAADEField;
        
        private string cSTATUSField;
        
        private string cBESTILField;
        
        private string cREDIGField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EKOM {
            get {
                return this.eKOMField;
            }
            set {
                this.eKOMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EPNR {
            get {
                return this.ePNRField;
            }
            set {
                this.ePNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COMRAADE {
            get {
                return this.cOMRAADEField;
            }
            set {
                this.cOMRAADEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CSTATUS {
            get {
                return this.cSTATUSField;
            }
            set {
                this.cSTATUSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CBESTIL {
            get {
                return this.cBESTILField;
            }
            set {
                this.cBESTILField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CREDIG {
            get {
                return this.cREDIGField;
            }
            set {
                this.cREDIGField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://zsrsoap.kmd.dk/AS78206")]
    public partial class AS78206 {
        
        private PARM inputRecordField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public PARM InputRecord {
            get {
                return this.inputRecordField;
            }
            set {
                this.inputRecordField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://zsrsoap.kmd.dk/AS78206")]
    public partial class AS78206Response {
        
        private AS782_2 outputRecordField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AS782_2 OutputRecord {
            get {
                return this.outputRecordField;
            }
            set {
                this.outputRecordField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void SubmitAS78206CompletedEventHandler(object sender, SubmitAS78206CompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SubmitAS78206CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SubmitAS78206CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public AS78206Response Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((AS78206Response)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591