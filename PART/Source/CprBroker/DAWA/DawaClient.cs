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
 * Heini Leander Ovason
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

using CprBroker.Engine.Local;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CprBroker.DAWA
{
    public class DawaClient
    {
        private const string URL = "https://dawa.aws.dk";

        public static string Lookup(string serviceName, Dictionary<string, string> inputDict)
        {
            if (inputDict != null
                && ValidateParameterKeysAndValues(inputDict)
                && ValidateServiceName(serviceName)
                )
            {
                return CallService(serviceName, inputDict);
            }
            else
            {
                Admin.LogError("DawaClient.Lookup returned NULL");
                //Console.WriteLine("DawaClient.Lookup returned NULL");
                return null;
            }
        }

        public static Dictionary<string, string> ParseAddressResponse(string serviceResponseJSON)
        {
            try
            {
                dynamic responseDict = JsonConvert.DeserializeObject(serviceResponseJSON);

                string postCode = responseDict[0]["adgangsadresse"]["postnummer"]["nr"];
                string munName = responseDict[0]["adgangsadresse"]["kommune"]["navn"];
                string munCode = responseDict[0]["adgangsadresse"]["kommune"]["kode"];
                string townName = responseDict[0]["adgangsadresse"]["postnummer"]["navn"];
                string streetCode = responseDict[0]["adgangsadresse"]["vejstykke"]["kode"];
                string floor = responseDict[0]["etage"];
                string door = responseDict[0]["dør"];
                string houseNo = responseDict[0]["adgangsadresse"]["husnr"];
                string addrDescr = responseDict[0]["adressebetegnelse"];
                string dawaUuid = responseDict[0]["id"];

                Dictionary<string, string> sanitizedDict = new Dictionary<string, string>()
                {
                    {"postCode", postCode},
                    {"munName", munName},
                    {"munCode", munCode},
                    {"townName", townName},
                    {"streetCode", streetCode},
                    {"floor", floor},
                    {"door", door},
                    {"houseNo", houseNo },
                    {"addrDescr", addrDescr},
                    {"dawaUuid", dawaUuid}
                };

                return sanitizedDict;

            }
            catch (Exception ex)
            {
                string svc_resp = string.Format("Response as JSON: {0}", serviceResponseJSON);
                Admin.LogException(ex, svc_resp);
                return null;
            }

        }

        public static Dictionary<string, string> ParseMunicipalResponse(string serviceResponseJSON)
        {
            try
            {
                dynamic responseDict = JsonConvert.DeserializeObject(serviceResponseJSON);

                string munName = responseDict[0]["navn"];
                string regionName = responseDict[0]["region"]["navn"];
                string regionCode = responseDict[0]["region"]["kode"];

                Dictionary<string, string> sanitezedDict = new Dictionary<string, string>()
                {
                    {"munName", munName},
                    {"regionName", regionName},
                    {"regionCode", regionCode}
                };

                return sanitezedDict;

            }
            catch (Exception ex)
            {
                Admin.LogError(ex.ToString());
                //Console.WriteLine(ex.ToString());
                return null;
            }

        }

        private static string CallService(string serviceName, Dictionary<string, string> inputDict)
        {
            string url = string.Format("{0}/{1}?", URL, serviceName);
            string urlParams = DawaClient.ConstructUrlParameters(inputDict);
            string requestUrl = string.Format("{0}{1}", url, urlParams);

            string result = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Admin.LogError(ex.ToString());
                //Console.WriteLine(ex.ToString());
            }
            return result;
        }

        private static string ConstructUrlParameters(Dictionary<string, string> inputDict)
        {
            string urlParameters = "";
            foreach (var pair in inputDict)
            {
                string param = string.Format("&{0}={1}", pair.Key, pair.Value);
                urlParameters = string.Format("{0}{1}", urlParameters, param);
            }
            return urlParameters.TrimStart('&'); // Trim to remove prepended ampersand.

        }

        private static bool ValidateParameterKeysAndValues(Dictionary<string, string> input)
        {
            // See https://dawa.aws.dk/dok/api/adresse for additional keys not present in the Dictionary.
            // enum of keys instead...?
            Dictionary<string, string> paramNamesAndDescriptions = new Dictionary<string, string>()
            {
                {"id", "Adressens unikke id, f.eks. 0a3f5095-45ec-32b8-e044-0003ba298018. (Flerværdisøgning mulig)."},
                { "adgangsadresseid", "Id på den til adressen tilknyttede adgangsadresse. UUID. (Flerværdisøgning mulig)."},
                { "etage", "Etagebetegnelse. Hvis værdi angivet kan den antage følgende værdier: tal fra 1 til 99, st, kl, k2 op til k9. (Flerværdisøgning mulig). Søgning efter ingen værdi mulig."},
                { "dør", "Dørbetegnelse. Tal fra 1 til 9999, små bogstaver samt tegnene / og -. (Flerværdisøgning mulig). Søgning efter ingen værdi mulig."},
                { "vejkode", "Vejkoden. 4 cifre. (Flerværdisøgning mulig)."},
                { "vejnavn", "Vejnavn. Der skelnes mellem store og små bogstaver. (Flerværdisøgning mulig). Søgning efter ingen værdi mulig."},
                { "husnr", "Husnummer. Max 3 cifre eventuelt med et efterfølgende bogstav. (Flerværdisøgning mulig)."},
                { "husnrfra", "Returner kun adresser hvor husnr er større eller lig det angivne."},
                { "husnrtil", "Returner kun adresser hvor husnr er mindre eller lig det angivne. Bemærk, at hvis der angives f.eks. husnrtil=20, så er 20A ikke med i resultatet."},
                { "supplerendebynavn", "Det supplerende bynavn. (Flerværdisøgning mulig). Søgning efter ingen værdi mulig."},
                { "postnr", "Postnummer. 4 cifre. (Flerværdisøgning mulig)."},
                { "kommunekode", "Kommunekoden for den kommune som adressen skal ligge på. 4 cifre. (Flerværdisøgning mulig)."},
                { "struktur", "Angiver om der ønskes en fuld svarstruktur (nestet), en flad svarstruktur (flad) eller en reduceret svarstruktur (mini). Mulige værdier: \"nestet\", \"flad\" eller \"mini\". For JSON er default \"nestet\", og for CSV og GeoJSON er default \"flad\". Det anbefales at benytte mini-formatet hvis der ikke er behov for den fulde struktur, da dette vil give bedre svartider."}
            };

            int nonValidKeysOrValues = 0;
            foreach (var pair in input)
            {
                if (!paramNamesAndDescriptions.ContainsKey(pair.Key) && !string.IsNullOrEmpty(pair.Value))
                {
                    nonValidKeysOrValues++;
                    string logMsg = string.Format("Input not valid. KEY = {0} and Value = {1}", pair.Key, pair.Value);
                    Admin.LogError(logMsg);
                    //Console.WriteLine(logMsg);
                }
            }
            return (nonValidKeysOrValues == 0 ? true : false);
        }

        public static bool ValidateServiceName(string serviceName)
        {
            List<string> validServiceNames = new List<string>() { "adresser", "kommuner" };
            string input = serviceName.ToLower();
            bool result = false;
            foreach (string name in validServiceNames)
            {
                if (input == name)
                {
                    result = true;
                }
            }
            if (!result)
            {
                string logMsg = string.Format("Service name not valid. Input = {0}", serviceName);
                Admin.LogError(logMsg);
                //Console.WriteLine(logMsg);
            }
            return (result == true ? true : false);
        }


        public static Dictionary<string, string> ConstructAddressDictWithCurrentAddressWrapper(CurrentAddressWrapper currentAddressWrapper)
        {
            Dictionary<string, string> addressDict = new Dictionary<string, string>();

            try
            { 
                addressDict.Add("kommunekode", currentAddressWrapper.ClearWrittenAddress.MunicipalityCode.ToString());
                addressDict.Add("vejkode", currentAddressWrapper.ClearWrittenAddress.StreetCode.ToString());

                string houseNo = currentAddressWrapper.ClearWrittenAddress.HouseNumber.ToString();
                if (houseNo.StartsWith("0"))
                {
                    addressDict.Add("husnr", houseNo.TrimStart('0'));
                }
                else
                {
                    addressDict.Add("husnr", houseNo);
                }

                if (!string.IsNullOrEmpty(currentAddressWrapper.ClearWrittenAddress.Floor.ToString()))
                {
                    string etage = currentAddressWrapper.ClearWrittenAddress.Floor.ToString();
                    if(etage.StartsWith("0"))
                    {
                        addressDict.Add("etage", etage.TrimStart('0'));
                    }
                    addressDict.Add("etage", etage);
                }
                if (!string.IsNullOrEmpty(currentAddressWrapper.ClearWrittenAddress.Door.ToString()))
                {
                    string door = currentAddressWrapper.ClearWrittenAddress.Door.ToString();
                    if(door.StartsWith("0"))
                    {
                        addressDict.Add("dør", door.TrimStart('0'));
                    }
                    else
                    {
                        addressDict.Add("dør", door);
                    }
                }
            }
            catch(Exception ex)
            {
                string addressWrapper = string.Format("currentAddressWrapper: {0}", currentAddressWrapper.ToString());
                Admin.LogException(ex, addressWrapper);
            }
            string test = string.Format("currentAddressWrapper dict as JSON: {0}", JsonConvert.SerializeObject(addressDict));
            Admin.LogSuccess(test);
            return addressDict;
        }

    }
}
