﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3615
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.3615.
// 
#pragma warning disable 1591

namespace CprBroker.EventBroker.EventsService {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="EventsSoap", Namespace="http://tempuri.org/")]
    public partial class Events : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private ApplicationHeader applicationHeaderValueField;
        
        private System.Threading.SendOrPostCallback DequeueDataChangeEventsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetPersonBirthdatesOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Events() {
            this.Url = global::CprBroker.EventBroker.Properties.Settings.Default.CprBroker_EventBroker_EventsService_Events;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public ApplicationHeader ApplicationHeaderValue {
            get {
                return this.applicationHeaderValueField;
            }
            set {
                this.applicationHeaderValueField = value;
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
        public event DequeueDataChangeEventsCompletedEventHandler DequeueDataChangeEventsCompleted;
        
        /// <remarks/>
        public event GetPersonBirthdatesCompletedEventHandler GetPersonBirthdatesCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("ApplicationHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DequeueDataChangeEvents", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public DataChangeEventInfo[] DequeueDataChangeEvents(int maxCount) {
            object[] results = this.Invoke("DequeueDataChangeEvents", new object[] {
                        maxCount});
            return ((DataChangeEventInfo[])(results[0]));
        }
        
        /// <remarks/>
        public void DequeueDataChangeEventsAsync(int maxCount) {
            this.DequeueDataChangeEventsAsync(maxCount, null);
        }
        
        /// <remarks/>
        public void DequeueDataChangeEventsAsync(int maxCount, object userState) {
            if ((this.DequeueDataChangeEventsOperationCompleted == null)) {
                this.DequeueDataChangeEventsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDequeueDataChangeEventsOperationCompleted);
            }
            this.InvokeAsync("DequeueDataChangeEvents", new object[] {
                        maxCount}, this.DequeueDataChangeEventsOperationCompleted, userState);
        }
        
        private void OnDequeueDataChangeEventsOperationCompleted(object arg) {
            if ((this.DequeueDataChangeEventsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DequeueDataChangeEventsCompleted(this, new DequeueDataChangeEventsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("ApplicationHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetPersonBirthdates", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public PersonBirthdate[] GetPersonBirthdates([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] System.Nullable<System.Guid> personUuidToStartAfter, int maxCount) {
            object[] results = this.Invoke("GetPersonBirthdates", new object[] {
                        personUuidToStartAfter,
                        maxCount});
            return ((PersonBirthdate[])(results[0]));
        }
        
        /// <remarks/>
        public void GetPersonBirthdatesAsync(System.Nullable<System.Guid> personUuidToStartAfter, int maxCount) {
            this.GetPersonBirthdatesAsync(personUuidToStartAfter, maxCount, null);
        }
        
        /// <remarks/>
        public void GetPersonBirthdatesAsync(System.Nullable<System.Guid> personUuidToStartAfter, int maxCount, object userState) {
            if ((this.GetPersonBirthdatesOperationCompleted == null)) {
                this.GetPersonBirthdatesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPersonBirthdatesOperationCompleted);
            }
            this.InvokeAsync("GetPersonBirthdates", new object[] {
                        personUuidToStartAfter,
                        maxCount}, this.GetPersonBirthdatesOperationCompleted, userState);
        }
        
        private void OnGetPersonBirthdatesOperationCompleted(object arg) {
            if ((this.GetPersonBirthdatesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPersonBirthdatesCompleted(this, new GetPersonBirthdatesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/", IsNullable=false)]
    public partial class ApplicationHeader : System.Web.Services.Protocols.SoapHeader {
        
        private string applicationTokenField;
        
        private string userTokenField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        public string ApplicationToken {
            get {
                return this.applicationTokenField;
            }
            set {
                this.applicationTokenField = value;
            }
        }
        
        /// <remarks/>
        public string UserToken {
            get {
                return this.userTokenField;
            }
            set {
                this.userTokenField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class PersonBirthdate {
        
        private System.Guid personUuidField;
        
        private System.DateTime birthdateField;
        
        /// <remarks/>
        public System.Guid PersonUuid {
            get {
                return this.personUuidField;
            }
            set {
                this.personUuidField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime Birthdate {
            get {
                return this.birthdateField;
            }
            set {
                this.birthdateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class DataChangeEventInfo {
        
        private System.Guid eventIdField;
        
        private System.Guid personUuidField;
        
        private System.DateTime receivedDateField;
        
        /// <remarks/>
        public System.Guid EventId {
            get {
                return this.eventIdField;
            }
            set {
                this.eventIdField = value;
            }
        }
        
        /// <remarks/>
        public System.Guid PersonUuid {
            get {
                return this.personUuidField;
            }
            set {
                this.personUuidField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime ReceivedDate {
            get {
                return this.receivedDateField;
            }
            set {
                this.receivedDateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void DequeueDataChangeEventsCompletedEventHandler(object sender, DequeueDataChangeEventsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DequeueDataChangeEventsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DequeueDataChangeEventsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public DataChangeEventInfo[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((DataChangeEventInfo[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void GetPersonBirthdatesCompletedEventHandler(object sender, GetPersonBirthdatesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPersonBirthdatesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPersonBirthdatesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public PersonBirthdate[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((PersonBirthdate[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591