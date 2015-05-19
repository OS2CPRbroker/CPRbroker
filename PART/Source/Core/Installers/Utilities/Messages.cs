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
using System.Windows.Forms;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    /// <summary>
    /// Contains text messages displayed by the application
    /// </summary>
    public class Messages
    {
        public static readonly string WebsiteExists = "Website already exists. Do you want to overwrite it?";
        public static readonly string WebAppExists = "Web application already exists. Do you want to overwrite it?";
        public static readonly string MissingIISComponents = "IIS Metabase and IIS6 Configuration Compatibility are missing from your IIS installation. Please make sure to install all the necessary components and try the installation again";


        public static readonly string Required = "Required";
        public static readonly string MaxLength = "Maximum allowed length is ";
        public static readonly string InvalidInput = "Invalid input";

        public static readonly string Succeeded = "Succeeded";
        public static readonly string Unsuccessful = "Unsuccessful";


        public static readonly string AdminConnectionFailed = "Admin connection failed";
        public static readonly string ApplicationConnectionFailedCheckUserNameAndPassword = "Application connection failed. Please check the user name and password";

        public static readonly string DatabaseAlreadyExistsDoYouWantToUseExisting = "The database already exists, would you like to use the existing database?";
        public static readonly string DatabaseAlreadyExists = "The database already exists";
        public static readonly string DoYouWantToDropDatabase = "Do you want to drop the system's database?";
        public static readonly string DatabaseDoesNotExist = "The database does not exist. Please check the database name";

        public static readonly string AdminConnectionHasInsufficientRights = "Admin connection does not have sufficient rights to create a database";

        public static readonly string WindowsAuthenticationNotAllowed = "Windows authentication is not allowed for application login";
        public static readonly string WindowsAuthenticationContactAdmin = "The web site and backend service are configured to use Windows authentication to connect to the database. \r\nYou may need to contact your database administrator to add a new user for the windows identity used by the web site and backend service";
        public static readonly string SqlAuthenticationNotAllowedOnServer = "SQL Server authentication is not allowed on the server";

        public static readonly string CancelSetup = "Cancel setup";
        public static readonly string AreYouSureYouWantToCancel = "Are you sure you want to cancel setup?";

        public static readonly string AnErrorHasOccurredAndInstallationWillBeCancelled = "An error has occurred. Installation will be cancelled";
        public static readonly string AnErrorHasOccurredAndItWillBeIgnored = "An error has occurred. The process will ignore it and continue.";

        public static void ShowException(System.Configuration.Install.Installer installer, string message, Exception ex)
        {
            message = string.Format("{0}\r\n{1}\r\n",
                AnErrorHasOccurredAndItWillBeIgnored,
                message,
                ex.ToString()
                );
            MessageBox.Show(Installation.InstallerWindowWrapper(installer), message);
        }
    }
}
