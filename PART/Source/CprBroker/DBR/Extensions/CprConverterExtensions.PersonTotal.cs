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
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;
using CprBroker.DAWA;

namespace CprBroker.DBR.Extensions
{
    public partial class CprConverterExtensions
    {

        public static PersonTotal7 ToPersonTotal(this IndividualResponseType resp, DPRDataContext dataContext, char dataRetrievalType = CprBroker.Providers.DPR.DataRetrievalTypes.Extract, char? updatingProgram = null, bool skipAddressIfDead = false)
        {
            var pt = new PersonTotal7();

            #region Status & Gender

            pt.PNR = Decimal.Parse(resp.PersonInformation.PNR);
            if (resp.PersonInformation.StatusStartDate != null)
            {
                pt.StatusDate = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            }
            else
            {
                pt.StatusDate = null;//CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            }
            pt.Status = resp.PersonInformation.Status;

            if (resp.PersonInformation.Birthdate.HasValue)
                pt.DateOfBirth = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.Birthdate.Value, 8);

            pt.Sex = resp.PersonInformation.Gender;
            #endregion

            #region Address decision

            bool putCurrentAddress = true;
            if (skipAddressIfDead == true && resp.PersonInformation.Status == 90)
            {
                putCurrentAddress = false;
            }

            #endregion

            #region Address

            pt.AddressDateMarker = null; // TODO: Fill from address date marker //DPR SPECIFIC            

            // Zero initialization
            pt.MunicipalityArrivalDate = 0;
            pt.MunicipalityLeavingDate = null;

            var adr = resp.GetFolkeregisterAdresseSource(false);
            if (adr != null)
            {
                var schemaAdr = adr.ToAdresseType();
                if (schemaAdr != null)
                {
                    if (schemaAdr.Item is DanskAdresseType || schemaAdr.Item is GroenlandAdresseType)
                    {
                        // KEEP in dead people
                        if (resp.CurrentAddressInformation.MunicipalityCode > 0)
                            pt.CurrentMunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetAuthorityNameByCode(pt.MunicipalityCode.ToString());

                        if (putCurrentAddress)
                        {
                            pt.StandardAddress = resp.ClearWrittenAddress.LabelledAddress.NullIfEmpty();
                            pt.Location = resp.ClearWrittenAddress.Location.NullIfEmpty();

                            pt.MunicipalityCode = resp.CurrentAddressInformation.MunicipalityCode;
                            pt.StreetCode = resp.CurrentAddressInformation.StreetCode;
                            pt.HouseNumber = resp.CurrentAddressInformation.HouseNumber;

                            if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.Floor))
                                pt.Floor = resp.CurrentAddressInformation.Floor;
                            else
                                pt.Floor = null;
                            if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.Door))
                            {
                                if (new string[] { "th", "tv", "mf" }.Contains(resp.CurrentAddressInformation.Door))
                                    pt.Door = resp.CurrentAddressInformation.Door.PadLeft(4, ' ');
                                else
                                    pt.Door = resp.CurrentAddressInformation.Door;
                            }
                            else
                                pt.Door = null;
                            if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.BuildingNumber))
                                pt.ConstructionNumber = resp.CurrentAddressInformation.BuildingNumber;
                            else
                                pt.ConstructionNumber = null;
                            if (resp.CurrentAddressInformation.RelocationDate.HasValue)
                                pt.AddressDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.RelocationDate.Value, 12);
                            if (resp.CurrentAddressInformation.MunicipalityArrivalDate.HasValue)
                                pt.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);

                            if (resp.CurrentAddressInformation.LeavingMunicipalityDepartureDate.HasValue)
                            {
                                pt.MunicipalityLeavingDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.LeavingMunicipalityDepartureDate.Value, 12);
                            }

                            if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.CareOfName))
                                pt.CareOfName = resp.CurrentAddressInformation.CareOfName;
                            else
                                pt.CareOfName = null;
                            if (!string.IsNullOrEmpty(resp.ClearWrittenAddress.CityName))
                                pt.CityName = resp.ClearWrittenAddress.CityName;
                            else
                                pt.CityName = null;
                        }
                    }
                }
            }
            #endregion

            #region Post code & district
            // Post code & text could be empty for dead people
            if (putCurrentAddress)
            {
                // TODO: this can be empty string in source data - handle this case
                pt.PostCode = resp.ClearWrittenAddress.PostCode;
                pt.PostDistrictName = resp.ClearWrittenAddress.PostDistrictText.NullIfEmpty();
            }
            #endregion

            #region Protection

            Func<ProtectionType.ProtectionCategoryCodes, char?> protectionMarkerGetter = (cd) =>
                resp.Protection
                .Where(p => p.ProtectionCategoryCode == cd)
                .Count()
                > 0 ? '1' : null as char?;

            pt.AddressProtectionMarker = protectionMarkerGetter(ProtectionType.ProtectionCategoryCodes.NameAndAddress);
            pt.DirectoryProtectionMarker = protectionMarkerGetter(ProtectionType.ProtectionCategoryCodes.LocalDirectory);
            #endregion

            #region Birth registration

            if (!string.IsNullOrEmpty(resp.BirthRegistrationInformation.BirthRegistrationAuthorityCode))
                pt.BirthPlaceOfRegistration = Authority.GetAuthorityNameByCode(resp.BirthRegistrationInformation.BirthRegistrationAuthorityCode);
            else
                pt.BirthPlaceOfRegistration = null;

            pt.BirthplaceText = resp.BirthRegistrationInformation.AdditionalBirthRegistrationText;

            #endregion

            pt.PnrMarkingDate = null; //TODO: Can be fetched in CPR Services: pnrhaenstart

            #region Parents
            Func<string, DateTime?, string> parentPnrOrBirthdateGetter = (parentPnr, parentBirthdate) =>
            {
                if (string.IsNullOrEmpty(parentPnr) || parentPnr.Equals("0000000000"))
                {
                    if (parentBirthdate.HasValue)
                    {
                        return parentBirthdate.Value.ToString("dd MM yyyy");
                    }
                    else
                    {
                        return "000000-0000";
                    }
                }
                else
                {
                    return parentPnr.Substring(0, 6) + "-" + parentPnr.Substring(6, 4);
                }
            };

            Func<string, char> parentPnrMarkerGetter = (string parentPnr) =>
            {
                if (string.IsNullOrEmpty(parentPnr) || parentPnr.Equals("0000000000"))
                    return '*';
                else
                    return ' ';
            };


            pt.MotherPersonalOrBirthDate = parentPnrOrBirthdateGetter(resp.ParentsInformation.MotherPNR, resp.ParentsInformation.MotherBirthDate);
            pt.MotherMarker = parentPnrMarkerGetter(resp.ParentsInformation.MotherPNR);
            pt.FatherPersonalOrBirthdate = parentPnrOrBirthdateGetter(resp.ParentsInformation.FatherPNR, resp.ParentsInformation.FatherBirthDate);
            pt.FatherMarker = parentPnrMarkerGetter(resp.ParentsInformation.FatherPNR);

            if (resp.ParentsInformation.FatherDate.HasValue)
                pt.PaternityDate = CprBroker.Utilities.Dates.DateToDecimal(resp.ParentsInformation.FatherDate.Value, 12);

            #endregion

            #region Guardian
            if (resp.Disempowerment != null && resp.Disempowerment.DisempowermentStartDate.HasValue)
            {
                pt.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(resp.Disempowerment.DisempowermentStartDate.Value, 8);
            }
            else
            {
                pt.UnderGuardianshipDate = null;
            }
            #endregion

            #region Marriage, spouse & children

            pt.MaritalStatus = resp.CurrentCivilStatus.CivilStatusCode;

            pt.MaritalStatusDate = resp.CurrentCivilStatus.CivilStatusStartDateDecimal;

            if (resp.CurrentCivilStatus.SpouseBirthDate.HasValue)
            {
                pt.SpousePersonalOrBirthdate = resp.CurrentCivilStatus.SpouseBirthDate.Value.ToString("dd-MM-yyyy");
            }
            else if (string.Format("{0}", resp.CurrentCivilStatus.SpousePNR).Trim().Length > 1)
            {
                pt.SpousePersonalOrBirthdate = resp.CurrentCivilStatus.SpousePNR.Substring(0, 6) + "-" + resp.CurrentCivilStatus.SpousePNR.Substring(6, 4);
            }
            pt.SpouseMarker = null; // Unavailable in CPR Extracts
            pt.MaritalAuthorityName = null; //TODO: Retrieve this from the CPR Service field mynkod

            #endregion

            #region Voting
            var voting = resp.ElectionInformation.OrderByDescending(e => e.ElectionInfoStartDate).FirstOrDefault();

            if (voting != null && voting.VotingDate.HasValue)
                pt.VotingDate = CprBroker.Utilities.Dates.DateToDecimal(voting.VotingDate.Value, 8);
            else
                pt.VotingDate = null;
            #endregion

            #region Markers
            if (
                resp.CurrentDepartureData != null
                || resp.HistoricalDeparture.Count() > 0
                )
                pt.ExitEntryMarker = '1';

            pt.ChristianMark = resp.ChurchInformation.ChurchRelationship;

            if (resp.CurrentDisappearanceInformation != null
                || resp.HistoricalDisappearance.Count() > 0
                )
                pt.DisappearedMarker = '1';

            pt.ChildMarker = resp.Child.Count > 0 ?
                '1' : null as char?;

            if (resp.CurrentAddressInformation != null)
            {
                if (
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress1) ||
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress2) ||
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress3) ||
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress4) ||
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress5)
                    )
                    pt.SupplementaryAddressMarker = '1'; //DPR SPECIFIC
            }
            pt.MunicipalRelationMarker = resp.MunicipalConditions.Count() > 0 ? '1' : null as char?;

            pt.NationalMemoMarker = resp.Notes.Count() > 0 ? '1' : null as char?;
            pt.FormerPersonalMarker = resp.HistoricalPNR.Count > 0 ? '1' : null as char?;
            pt.ContactAddressMarker = resp.ContactAddress == null ?
                null as char? : '1';

            pt.PaternityAuthorityName = null; //TODO: Retrieve this from the CPR Service field far_mynkod
            #endregion

            #region Job & Nationality

            if (!string.IsNullOrEmpty(resp.PersonInformation.Job))
                pt.Occupation = resp.PersonInformation.Job;
            else
                pt.Occupation = null;
            pt.NationalityRight = Authority.GetAuthorityNameByCode(resp.CurrentCitizenship.CountryCode.ToString());
            #endregion

            #region Previous address & municipality

            if (resp.CurrentAddressInformation?.LeavingMunicipalityCode > 0)// Leave it null otherwise
            {
                pt.PreviousMunicipalityName = Authority.GetAuthorityNameByCode(
                    string.Format("{0}", resp.CurrentAddressInformation?.LeavingMunicipalityCode));
            }

            var previousAddresses = resp.HistoricalAddress
                .Where(e => (e as CprBroker.Schemas.Part.IHasCorrectionMarker).CorrectionMarker == CprBroker.Schemas.Part.CorrectionMarker.OK)
                .OrderByDescending(e => e.RelocationDate);

            var prevAddress = previousAddresses.FirstOrDefault();
            if (prevAddress != null)
            {
                if (resp.CurrentAddressInformation == null)
                {
                    pt.CareOfName = prevAddress.CareOfName;
                }

                if (resp.PersonInformation.Status == 90 && string.IsNullOrEmpty(pt.CurrentMunicipalityName))
                {
                    pt.CurrentMunicipalityName = Authority.GetAuthorityNameByCode(prevAddress.MunicipalityCode.ToString());
                }

                if (string.IsNullOrEmpty(pt.CurrentMunicipalityName))
                    pt.CurrentMunicipalityName = Authority.GetAuthorityNameByCode(prevAddress.MunicipalityCode.ToString());
            }

            pt.PreviousAddress = resp.ToPreviousAddressString(dataContext);

            if (resp.CurrentAddressInformation != null && resp.CurrentAddressInformation.RelocationDate.HasValue)
            {
                var previousDeparture = resp.HistoricalDeparture
                    .Where(d =>
                        d.IsOk()
                        && d.EntryDate.HasValue
                        && d.EntryDate.Value.Date.Equals(resp.CurrentAddressInformation.RelocationDate.Value.Date)
                        )
                    .SingleOrDefault();
                if (previousDeparture != null)
                {
                    if (string.IsNullOrEmpty(pt.PreviousAddress))
                    {
                        pt.PreviousAddress = string.Format(
                            "Personen er indrejst {0} fra {1}",
                            previousDeparture.EntryDate.Value.ToString("dd MM yyyy"),
                            Authority.GetAuthorityNameByCode(previousDeparture.EntryCountryCode.ToString())
                            );
                    }
                }
            }
            #endregion

            #region Names

            // In DPR SearchName contains both the first name and the middlename.
            pt.SearchName = ToDprFirstName(resp.CurrentNameInformation.FirstName_s, resp.CurrentNameInformation.MiddleName, true);
            if (!string.IsNullOrEmpty(resp.CurrentNameInformation.LastName))
                pt.SearchSurname = resp.CurrentNameInformation.LastName.ToUpper();
            else
                pt.SearchSurname = null;

            // Special logic for addressing name
            pt.AddressingName = ToDprAddressingName(resp.ClearWrittenAddress.AddressingName, resp.CurrentNameInformation.LastName);
            #endregion

            #region Update markers
            pt.DprLoadDate = DateTime.Now;
            pt.ApplicationCode = updatingProgram;
            pt.DataRetrievalType = dataRetrievalType; // TODO: Use other types for deleted subscriptions and loaded from CPR direct)
            #endregion

            return pt;
        }

        public static string ToPreviousAddressString(this IndividualResponseType resp, DPRDataContext dataContext)
        {
            IAddressSource prevAddress = null;

            if (resp.PersonInformation.Status == 90)
            {
                prevAddress = resp.GetFolkeregisterAdresseSource(false);
            }
            if (prevAddress == null)
            {
                List<IAddressSource> addresses = new List<IAddressSource>();
                addresses.AddRange(resp.HistoricalAddress);
                addresses.AddRange(resp.HistoricalDeparture);
                addresses.AddRange(resp.HistoricalDisappearance);
                prevAddress = addresses
                    .Where(a =>
                        !(a is IHasCorrectionMarker)
                            ||
                        (a as IHasCorrectionMarker).IsOk()
                        )
                .OrderByDescending(a => a.ToEndTS())
                .FirstOrDefault();
            }

            if (prevAddress is CurrentAddressWrapper)
            {
                var currAdr = (prevAddress as CurrentAddressWrapper).ClearWrittenAddress;
                return resp.ToPreviousAddressString(dataContext, currAdr.MunicipalityCode, currAdr.StreetCode, currAdr.HouseNumber, currAdr.Floor, currAdr.Door, currAdr.BuildingNumber);
            }
            else if (prevAddress is HistoricalAddressType)
            {
                var histAdr = prevAddress as HistoricalAddressType;
                return resp.ToPreviousAddressString(dataContext, histAdr.MunicipalityCode, histAdr.StreetCode, histAdr.HouseNumber, histAdr.Floor, histAdr.Door, histAdr.BuildingNumber);
            }
            else if (prevAddress is CurrentDepartureDataType)
            {
                return null; // Case is not possible
                //var dep = prevAddress as CurrentDepartureDataType;
                //return ToPreviousAddressString_Departure(dep.ExitDate, dep.EntryDate, dep.EntryCountryCode);
            }
            else if (prevAddress is HistoricalDepartureType)
            {
                var dep = prevAddress as HistoricalDepartureType;
                return ToPreviousAddressString_Departure(dep.ExitDate, dep.EntryDate, dep.EntryCountryCode);
            }
            else if (prevAddress is CurrentDisappearanceInformationType)
            {
                return null; // Case is not possible
                //var dis = prevAddress as CurrentDisappearanceInformationType;
                //return ToPreviousAddressString_Disappearance(dis.DisappearanceDate, dis.RetrievalDate);
            }
            else if (prevAddress is HistoricalDisappearanceType)
            {
                var dis = prevAddress as HistoricalDisappearanceType;
                return ToPreviousAddressString_Disappearance(dis.DisappearanceDate, dis.RetrievalDate);
            }
            // No previous address - return null
            return null;
        }


        private static string ToPreviousAddressString(this IndividualResponseType resp, DPRDataContext dataContext, decimal municipalityCode, decimal streetCode, string houseNumber, string floor, string door, string bnrNotUsed = null)
        {
            // If it is a valid address
            // Alternatively, Street.GetAddressingName(dataContext.Connection.ConnectionString, historicalAddress.MunicipalityCode, historicalAddress.StreetCode) != null)
            if (houseNumber.Trim() != "")
            {
                var prevAdrStr = string.Format("{0} {1}",
                        Street.GetAddressingName(dataContext.Connection.ConnectionString, municipalityCode, streetCode),
                        System.Text.RegularExpressions.Regex.Replace(
                            houseNumber.TrimStart('0', ' '),
                            "(?<num>\\d+)(?<char>[a-zA-Z]+)",
                            "${num} ${char}"));

                var floorDoor = string.Format("{0} {1}",
                    floor.TrimStart('0', ' '),
                    door.TrimStart('0', ' '))
                 .Trim();

                if (!string.IsNullOrEmpty(floorDoor))
                    prevAdrStr += "," + floorDoor;

                var kom = Authority.GetAuthorityNameByCode(municipalityCode.ToString());
                if (!string.IsNullOrEmpty(kom))
                    prevAdrStr += string.Format("  ({0} kommune)", kom);

                return prevAdrStr;
            }
            return null;
        }

        private static string ToPreviousAddressString_Departure(DateTime? exitDate, DateTime? entryDate, decimal entryCountryCode)
        {
            if (exitDate.HasValue)
            {
                return string.Format("Personen har været udrejst fra {0} TIL {1}",
                    exitDate?.ToString("dd MM yyyy"),
                    entryDate?.ToString("dd MM yyyy"));
            }
            else
            {
                return string.Format("Personen er indrejst {0} fra {1}",
                    entryDate?.ToString("dd MM yyyy"),
                    Authority.GetAuthorityNameByCode(entryCountryCode.ToString()));
            }
        }

        private static string ToPreviousAddressString_Disappearance(DateTime? disappearanceDate, DateTime? retrievalDate)
        {
            return string.Format("Personen har været forsvundet fra {0} til {1}",
                    disappearanceDate?.ToString("dd MM yyyy"),
                    retrievalDate?.ToString("dd MM yyyy")
                    );
        }
    }
}
