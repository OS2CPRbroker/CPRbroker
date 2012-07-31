﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using System.IO;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectExtractDataProvider : IPartReadDataProvider, IExternalDataProvider
    {
        #region IPartReadDataProvider members
        public RegistreringType1 Read(CprBroker.Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            ql = QualityLevel.Cpr;
            IndividualResponseType response = null;

            response = ExtractManager.GetPerson(uuid.CprNumber);

            if (response != null)
            {
                DateTime effectDate = DateTime.Today;
                return response.ToRegistreringType1(cpr2uuidFunc, effectDate);
            }
            return null;
        }
        #endregion

        #region IDataProvider members
        public bool IsAlive()
        {
            try
            {
                if (Directory.Exists(this.ExtractsFolder))
                {
                    // Try to create and move file
                    var filePath = CprBroker.Utilities.Strings.NewUniquePath(this.ExtractsFolder, "txt");
                    File.WriteAllText(filePath, "ABC");
                    string ss = File.ReadAllText(filePath);
                    var tmpFile = ExtractManager.MoveToProcessed(this.ExtractsFolder, filePath);
                    File.Delete(tmpFile);
                    // Now we are sure the folder is accessible with read and write permissions
                    return true;
                }
            }
            catch { }
            return false;
        }

        public Version Version
        {
            get { return new Version(Utilities.Constants.Versioning.Major, Utilities.Constants.Versioning.Minor); }
        }
        #endregion

        #region IExternalDataProvider Members
        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] { 
                    new DataProviderConfigPropertyInfo(){ Name=Constants.PropertyNames.ExtractsFolder, Type= DataProviderConfigPropertyInfoTypes.String, Required=true, Confidential=false}
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
        public string ExtractsFolder
        {
            get
            { return ConfigurationProperties[Constants.PropertyNames.ExtractsFolder]; }
        }
        #endregion
    }
}