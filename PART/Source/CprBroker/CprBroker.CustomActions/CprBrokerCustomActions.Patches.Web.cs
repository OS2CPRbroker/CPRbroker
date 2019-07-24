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
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Utilities;
using CprBroker.Data;
using CprBroker.Data.DataProviders;
using CprBroker.Installers;
using CprBroker.Installers.EventBrokerInstallers;
using CprBroker.Utilities.Config;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace CprBroker.CustomActions
{
    public partial class CprBrokerCustomActions
    {
        public static void PatchWebsite_1_3_0(SessionAdapter session)
        {
            var types = new Type[]
            {
                typeof(CprBroker.Providers.CPRDirect.CPRDirectClientDataProvider),
                typeof(CprBroker.Providers.CPRDirect.CPRDirectExtractDataProvider)
            };
            var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var configFilePath = webInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);

            // Add new node(s) for data providers
            CprBroker.Installers.Installation.AddKnownDataProviderTypes(types, configFilePath);
        }

        private static void PatchWebsite_2_1_1(SessionAdapter session)
        {
            // This patch adds /PersonMasterService12 to the address of existing person master data providers for versions prior to 2.1.1
            try
            {
                // Load connection string and encryption keys
                var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
                var configFilePath = webInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);
                var config = CprBroker.Installers.Installation.OpenConfigFile(configFilePath);
                DataProvider.EncryptionAlgorithm = DataProviderKeysSection.GetFromConfig(config);
                var connectionString = config.ConnectionStrings.ConnectionStrings["CprBroker.Config.Properties.Settings.CprBrokerConnectionString"].ConnectionString;

                using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext(connectionString))
                {
                    var providers = dataContext.DataProviders.ToArray();
                    providers = providers
                        .Where(dp =>
                        {
                            var type = Type.GetType(dp.TypeName, false, true);
                            return
                                type != null
                                && string.Equals(type.Name, typeof(CprBroker.Providers.PersonMaster.PersonMasterDataProvider).Name);
                        }
                        ).ToArray();

                    foreach (var prov in providers)
                    {
                        var adr = prov.Get("Address");
                        if (!string.IsNullOrEmpty(adr))
                        {
                            if (!adr.EndsWith("/PersonMasterService12", StringComparison.InvariantCultureIgnoreCase))
                            {
                                prov.Set("Address", prov.Get("Address") + "/PersonMasterService12");
                            }
                        }
                    }
                    dataContext.SubmitChanges();
                }
            }
            finally
            {
                // Unload encryption keys
                DataProvider.EncryptionAlgorithm = null;
            }
        }

        private static void PatchWebsite_2_2_2(SessionAdapter session)
        {
            var types = new Type[]
            {
                typeof(CprBroker.Engine.Events.DataChangeEventManager),
                typeof(CprBroker.Providers.Local.Search.LocalSearchDataProvider)
            };
            var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var configFilePath = webInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);


            // Remove node for data provider: DataChangeEventManager under Engine.dll
            CprBroker.Installers.Installation.RemoveSectionNode(configFilePath, "dataProviders/knownTypes/add[contains(@type, 'DataChangeEventManager') and contains(@type, 'Engine')]");

            // Add new node(s) for data providers
            CprBroker.Installers.Installation.AddKnownDataProviderTypes(types, configFilePath);
        }

        public static void ResetDataProviderSectionDefinitions(SessionAdapter session)
        {
            var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var eventWebInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "EVENT");

            // Ensure correct namespaces for config sections
            CprBroker.Installers.Installation.ResetDataProviderSectionDefinitions(webInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath));
            CprBroker.Installers.Installation.ResetDataProviderSectionDefinitions(eventWebInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.EventBrokerWebsiteDirectoryRelativePath));
            CprBroker.Installers.Installation.ResetDataProviderSectionDefinitions(EventBrokerCustomActions.GetServiceExeConfigFullFileName(session));
        }

        private static void PatchWebsite_2_2_3(SessionAdapter session)
        {
            ResetDataProviderSectionDefinitions(session);

            var cprWebInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var cprConfigFilePath = cprWebInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);

            // Add new node(s) for data providers
            var types = new Type[]
            {
                typeof(CprBroker.Providers.CprServices.CprServicesDataProvider),
            };
            CprBroker.Installers.Installation.AddKnownDataProviderTypes(types, cprConfigFilePath);

            var configFilePaths = new string[]
            {
                WebInstallationInfo.CreateFromFeature(session, "CPR").GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath),
                WebInstallationInfo.CreateFromFeature(session, "EVENT").GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath),
                EventBrokerCustomActions.GetServiceExeConfigFullFileName(session)
            };

            foreach (var configFilePath in configFilePaths)
            {
                var doc = new XmlDocument();
                doc.Load(configFilePath);
                var dataProviderKeysNode = doc.SelectSingleNode("//section[@name='dataProviderKeys']");
                var dataProvidersNode = doc.SelectSingleNode("//section[@name='dataProviders']");
                dataProviderKeysNode.Attributes["type"].Value = typeof(DataProviderKeysSection).AssemblyQualifiedName;
                dataProvidersNode.Attributes["type"].Value = typeof(DataProvidersConfigurationSection).AssemblyQualifiedName;
                doc.Save(configFilePath);
            }
        }

        private static void PatchWebsite_2_2_4(SessionAdapter session)
        {
            var cprWebInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var cprConfigFilePath = cprWebInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);
            // Add new node(s) for data providers
            var types = new Type[]
            {
                typeof(CprBroker.Providers.ServicePlatform.ServicePlatformDataProvider),
            };
            CprBroker.Installers.Installation.AddKnownDataProviderTypes(types, cprConfigFilePath);
        }

        private static void PatchWebsite_2_2_5(SessionAdapter session)
        {
            var cprWebInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var cprConfigFilePath = cprWebInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);
            // Add new node(s) for data providers
            var types = new Type[]
            {
                typeof(CprBroker.Providers.ServicePlatform.ServicePlatformExtractDataProvider),
            };
            CprBroker.Installers.Installation.AddKnownDataProviderTypes(types, cprConfigFilePath);

            // Set ASP.NET to target framework version                
            CprBroker.Installers.WebsiteCustomAction.RunRegIIS(string.Format("-s {0}", cprWebInstallationInfo.TargetWmiSubPath), new Version(4, 0));

            // ADD MVC Elements
            AddMvcElements(cprConfigFilePath);
        }

        public static void AddMvcElements(string configFilePath)
        {
            var templatePath = string.Format("{0}\\{1}.xml", new FileInfo(configFilePath).Directory.FullName, new Random().Next(10000, 100000));
            File.WriteAllText(templatePath, Properties.Resources.MvcWebConfig);

            CprBroker.Installers.Installation.RemoveXmlNode(
                configFilePath,
                "//configuration/configSections/sectionGroup[@name='system.web.extensions']");

            CprBroker.Installers.Installation.CopyConfigNode(
                "//configuration/configSections",
                "//configuration/configSections/sectionGroup[@name='system.web.webPages.razor']",
                "sectionGroup",
                templatePath, configFilePath, CprBroker.Installers.Installation.MergeOption.Overwrite);

            CprBroker.Installers.Installation.CopyConfigNode(
                "//configuration/system.web",
                "//configuration/system.web/compilation",
                "compilation",
                templatePath, configFilePath, CprBroker.Installers.Installation.MergeOption.Overwrite);

            CprBroker.Installers.Installation.CopyConfigNode(
                "//configuration/system.web",
                "//configuration/system.web/pages",
                "pages",
                templatePath, configFilePath, CprBroker.Installers.Installation.MergeOption.Overwrite);

            CprBroker.Installers.Installation.RemoveXmlNode(
                configFilePath,
                "//configuration/system.web/httpHandlers");

            CprBroker.Installers.Installation.RemoveXmlNode(
                configFilePath,
                "//configuration/system.web/httpModules");

            CprBroker.Installers.Installation.RemoveXmlNode(
                configFilePath,
                "//configuration/system.codedom");

            CprBroker.Installers.Installation.CopyConfigNode(
                "//configuration",
                "//configuration/runtime",
                "runtime",
                templatePath, configFilePath, CprBroker.Installers.Installation.MergeOption.Overwrite);

            CprBroker.Installers.Installation.CopyConfigNode(
                "//configuration",
                "//configuration/system.web.webPages.razor",
                "system.web.webPages.razor",
                templatePath, configFilePath, CprBroker.Installers.Installation.MergeOption.Overwrite);

            CprBroker.Installers.Installation.RemoveXmlNode(
                configFilePath,
                "//configuration/system.webServer/validation");

            CprBroker.Installers.Installation.RemoveXmlNode(
                configFilePath,
                "//configuration/system.webServer/modules");

            CprBroker.Installers.Installation.CopyConfigNode(
                "//configuration/system.webServer",
                "//configuration/system.webServer/handlers",
                "handlers",
                templatePath, configFilePath, CprBroker.Installers.Installation.MergeOption.Overwrite);
        }

        public static void PatchWebsite_2_2_7(SessionAdapter session)
        {
            var cprWebInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var eventWebInfo = WebInstallationInfo.CreateFromFeature(session, "EVENT");

            // Log4net settings file path
            var configs = new KeyValuePair<string, string>[]{
                new  KeyValuePair<string, string>(cprWebInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath), @"Config\log4net.config"),
                new  KeyValuePair<string, string>(eventWebInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.EventBrokerWebsiteDirectoryRelativePath), @"Config\log4net.config"),
                new  KeyValuePair<string, string>(EventBrokerCustomActions.GetServiceExeConfigFullFileName(session), "log4net.config")
            };

            foreach (var kvp in configs)
            {
                CprBroker.Installers.Installation.SetApplicationSettingInConfigFile(
                    configFileName: kvp.Key,
                    settingsType: typeof(CprBroker.Config.Properties.Settings),
                    settingName: "Log4NetConfig",
                    value: kvp.Value);
            }
        }

        public static void PatchWebsite_2_3_0(SessionAdapter session)
        {
            var types = new Type[]
            {
                typeof(CprBroker.Slet.RemovePersonDataProvider)
            };
            var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var configFilePath = webInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);

            // Add new node(s) for data providers
            CprBroker.Installers.Installation.AddKnownDataProviderTypes(types, configFilePath);
        }

        public static void PatchWebsite_2_3_6(SessionAdapter session)
        {
            var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var configFilePath = webInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);

            // Move ExcludedMunicipalityCodes from Tracking to CPRDirect settings
            XmlNode node = CprBroker.Installers.Installation.RemoveXmlNode(configFilePath,"/configuration/applicationSettings/CprBroker.PartInterface.Tracking.Properties.Settings/setting[@name='ExcludedMunicipalityCodes']");
            if (node != null)
            {
                CprBroker.Installers.Installation.AddConfigSectionDefinition(configFilePath, "applicationSettings", "CprBroker.PartInterface.Tracking.Properties.Settings", typeof(System.Configuration.ClientSettingsSection));
                // TODO Add the actual node to the new config for CPR Direct.
            }
        }
    }
}
