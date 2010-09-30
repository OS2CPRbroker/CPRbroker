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

namespace NUnitTester.Access {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="AccessSoap", Namespace="http://tempuri.org/")]
    public partial class Access : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback SendNotificationsOperationCompleted;
        
        private System.Threading.SendOrPostCallback RefreshPersonsDataOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Access() {
            this.Url = "http://localhost:1551/services/Access.asmx";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
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
        public event SendNotificationsCompletedEventHandler SendNotificationsCompleted;
        
        /// <remarks/>
        public event RefreshPersonsDataCompletedEventHandler RefreshPersonsDataCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendNotifications", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public SendNotificationsResult SendNotifications(System.DateTime today) {
            object[] results = this.Invoke("SendNotifications", new object[] {
                        today});
            return ((SendNotificationsResult)(results[0]));
        }
        
        /// <remarks/>
        public void SendNotificationsAsync(System.DateTime today) {
            this.SendNotificationsAsync(today, null);
        }
        
        /// <remarks/>
        public void SendNotificationsAsync(System.DateTime today, object userState) {
            if ((this.SendNotificationsOperationCompleted == null)) {
                this.SendNotificationsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendNotificationsOperationCompleted);
            }
            this.InvokeAsync("SendNotifications", new object[] {
                        today}, this.SendNotificationsOperationCompleted, userState);
        }
        
        private void OnSendNotificationsOperationCompleted(object arg) {
            if ((this.SendNotificationsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendNotificationsCompleted(this, new SendNotificationsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/RefreshPersonsData", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public RefreshPersonsDataResult RefreshPersonsData() {
            object[] results = this.Invoke("RefreshPersonsData", new object[0]);
            return ((RefreshPersonsDataResult)(results[0]));
        }
        
        /// <remarks/>
        public void RefreshPersonsDataAsync() {
            this.RefreshPersonsDataAsync(null);
        }
        
        /// <remarks/>
        public void RefreshPersonsDataAsync(object userState) {
            if ((this.RefreshPersonsDataOperationCompleted == null)) {
                this.RefreshPersonsDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRefreshPersonsDataOperationCompleted);
            }
            this.InvokeAsync("RefreshPersonsData", new object[0], this.RefreshPersonsDataOperationCompleted, userState);
        }
        
        private void OnRefreshPersonsDataOperationCompleted(object arg) {
            if ((this.RefreshPersonsDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RefreshPersonsDataCompleted(this, new RefreshPersonsDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    public partial class SendNotificationsResult {
        
        private System.Guid[] sentNotificationIdsField;
        
        private System.Guid[] failedNotificationIdsField;
        
        /// <remarks/>
        public System.Guid[] SentNotificationIds {
            get {
                return this.sentNotificationIdsField;
            }
            set {
                this.sentNotificationIdsField = value;
            }
        }
        
        /// <remarks/>
        public System.Guid[] FailedNotificationIds {
            get {
                return this.failedNotificationIdsField;
            }
            set {
                this.failedNotificationIdsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class RefreshPersonsDataResult {
        
        private string[] succeededCprNumbersField;
        
        private string[] failedCprNumbersField;
        
        /// <remarks/>
        public string[] SucceededCprNumbers {
            get {
                return this.succeededCprNumbersField;
            }
            set {
                this.succeededCprNumbersField = value;
            }
        }
        
        /// <remarks/>
        public string[] FailedCprNumbers {
            get {
                return this.failedCprNumbersField;
            }
            set {
                this.failedCprNumbersField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void SendNotificationsCompletedEventHandler(object sender, SendNotificationsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SendNotificationsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SendNotificationsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public SendNotificationsResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((SendNotificationsResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    public delegate void RefreshPersonsDataCompletedEventHandler(object sender, RefreshPersonsDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RefreshPersonsDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RefreshPersonsDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public RefreshPersonsDataResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((RefreshPersonsDataResult)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591