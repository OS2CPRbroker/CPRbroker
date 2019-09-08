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
using System.IO;
using System.Configuration.Install;
using System.Xml;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Security;
using System.Security.AccessControl;
using System.Diagnostics;
using System.ServiceProcess;
using CprBroker.Utilities;
using CprBroker.Utilities.Config;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CprBroker.Installers
{
    /// <summary>
    /// Utility class with methods that assist the installation process by extending the Installer class
    /// </summary>
    public static partial class Installation
    {
        #region Generic

        public static Configuration OpenConfigFile(string configFilePath)
        {
            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configFilePath;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            return configuration;
        }

        public static XmlNode RemoveXmlNode(string configFileName, string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode node = doc.SelectSingleNode(path);
            if (node != null)
            {
                var parentNode = node.ParentNode;
                parentNode.RemoveChild(node);
                doc.Save(configFileName);
            }
            return node;
        }

        public static XmlNode RemoveSectionNode(string configFileName, string nodeName)
        {
            var path = "//" + nodeName;
            return RemoveXmlNode(configFileName, path);
        }

        public static void AddConfigSectionDefinition(string configFilePath, string groupName, string sectionName, Type sectionType)
        {
            string parentNodeXPath = "//configSections";
            if (!string.IsNullOrEmpty(groupName))
            {
                var groupDefPath = "//sectionGroup[@name='" + groupName + "']";
                parentNodeXPath = groupDefPath;

                var doc = new XmlDocument();
                doc.Load(configFilePath);
                var groupDefNode = doc.SelectSingleNode(groupDefPath);

                if (groupDefNode == null)
                {
                    var groupDefAttr = new Dictionary<string, string>();
                    groupDefAttr["name"] = groupName;
                    AddSectionNode("sectionGroup", groupDefAttr, configFilePath, "//configSections");
                }
            }
            var attributes = new Dictionary<string, string>();
            attributes["name"] = sectionName;
            attributes["type"] = sectionType.AssemblyQualifiedName;

            AddSectionNode("section", attributes,"section[@name = '" + sectionName + "']", configFilePath, parentNodeXPath);
        }

        public static bool AddSectionNode(string nodeName, Dictionary<string, string> attributes, string configFileName, string parentNodeXPath)
        {
            return AddSectionNode(nodeName, attributes, null, configFileName, parentNodeXPath);
        }

        public static bool AddSectionNode(string nodeName, Dictionary<string, string> attributes, string existingXPath, string configFileName, string parentNodeXPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode parentNode = doc.SelectSingleNode(parentNodeXPath);
            if (parentNode != null)
            {
                var newNode = AppendChildNodeIfNotExists(nodeName, existingXPath, parentNode);
                foreach (var attr in attributes)
                {
                    var at = newNode.Attributes[attr.Key];
                    if (at == null)
                    {
                        at = doc.CreateAttribute(attr.Key);
                        newNode.Attributes.Append(at);
                    }
                    at.Value = attr.Value;
                }
                doc.Save(configFileName);
                return true;
            }
            return false;
        }

        public static bool AddSectionNodeWithValue(string nodeName, Dictionary<string, string> attributes, string innerXml, string configFileName, string parentNodeXPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode parentNode = doc.SelectSingleNode(parentNodeXPath);
            if (parentNode != null)
            {
                var newNode = AppendChildNodeIfNotExists(nodeName, null, parentNode);
                foreach (var attr in attributes)
                {
                    var at = newNode.Attributes[attr.Key];
                    if (at == null)
                    {
                        at = doc.CreateAttribute(attr.Key);
                        newNode.Attributes.Append(at);
                    }
                    at.Value = attr.Value;
                }
                newNode.InnerXml = innerXml;
                doc.Save(configFileName);
                return true;
            }
            return false;
        }

        private static XmlNode AppendChildNodeIfNotExists(string nodeName, string existingXPath, XmlNode parentNode)
        {
            XmlNode newNode = null;

            // Search for existing node
            if (!string.IsNullOrEmpty(existingXPath))
                newNode = parentNode.SelectSingleNode(existingXPath);

            // If not found, create a new one
            if (newNode == null)
            {
                newNode = parentNode.OwnerDocument.CreateElement(nodeName);
                parentNode.AppendChild(newNode);
            }
            return newNode;
        }

        private static XmlNode GetConfigNode(string nodePath, ref string configFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilePath);
            XmlNode node = doc.SelectSingleNode(nodePath);

            if (node != null && node.Attributes["configSource"] != null)
            {
                string filePath = node.Attributes["configSource"].Value;
                var configDir = Path.GetDirectoryName(configFilePath);
                configFilePath = configDir + "\\" + filePath;

                doc.Load(configFilePath);
                node = doc.SelectSingleNode(nodePath);
            }
            return node;
        }

        public static bool CopyConfigNode(string parentNodePath, string nodeName, string fromConfigFile, string toConfigFile, MergeOption mergeOption)
        {
            string nodePath = string.Format("{0}/{1}", parentNodePath, nodeName);
            return CopyConfigNode(parentNodePath, nodePath, nodeName, fromConfigFile, toConfigFile, mergeOption);
        }

        public static bool CopyConfigNode(string parentNodePath, string nodePath, string nodeName, string fromConfigFile, string toConfigFile, MergeOption mergeOption)
        {

            XmlNode sourceNode = GetConfigNode(nodePath, ref fromConfigFile);
            XmlNode targetNode = GetConfigNode(nodePath, ref toConfigFile);

            if (targetNode == null)
            {
                var targetNodeParent = GetConfigNode(parentNodePath, ref toConfigFile);
                if (targetNodeParent == null)
                    return false;

                targetNode = targetNodeParent.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, "");
                targetNodeParent.AppendChild(targetNode);
            }
            else if (mergeOption == MergeOption.Ignore)
            {
                return false;
            }

            // Now start working on Inner XML
            if (mergeOption == MergeOption.Overwrite)
            {
                targetNode.InnerXml = sourceNode.InnerXml;
            }
            else
            {
                foreach (XmlNode sourceChildNode in sourceNode.ChildNodes)
                {
                    CopyConfigNode(nodePath, sourceChildNode.Name, fromConfigFile, toConfigFile, mergeOption);
                }
            }

            // Now work on attrinutes
            foreach (XmlAttribute sourceAttribute in sourceNode.Attributes)
            {
                var targetAttribute = targetNode.Attributes[sourceAttribute.Name];
                if (targetAttribute == null)
                {
                    targetAttribute = targetNode.OwnerDocument.CreateAttribute(sourceAttribute.Name);
                    targetNode.Attributes.Append(targetAttribute);
                    targetAttribute.Value = sourceAttribute.Value;
                }
                else if (mergeOption == MergeOption.Overwrite)
                {
                    targetAttribute.Value = sourceAttribute.Value;
                }
            }

            targetNode.OwnerDocument.Save(toConfigFile);
            return true;
        }

        #endregion


        #region Standard sections

        /// <summary>
        /// Sets the connection string value in the given config file
        /// </summary>
        /// <param name="installer"></param>
        /// <param name="configFilePath"></param>
        /// <param name="connectionString"></param>
        public static void SetConnectionStringInConfigFile(string configFilePath, string name, string connectionString)
        {
            var configuration = OpenConfigFile(configFilePath);
            System.Configuration.ConnectionStringsSection sec = configuration.GetSection("connectionStrings") as ConnectionStringsSection;
            if (sec.ConnectionStrings[name] == null)
            {
                sec.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString));
            }
            else
            {
                sec.ConnectionStrings[name].ConnectionString = connectionString;
            }
            configuration.Save();
        }

        public static void SetApplicationSettingInConfigFile(string configFileName, Type settingsType, string settingName, string value)
        {
            SetApplicationSettingInConfigFile(configFileName, settingsType.FullName, settingName, value);
        }

        public static void SetApplicationSettingInConfigFile(string configFileName, string settingsTypeName, string settingName, string value)
        {
            var conf = OpenConfigFile(configFileName);
            var applicationSettings = conf.SectionGroups["applicationSettings"] as ApplicationSettingsGroup;
            if (applicationSettings == null)
            {
                applicationSettings = new ApplicationSettingsGroup();
                conf.SectionGroups.Add("applicationSettings", applicationSettings);
            }
            var configSettings = applicationSettings.Sections[settingsTypeName] as ClientSettingsSection;
            if (configSettings == null)
            {
                configSettings = new ClientSettingsSection();
                applicationSettings.Sections.Add(settingsTypeName, configSettings);
            }
            var settingElement = configSettings.Settings.Get(settingName);
            if (settingElement == null)
            {
                settingElement = new SettingElement(settingName, SettingsSerializeAs.String);
            }
            settingElement.Value.ValueXml = new XmlDocument().CreateNode(XmlNodeType.Element, "value", "");
            settingElement.Value.ValueXml.InnerText = value;
            configSettings.Settings.Add(settingElement);
            conf.Save(ConfigurationSaveMode.Full);
        }

        #endregion

        #region CPR broker related

        public static void ResetDataProviderSectionDefinitions(string configFilePath)
        {
            var doc = new XmlDocument();
            doc.Load(configFilePath);
            var groupDefPath = "//sectionGroup[@name='" + Utilities.Constants.DataProvidersSectionGroupName + "']";
            var groupDefNode = doc.SelectSingleNode(groupDefPath);
            if (groupDefNode != null)
            {
                var children = groupDefNode.ChildNodes.OfType<XmlNode>().ToArray();
                foreach (var ch in children)
                    groupDefNode.RemoveChild(ch);
                doc.Save(configFilePath);
            }

            AddConfigSectionDefinition(configFilePath, Utilities.Constants.DataProvidersSectionGroupName, DataProviderKeysSection.SectionName, typeof(DataProviderKeysSection));
            AddConfigSectionDefinition(configFilePath, Utilities.Constants.DataProvidersSectionGroupName, DataProvidersConfigurationSection.SectionName, typeof(DataProvidersConfigurationSection));
        }

        public static void AddKnownDataProviderTypes(Type[] types, string configFilePath)
        {
            Array.ForEach<Type>(
                types,
                type =>
                {
                    var dic = new Dictionary<string, string>();
                    dic["type"] = type.IdentifyableName();
                    CprBroker.Installers.Installation.AddSectionNode("add", dic, String.Format("add[contains(@type,'{0}')]", type.Name), configFilePath, "//dataProviders/knownTypes");
                }
            );
        }

        #endregion
    }


}
