using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DAWA
{
    public class DawaDataProvidersAdresser
    {
        private const string URL = "https://dawa.aws.dk";
        private const string SERVICE = "adresser";

        /* PENDING:
         * try/catch blocks and logging.
         */

        public static Dictionary<string, string> LookupAddress(Dictionary<string, string> inputDict)
        {
            if(ValidateParameterKeysAndValues(inputDict))
            {
                string serviceResponse = CallAddressService(inputDict);
                return ParseResponse(serviceResponse);
            }
            else
            {
                return null;
            }
        }

        public static Dictionary<string, string> ParseResponse(string serviceResponseJSON)
        {
            try
            {
                dynamic responseDict = JsonConvert.DeserializeObject(serviceResponseJSON);

                string postCode = responseDict[0]["adgangsadresse"]["postnummer"]["nr"];
                string munName = responseDict[0]["adgangsadresse"]["kommune"]["navn"];
                string cityName = responseDict[0]["adgangsadresse"]["postnummer"]["navn"];

                Dictionary<string, string> sanitezedDict = new Dictionary<string, string>()
                {
                    {"postCode", postCode},
                    {"munName", munName},
                    {"cityName", cityName}
                };
 
                return sanitezedDict;

            } catch (Exception ex)
            {
                return null;
            }
            
        }

        public static string CallAddressService(Dictionary<string, string> inputDict)
        {
            string url = string.Format("{0}/{1}?", URL, SERVICE);
            string urlParams = DawaDataProvidersAdresser.ConstructUrlParameters(inputDict);
            string requestUrl = string.Format("{0}{1}", url, urlParams);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }

        }

        public static string ConstructUrlParameters(Dictionary<string, string> inputDict)
        {
            string urlParameters = "";
            foreach (var pair in inputDict)
            {
                string param = string.Format("&{0}={1}", pair.Key, pair.Value);
                urlParameters = string.Format("{0}{1}", urlParameters, param);
            }
            return urlParameters.TrimStart('&'); // Trim to remove prepended ampersand.

        }

        public static bool ValidateParameterKeysAndValues(Dictionary<string, string> input)
        {
            // See https://dawa.aws.dk/dok/api/adresse for additional keys not present in the Dictionary.
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
                }
            }
            return (nonValidKeysOrValues == 0 ? true : false);
        }
    }
}
