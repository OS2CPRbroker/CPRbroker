// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 2.1.3314.24787
//    <NameSpace>CPRBroker.Schemas</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><HidePrivateFieldInIDE>True</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><IncludeSerializeMethod>False</IncludeSerializeMethod><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><DisableDebug>True</DisableDebug><CustomUsings></CustomUsings>
//  <auto-generated>
// ------------------------------------------------------------------------------
namespace CPRBroker.Schemas
{
    using System.Collections.Generic;
    using System.ComponentModel;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://rep.oio.dk/cpr.dk/xml/schemas/2008/05/01/")]
    [System.Xml.Serialization.XmlRootAttribute("ApplicationStructure", Namespace = "http://rep.oio.dk/cpr.dk/xml/schemas/2008/05/01/", IsNullable = false)]
    public partial class ApplicationStructureType
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<ApplicationStructureTypeBusinessApplications> businessApplicationsField;

        public ApplicationStructureType()
        {
            if ((this.businessApplicationsField == null))
            {
                this.businessApplicationsField = new List<ApplicationStructureTypeBusinessApplications>();
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BusinessApplications")]
        public List<ApplicationStructureTypeBusinessApplications> BusinessApplications
        {
            get
            {
                return this.businessApplicationsField;
            }
            set
            {
                this.businessApplicationsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3053")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://rep.oio.dk/cpr.dk/xml/schemas/2008/05/01/")]
    public partial class ApplicationStructureTypeBusinessApplications
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string applicationIdField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string tokenField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string nameField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private System.DateTime registrationDateField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool isApprovedField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private System.DateTime approvedDateField;

        /// <remarks/>
        public string ApplicationId
        {
            get
            {
                return this.applicationIdField;
            }
            set
            {
                this.applicationIdField = value;
            }
        }

        /// <remarks/>
        public string Token
        {
            get
            {
                return this.tokenField;
            }
            set
            {
                this.tokenField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public System.DateTime RegistrationDate
        {
            get
            {
                return this.registrationDateField;
            }
            set
            {
                this.registrationDateField = value;
            }
        }

        /// <remarks/>
        public bool IsApproved
        {
            get
            {
                return this.isApprovedField;
            }
            set
            {
                this.isApprovedField = value;
            }
        }

        /// <remarks/>
        public System.DateTime ApprovedDate
        {
            get
            {
                return this.approvedDateField;
            }
            set
            {
                this.approvedDateField = value;
            }
        }
    }
}
