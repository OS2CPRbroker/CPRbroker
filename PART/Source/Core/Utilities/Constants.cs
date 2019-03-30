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

namespace CprBroker.Utilities
{
    /// <summary>
    /// Contains all the constants used by the broker
    /// </summary>
    public static class Constants
    {

        public static readonly DateTime MinSqlDate = new DateTime(1753, 1, 1);
        public static readonly DateTime MaxSqlDate = new DateTime(9999, 12, 31);

        /// <summary>
        /// Token of preapproved base application that comes with a first installation
        /// </summary>
        public static readonly Guid BaseApplicationId = new Guid("3E9890FF-0038-42A4-987A-99B63E8BC865");
        public static readonly Guid BaseApplicationToken = new Guid("07059250-E448-4040-B695-9C03F9E59E38");
        public static readonly string UserToken = "";

        public static readonly Guid FakeActorId = new Guid("286548AA-E177-4EEB-9C5A-ABFF0D30F453");

        public static class Logging
        {
            public static readonly string Category = "General";
            public static readonly string ApplicationId = "ApplicationId";
            public static readonly string ActivityId = "ActivityId";
            public static readonly string ApplicationToken = "ApplicationToken";
            public static readonly string ApplicationName = "ApplicationName";
            public static readonly string UserToken = "UserToken";
            public static readonly string UserName = "UserName";
            public static readonly string MethodName = "MethodName";
            public static readonly string DataObjectType = "DataObjectType";
            public static readonly string DataObjectXml = "DataObjectXml";
        }

        public sealed class Versioning
        {
            public static readonly int Major = 1;
            public static readonly int Minor = 0;

            // TODO: Set version manually in Config project
            public const string AssemblyVersion = "2.3.4.0";
            public const string AssemblyFileVersion = "2.3.4.0";
        }

        public static readonly Guid EventBrokerApplicationId = new Guid("{C98F9BE7-2DDE-404a-BAB5-5A7B1BBC3063}");
        public static readonly Guid EventBrokerApplicationToken = new Guid("{FCD568A0-8F18-4b6f-8691-C09239F158F3}");

        public const string DataProvidersSectionGroupName = "dataProvidersGroup";

    }
}
