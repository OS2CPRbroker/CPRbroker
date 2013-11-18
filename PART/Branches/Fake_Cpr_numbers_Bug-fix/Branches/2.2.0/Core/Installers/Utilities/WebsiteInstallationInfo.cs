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
using System.DirectoryServices;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    public class WebsiteInstallationInfo : WebInstallationInfo
    {
        public static readonly string AppPoolsRoot = ServerRoot + "/APPPOOLS";
        public override bool TargetEntryExists
        {
            get
            {
                DirectoryEntry machineRoot = new DirectoryEntry(ServerRoot);
                foreach (DirectoryEntry e in machineRoot.Children)
                {
                    if (
                            e.SchemaClassName == "IIsWebServer"
                            && e.Properties["ServerComment"].Value.ToString().ToLower() == WebsiteName.ToLower()
                       )
                        return true;
                }
                return false;
            }
        }

        public override string TargetWmiPath
        {
            get
            {
                return ServerRoot + "/" + GetSiteId();
            }
        }

        public override string TargetWmiSubPath
        {
            get
            {
                return "w3svc/" + GetSiteId();
            }
        }

        public string AppPoolWmiPath
        {
            get
            {
                return string.Format("{0}/{1}", AppPoolsRoot, WebsiteName);
            }
        }

        public override string GetAppRelativePath()
        {
            return "/";
        }


        public override string[] CalculateWebUrls()
        {
            using (DirectoryEntry ent = new DirectoryEntry(this.TargetWmiPath))
            {
                string[] hostHeaders = GetSiteHostHeaders(ent);
                return hostHeaders.Select(hh => string.Format("http://{0}/", hh)).ToArray();
            }
        }

        protected override bool IsMatchingDirectoryEntry(DirectoryEntry e)
        {
            return e.Properties["ServerComment"].Value.ToString().ToLower() == WebsiteName.ToLower();
        }

        public override bool Validate(out string message)
        {
            message = null;
            if (!Strings.IsValidHostName(this.WebsiteName))
            {
                message = string.Format("Invalid website name: '{0}'", WebsiteName);
                return false;
            }
            return true;
        }

    }
}