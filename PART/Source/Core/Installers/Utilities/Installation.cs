﻿/* ***** BEGIN LICENSE BLOCK *****
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
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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

namespace CprBroker.Installers
{
    /// <summary>
    /// Utility class with methods that assist the installation process by extending the Installer class
    /// </summary>
    public static class Installation
    {
        /// <summary>
        /// Returns the full path of the currently executing exe installer
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetInstallerAssemblyFilePath(this Installer installer)
        {
            return installer.Context.Parameters["assemblypath"]; ;
        }

        /// <summary>
        /// Returns the full path of the config file related to the current exe
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetInstallerAssemblyConfigFilePath(this Installer installer)
        {
            return installer.GetInstallerAssemblyFilePath() + ".config";
        }

        /// <summary>
        /// Returns the full path of the folder containing the current exe
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetInstallerAssemblyFolderPath(this Installer installer)
        {
            string exePath = installer.GetInstallerAssemblyFilePath();
            FileInfo fileInfo = new FileInfo(exePath);
            return fileInfo.Directory.FullName;
        }

        public static string GetWebFolderPath(this Installer installer)
        {
            var installerAssemblyDir = new DirectoryInfo(installer.GetInstallerAssemblyFolderPath());
            return installerAssemblyDir.Parent.FullName;
        }
        /// <summary>
        /// Gets the full path of the web config of the file that contains the current exe installer
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetWebConfigFilePathFromInstaller(this Installer installer)
        {
            var webDir = new DirectoryInfo(installer.GetWebFolderPath());
            string configFileName = webDir + "\\web.config";
            return configFileName;
        }

        public static Configuration OpenConfigFile(string configFilePath)
        {
            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configFilePath;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            return configuration;
        }

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

        public static XmlNode RemoveSectionNode(string configFileName, string nodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode node = doc.SelectSingleNode("//" + nodeName);
            if (node != null)
            {
                var parentNode = node.ParentNode;
                parentNode.RemoveChild(node);
                doc.Save(configFileName);
            }
            return node;
        }

        public static bool AddSectionNode(string nodeName, Dictionary<string, string> attributes, string configFileName, string parentNodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode parentNode = doc.SelectSingleNode("//" + parentNodeName);
            if (parentNode != null)
            {
                var newNode = doc.CreateElement(nodeName);
                foreach (var attr in attributes)
                {
                    var at = doc.CreateAttribute(attr.Key);
                    at.Value = attr.Value;
                    newNode.Attributes.Append(at);
                }
                parentNode.AppendChild(newNode);
                doc.Save(configFileName);
                return true;
            }
            return false;
        }
        public static bool AddSectionNode(XmlNode node, string configFileName, string parentNodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode parentNode = doc.SelectSingleNode("//" + parentNodeName);
            if (parentNode != null)
            {
                var newNode = doc.CreateElement(node.Name);
                newNode.InnerXml = node.InnerXml;
                parentNode.AppendChild(newNode);
                doc.Save(configFileName);
                return true;
            }
            return false;
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

        private static XmlNode GetConfigNode(string nodePath, ref string configFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilePath);
            XmlNode node = doc.SelectSingleNode(nodePath);

            if (node.Attributes["configSource"] != null)
            {
                string filePath = node.Attributes["configSource"].Value;
                var configDir = Path.GetDirectoryName(configFilePath);
                configFilePath = configDir + "\\" + filePath;

                doc.Load(configFilePath);
                node = doc.SelectSingleNode(nodePath);
            }
            return node;
        }

        public static void SetFlatFileLogListenerAccessRights(string configFileName)
        {
            XmlNode loggingConfigurationNode = GetConfigNode("//loggingConfiguration", ref configFileName);

            var listenersNode = loggingConfigurationNode.SelectSingleNode("listeners");
            var flatFileNode = listenersNode.SelectSingleNode("add[@name='FlatFile']");
            string fileName = flatFileNode.Attributes["fileName"].Value;

            if (!File.Exists(fileName))
            {
                string directoryName = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                File.Create(fileName);
            }
            FileSecurity access = System.IO.File.GetAccessControl(fileName);
            FileSystemAccessRule rule = new FileSystemAccessRule("NETWORK SERVICE", FileSystemRights.FullControl, AccessControlType.Allow);
            access.ResetAccessRule(rule);
            File.SetAccessControl(fileName, access);
        }

        public static void CopyConfigNode(string nodePath, string fromConfigFile, string toConfigFile)
        {
            XmlNode sourceNode = GetConfigNode(nodePath, ref fromConfigFile);
            XmlNode targetNode = GetConfigNode(nodePath, ref toConfigFile);

            targetNode.Attributes.RemoveAll();
            foreach (XmlAttribute sourceAttribute in sourceNode.Attributes)
            {
                var targetAttribute = targetNode.OwnerDocument.CreateAttribute(sourceAttribute.Name);
                targetAttribute.Value = sourceAttribute.Value;
                targetNode.Attributes.Append(targetAttribute);
            }
            targetNode.InnerXml = sourceNode.InnerXml;

            targetNode.OwnerDocument.Save(toConfigFile);
        }

        public static string GetNetFrameworkDirectory(Version frameworkVersion)
        {
            var targetFrameworkDir = new DirectoryInfo(GetNetFrameworkDirectory()).Parent.GetDirectories(string.Format("v{0}*", frameworkVersion.ToString(2))).FirstOrDefault();
            return CprBroker.Utilities.Strings.EnsureDirectoryEndSlash(CprBroker.Utilities.Strings.EnsureDirectoryEndSlash(targetFrameworkDir.FullName));
        }

        const int MAX_PATH = 256;
        private static string GetNetFrameworkDirectory()
        {
            StringBuilder buf = new StringBuilder(
                MAX_PATH, MAX_PATH);
            int cch = MAX_PATH;
            int hr = GetCORSystemDirectory(
                buf, MAX_PATH, ref cch);
            if (hr < 0) Marshal.ThrowExceptionForHR(hr);
            return buf.ToString();
        }

        [DllImport("mscoree.dll",
         CharSet = CharSet.Unicode,
         ExactSpelling = true)]
        private static extern int GetCORSystemDirectory(
                StringBuilder buf,
                int cchBuf,
                ref int cchRequired);

        public static WindowHandleWrapper InstallerWindowWrapper(this Installer installer)
        {
            return new WindowHandleWrapper(installer.Context.Parameters["productName"]);
        }

        public static WindowHandleWrapper InstallerWindowWrapper(this Microsoft.Deployment.WindowsInstaller.Session session)
        {
            var title = session.GetPropertyValue("ProductName");
            var ret = new WindowHandleWrapper(title + " Setup");
            if (ret.Handle == IntPtr.Zero)
            {
                ret = new WindowHandleWrapper(title);
            }
            return ret;
        }

        public static bool IsInDeferredMode(this Microsoft.Deployment.WindowsInstaller.Session session)
        {
            return session.GetMode(Microsoft.Deployment.WindowsInstaller.InstallRunMode.Scheduled) || session.GetMode(Microsoft.Deployment.WindowsInstaller.InstallRunMode.Rollback);
        }

        public static string GetPropertyValue(this Microsoft.Deployment.WindowsInstaller.Session session, string propName)
        {
            return GetPropertyValue(session, propName, "");
        }

        public static string GetPropertyValue(this Microsoft.Deployment.WindowsInstaller.Session session, string propName, string featureName)
        {
            if (!string.IsNullOrEmpty(featureName))
                propName = string.Format("{0}_{1}", propName, featureName);

            if (session.IsInDeferredMode())
            {
                return session.CustomActionData[propName];
            }
            else
            {
                return session[propName];
            }
        }

        public static bool GetBooleanPropertyValue(this Microsoft.Deployment.WindowsInstaller.Session session, string propName)
        {
            return GetBooleanPropertyValue(session, propName, "");
        }

        public static bool GetBooleanPropertyValue(this Microsoft.Deployment.WindowsInstaller.Session session, string propName, string featureName)
        {
            var stringValue = GetPropertyValue(session, propName, featureName);
            bool ret;
            bool.TryParse(stringValue, out ret);
            return ret;
        }

        public static void SetPropertyValue(this Microsoft.Deployment.WindowsInstaller.Session session, string propName, string propValue)
        {
            if (session.IsInDeferredMode())
            {
                session.CustomActionData[propName] = propValue;
            }
            else
            {
                session[propName] = propValue;
            }
        }

        public static string GetInstallDirProperty(this Microsoft.Deployment.WindowsInstaller.Session session)
        {
            return CprBroker.Utilities.Strings.EnsureDirectoryEndSlash(session.GetPropertyValue("INSTALLDIR"));
        }

        public static void ShowErrorMessage(this Microsoft.Deployment.WindowsInstaller.Session session, Exception ex)
        {
            Microsoft.Deployment.WindowsInstaller.Record record = new Microsoft.Deployment.WindowsInstaller.Record();
            record.FormatString = ex.ToString();
            session.Message(
                Microsoft.Deployment.WindowsInstaller.InstallMessage.Error | (Microsoft.Deployment.WindowsInstaller.InstallMessage)System.Windows.Forms.MessageBoxButtons.OK | (Microsoft.Deployment.WindowsInstaller.InstallMessage)System.Windows.Forms.MessageBoxIcon.Error,
                record);
        }

        public static InstallUILevel UiLevel(this Microsoft.Deployment.WindowsInstaller.Session session)
        {
            return (InstallUILevel)int.Parse(session.GetPropertyValue("UILevel"));
        }

        public static void RunCommand(string fileName, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;

            startInfo.Arguments = args;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new InstallException(string.Format("Process '{0} {1}' failed", startInfo.FileName, startInfo.Arguments));
            }
        }
    }


}
