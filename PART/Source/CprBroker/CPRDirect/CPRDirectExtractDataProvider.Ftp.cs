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
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using CprBroker.Engine.Local;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CPRDirectExtractDataProvider
    {

        public string GetFtpUrl()
        {
            return GetFtpUrl(null);
        }

        public string GetFtpUrl(string subPath)
        {
            string url = FtpAddress;

            url = Strings.EnsureStartString(url, "ftp://", true, StringComparison.CurrentCultureIgnoreCase);

            url = Utilities.Strings.EnsureEndString(url, "/", false);

            subPath = string.Format("{0}", subPath);
            subPath = Utilities.Strings.EnsureStartString(subPath, "/", false);
            subPath = Utilities.Strings.EnsureEndString(subPath, "/", false);

            if (!string.IsNullOrEmpty(subPath))
            {
                url += "/" + subPath;
            }
            Admin.LogFormattedSuccess("URL <{0}>", url);
            return url;
        }

        public FtpWebRequest CreateFtpConnection(string subPath)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            FtpWebRequest req = WebRequest.Create(GetFtpUrl(subPath)) as FtpWebRequest;
            req.EnableSsl = true;
            req.Credentials = new NetworkCredential(FtpUser, FtpPassword);
            return req;
        }

        public string[] ListFtpContents()
        {
            var req = CreateFtpConnection(this.FtpOutPath);
            req.Method = WebRequestMethods.Ftp.ListDirectory;

            using (var response = req.GetResponse() as FtpWebResponse)
            {
                using (var rd = new StreamReader(response.GetResponseStream()))
                {
                    string txt = rd.ReadToEnd();
                    var lines = txt.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (!string.IsNullOrEmpty(FtpRegexFilter))
                    {
                        lines = lines.Where(l => Regex.IsMatch(l, FtpRegexFilter))
                            .ToArray();
                    }
                    return lines;
                }
            }
        }

        public string CompanionFilePostfix
        {
            get { return null; }
        }

        public string PutTempFileOnFtp()
        {
            var filePath = CprBroker.Utilities.Strings.NewUniquePath(this.ExtractsFolder, "txt");
            var inf = new FileInfo(filePath);

            byte[] data = Encoding.ASCII.GetBytes("ABC");

            var path = this.FtpOutPath + "/" + inf.Name;
            var req = CreateFtpConnection(path);
            req.Method = WebRequestMethods.Ftp.UploadFile;

            using (var requestStream = req.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }
            using (var resp = req.GetResponse() as FtpWebResponse)
            {
                // Do nothing
            }
            return path;
        }

        public void DeleteFile(string subPath)
        {
            var req = CreateFtpConnection(subPath);
            req.Method = WebRequestMethods.Ftp.DeleteFile;
            using (var resp = req.GetResponse() as FtpWebResponse)
            {
                // Do nothing
            }
        }


        public void DownloadFile(string subPath, string localFileName, long length)
        {
            long readSoFar = 0;
            var request = CreateFtpConnection(subPath);
            using (var resp = request.GetResponse())
            {
                using (var responseStream = resp.GetResponseStream())
                {
                    using (var localFileStream = new FileStream(localFileName, FileMode.CreateNew))
                    {
                        var data = new byte[1024 * 1024];
                        while (readSoFar < length)
                        {
                            var read = responseStream.Read(data, 0, data.Length);
                            readSoFar += read;
                            localFileStream.Write(data, 0, read);
                        }
                    }
                }
            }
        }

        public long GetLength(string subPath)
        {
            var request = CreateFtpConnection(subPath);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            using (var response = request.GetResponse() as FtpWebResponse)
            {
                using (var rd = new StreamReader(response.GetResponseStream()))
                {
                    return response.ContentLength;
                }
            }
        }

        public bool IsDataFile(string localPath)
        {
            // Accept all files
            return true;
        }

        public bool KeepFilesLocally
        {
            get
            {
                return false;
            }
        }
    }
}
