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
 * Dennis Amdi Skov Isaksen
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

using System;
using System.Collections.Generic;
using System.Linq;
using CprBroker.DAWA;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static PersonAddress ToDpr(this CurrentAddressWrapper currentAddress, DPRDataContext dataContext, PersonInformationType personInformation)
        {
            PersonAddress pa = new PersonAddress
            {
                PNR = Decimal.Parse(currentAddress.CurrentAddressInformation.PNR)
            };

            Dictionary<string, string> currentAddrDict = DawaClient.ConstructAddressDictWithCurrentAddressWrapper(currentAddress);

            string addressServiceResponseJSON = DawaClient.Lookup("adresser", currentAddrDict);

            if (addressServiceResponseJSON != null)
            {
                // If DAWA does return an address we use it for PersonAddress.
                Dictionary<string, string> dawaCurrentAddr = DawaClient.ParseAddressResponse(addressServiceResponseJSON);

                string munCode = dawaCurrentAddr["munCode"];
                if (!string.IsNullOrEmpty(munCode))
                {
                    pa.MunicipalityCode = Convert.ToDecimal(munCode);
                }
                else
                {
                    pa.StreetCode = 0;
                }

                string strCode = dawaCurrentAddr["streetCode"];
                if (!string.IsNullOrEmpty(strCode))
                {
                    pa.StreetCode = Convert.ToDecimal(strCode);
                }
                else
                {
                    pa.StreetCode = 0;
                }

                pa.HouseNumber = dawaCurrentAddr["houseNo"].NullIfEmpty();

                pa.Floor = dawaCurrentAddr["floor"].NullIfEmpty();

                if (new string[] { "th", "tv", "mf" }.Contains(dawaCurrentAddr["door"]))
                    pa.DoorNumber = dawaCurrentAddr["door"].PadLeft(4, ' ');
                else
                    pa.DoorNumber = dawaCurrentAddr["door"].NullIfEmpty(); 

                string PostCode = dawaCurrentAddr["postCode"];
                if (!string.IsNullOrEmpty(PostCode))
                {
                    pa.PostCode = Convert.ToDecimal(PostCode);
                }
                else
                {
                    pa.PostCode = 0;
                }

                pa.MunicipalityName = dawaCurrentAddr["munName"].NullIfEmpty();

                pa.StreetAddressingName = dawaCurrentAddr["addrDescr"].NullIfEmpty();

                pa.Town = dawaCurrentAddr["townName"].NullIfEmpty();
            }
            else
            {
                // If DAWA does not return any address(null) then we fall back to prior logic.

                pa.MunicipalityCode = currentAddress.CurrentAddressInformation.MunicipalityCode;


                pa.StreetCode = currentAddress.CurrentAddressInformation.StreetCode;


                pa.HouseNumber = currentAddress.CurrentAddressInformation.HouseNumber.NullIfEmpty();


                if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.Floor))
                    pa.Floor = currentAddress.CurrentAddressInformation.Floor;
                else
                    pa.Floor = null;


                if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.Door))
                {
                    if (new string[] { "th", "tv", "mf" }.Contains(currentAddress.CurrentAddressInformation.Door))
                        pa.DoorNumber = currentAddress.CurrentAddressInformation.Door.PadLeft(4, ' ');
                    else
                        pa.DoorNumber = currentAddress.CurrentAddressInformation.Door;
                }
                else
                { 
                    pa.DoorNumber = null;
                }

                pa.PostCode = currentAddress.ClearWrittenAddress.PostCode;

                pa.MunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetAuthorityNameByCode(pa.MunicipalityCode.ToString().NullIfEmpty());

                if (!string.IsNullOrEmpty(currentAddress.ClearWrittenAddress.StreetAddressingName))
                    pa.StreetAddressingName = currentAddress.ClearWrittenAddress.StreetAddressingName;
                else
                    pa.StreetAddressingName = Street.GetAddressingName(dataContext.Connection.ConnectionString, currentAddress.CurrentAddressInformation.MunicipalityCode, currentAddress.CurrentAddressInformation.StreetCode);

                if (!string.IsNullOrEmpty(currentAddress.ClearWrittenAddress.CityName))
                    pa.Town = currentAddress.ClearWrittenAddress.CityName;
                else
                    pa.Town = null;

            }
            
            // Below is all data that cannot be retrieved from DAWA.

            if (currentAddress.CurrentAddressInformation.RelocationDate.HasValue)
                pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.RelocationDate.Value, 12);

            if (!string.IsNullOrEmpty(currentAddress.ClearWrittenAddress.BuildingNumber))
                pa.GreenlandConstructionNumber = currentAddress.ClearWrittenAddress.BuildingNumber;
            else
                pa.GreenlandConstructionNumber = null;     

            if (currentAddress.CurrentAddressInformation.RelocationDate.Value != null)
            {
                pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.RelocationDate.Value, 12);
            }
            else
            {
                pa.AddressStartDate = 0;
            }

            if (!char.IsWhiteSpace(currentAddress.CurrentAddressInformation.RelocationDateUncertainty))
                pa.AddressStartDateMarker = currentAddress.CurrentAddressInformation.RelocationDateUncertainty;

            //if (personInformation.Status == 90 /* >= 20*/ && personInformation.StatusStartDate.HasValue)
            //    pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(personInformation.StatusStartDate.Value, 12);
            //else
            pa.AddressEndDate = null; // This is the current date

            pa.LeavingFromMunicipalityCode = null; // To be set later
            pa.LeavingFromMunicipalityDate = null; // To be set later

            if (currentAddress.CurrentAddressInformation.MunicipalityArrivalDate != null)
            {
                pa.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);
            }
            else
            {
                pa.MunicipalityArrivalDate = null;
            }
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;

            if (currentAddress.CurrentAddressInformation.StartDate != null)
            {
                pa.AdditionalAddressDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 12);
            }
            else
            {
                pa.AdditionalAddressDate = null;
            }

            pa.CorrectionMarker = null;

            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.CareOfName))
                pa.CareOfName = currentAddress.CurrentAddressInformation.CareOfName;
            else
                pa.CareOfName = null;

            if (!string.IsNullOrEmpty(currentAddress.ClearWrittenAddress.Location))
                pa.Location = currentAddress.ClearWrittenAddress.Location;
            else
                pa.Location = null;

            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress1))
                pa.AdditionalAddressLine1 = currentAddress.CurrentAddressInformation.SupplementaryAddress1;
            else
                pa.AdditionalAddressLine1 = null;

            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress2))
                pa.AdditionalAddressLine2 = currentAddress.CurrentAddressInformation.SupplementaryAddress2;
            else
                pa.AdditionalAddressLine2 = null;

            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress3))
                pa.AdditionalAddressLine3 = currentAddress.CurrentAddressInformation.SupplementaryAddress3;
            else
                pa.AdditionalAddressLine3 = null;

            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress4))
                pa.AdditionalAddressLine4 = currentAddress.CurrentAddressInformation.SupplementaryAddress4;
            else
                pa.AdditionalAddressLine4 = null;

            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress5))
                pa.AdditionalAddressLine5 = currentAddress.CurrentAddressInformation.SupplementaryAddress5;
            else
                pa.AdditionalAddressLine5 = null;

            return pa;
        }

        // Deprecated due to address data in DPR Emulation database not being updated with new addresses automatically. 
        // The whole reason for the DWA integration.
        //private static bool IsValidAddress(DPRDataContext dataContext, decimal municipalityCode, decimal streetCode, string houseNumber)
        //{
        //    var ret = PostDistrict.GetPostText(dataContext.Connection.ConnectionString, municipalityCode, streetCode, houseNumber);
        //    return ret != null;
        //}

        public static PersonAddress ToDpr(this HistoricalAddressType historicalAddress, DPRDataContext dataContext)
        {
            PersonAddress pa = new PersonAddress
            {
                PNR = Decimal.Parse(historicalAddress.PNR)
            };

            Dictionary<string, string> currentAddrDict = DawaClient.ConstructAddressDictWithHistoricalAddressType(historicalAddress);
            
            string addressServiceResponseJSON = DawaClient.Lookup("adresser", currentAddrDict);

            if (addressServiceResponseJSON != null)
            {
                // If DAWA does return an address we use it to populate the PersonAddress object.

                Dictionary<string, string> dawaCurrentAddr = DawaClient.ParseAddressResponse(addressServiceResponseJSON);

                string strCode = dawaCurrentAddr["streetCode"];
                if (!string.IsNullOrEmpty(strCode))
                {
                    pa.StreetCode = Convert.ToDecimal(strCode);
                }
                else
                {
                    pa.StreetCode = 0;
                }

                if (!string.IsNullOrEmpty(dawaCurrentAddr["munCode"]))
                {
                    pa.PostCode = Convert.ToDecimal(dawaCurrentAddr["munCode"]);
                }
                else
                {
                    pa.PostCode = 0;
                }

                pa.HouseNumber = dawaCurrentAddr["houseNo"].NullIfEmpty();

                pa.Floor = dawaCurrentAddr["floor"].NullIfEmpty();

                if (new string[] { "th", "tv", "mf" }.Contains(dawaCurrentAddr["door"]))
                    pa.DoorNumber = dawaCurrentAddr["door"].PadLeft(4, ' ');
                else
                    pa.DoorNumber = dawaCurrentAddr["door"].NullIfEmpty();

                if (!string.IsNullOrEmpty(dawaCurrentAddr["postCode"]))
                {
                    pa.PostCode = Convert.ToDecimal(dawaCurrentAddr["postCode"]);
                }
                else
                {
                    pa.PostCode = 0;
                }

                pa.MunicipalityName = dawaCurrentAddr["munName"].NullIfEmpty();

                pa.StreetAddressingName = dawaCurrentAddr["addrDescr"].NullIfEmpty();

                pa.Town = dawaCurrentAddr["townName"].NullIfEmpty();
            }
            else
            {
                // If DAWA does not return any address(null) then we fall back to prior logic.

                pa.StreetCode = historicalAddress.StreetCode;

                pa.MunicipalityCode = historicalAddress.MunicipalityCode;

                pa.HouseNumber = historicalAddress.HouseNumber.NullIfEmpty();
                if (!string.IsNullOrEmpty(historicalAddress.Floor))
                    pa.Floor = historicalAddress.Floor;
                else
                    pa.Floor = null;

                if (!string.IsNullOrEmpty(historicalAddress.Door))
                    if (historicalAddress.Door.Equals("th") || historicalAddress.Door.Equals("tv"))
                        pa.DoorNumber = "  " + historicalAddress.Door;
                    else
                        pa.DoorNumber = historicalAddress.Door;
                else
                    pa.DoorNumber = null;

                var postCode = PostDistrict.GetPostCode(dataContext.Connection.ConnectionString, historicalAddress.MunicipalityCode, historicalAddress.StreetCode, historicalAddress.HouseNumber);
                if (postCode.HasValue)
                    pa.PostCode = postCode.Value;


                pa.MunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetAuthorityNameByCode(pa.MunicipalityCode.ToString().NullIfEmpty());

                var streetAdressingName = Street.GetAddressingName(dataContext.Connection.ConnectionString, historicalAddress.MunicipalityCode, historicalAddress.StreetCode);
                if (!string.IsNullOrEmpty(streetAdressingName))
                    pa.StreetAddressingName = streetAdressingName;
                else if (historicalAddress.MunicipalityCode >= 101) // Real municipalites
                    pa.StreetAddressingName = "Adresse ikke komplet";
                else
                    pa.StreetAddressingName = null;

                pa.Town = City.GetCityName(dataContext.Connection.ConnectionString, historicalAddress.MunicipalityCode, historicalAddress.StreetCode, historicalAddress.HouseNumber);
            }

            // Below is all data that cannot be retrieved from DAWA.

            if (historicalAddress.RelocationDate.HasValue)
                pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 12);

            if (!string.IsNullOrEmpty(historicalAddress.BuildingNumber))
                pa.GreenlandConstructionNumber = historicalAddress.BuildingNumber;
            else
                pa.GreenlandConstructionNumber = null;

            // TODO: Shall we use length 12 or 13?
            if (historicalAddress.RelocationDate.HasValue)
                pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 12);
            if (!char.IsWhiteSpace(historicalAddress.RelocationDateUncertainty))
                pa.AddressStartDateMarker = historicalAddress.RelocationDateUncertainty;
            if (historicalAddress.LeavingDate.HasValue)
                pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.LeavingDate.Value, 12);


            pa.LeavingFromMunicipalityCode = null; // To be set later
            pa.LeavingFromMunicipalityDate = null; // To be set later

            pa.MunicipalityArrivalDate = null;  // TODO: Try to fill this field //Seems only available for current address
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            pa.AdditionalAddressDate = null; //TODO: Can be fetched in CPR Services, supladrhaenstart
            if (!char.IsWhiteSpace(historicalAddress.CorrectionMarker))
                pa.CorrectionMarker = historicalAddress.CorrectionMarker;
            if (!string.IsNullOrEmpty(historicalAddress.CareOfName))
                pa.CareOfName = historicalAddress.CareOfName;
            else
                pa.CareOfName = null;

            pa.Location = null; //Find in GoeLookup, based on street code and house number
            pa.AdditionalAddressLine1 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine2 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine3 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine4 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine5 = null; // Seems not available in historical records....
            return pa;
        }

        public static void ClearPreviousAddressData(PersonAddress[] addresses)
        {
            addresses = addresses.Where(a => !a.CorrectionMarker.HasValue).OrderBy(a => a.AddressStartDate).ToArray();
            foreach (var a in addresses)
            {
                a.LeavingFromMunicipalityCode = null;
                a.LeavingFromMunicipalityDate = null;
            }
            var latest = addresses.LastOrDefault();
            if (latest != null)
            {
                var lastWithAnotherMunicipality = addresses.LastOrDefault(a => a.MunicipalityCode != latest.MunicipalityCode);
                if (lastWithAnotherMunicipality != null)
                {
                    latest.LeavingFromMunicipalityCode = lastWithAnotherMunicipality.MunicipalityCode;
                    latest.LeavingFromMunicipalityDate = lastWithAnotherMunicipality.AddressEndDate;
                }
            }

        }

    }
}
