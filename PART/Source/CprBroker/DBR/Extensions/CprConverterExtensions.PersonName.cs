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

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static PersonName ToDpr(this CurrentNameInformationType currentName, PersonInformationType personInformation)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(currentName.PNR);
            pn.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentName.Registration.RegistrationDate, 12);
            pn.NameAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod
            pn.Status = personInformation.Status;
            if (personInformation.StatusStartDate != null)
            {
                pn.StatusDate = CprBroker.Utilities.Dates.DateToDecimal(personInformation.StatusStartDate.Value, 12);
            }
            else
            {
                pn.StatusDate = null;//CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            }
            if (!char.IsWhiteSpace(currentName.FirstNameMarker))
                pn.FirstNameMarker = currentName.FirstNameMarker;
            if (!char.IsWhiteSpace(currentName.LastNameMarker))
                pn.SurnameMarker = currentName.LastNameMarker;

            pn.NameStartDate = currentName.NameStartDateDecimal;

            pn.NameTerminationDate = null; //This is the current name
            pn.AddressingNameDate = null; //TODO: Can be fetched in CPR Services, adrnvnhaenstart
            pn.CorrectionMarker = null; // This is the current name
            pn.AddressingNameReportingMarker = null; //TODO: Can be fetched in CPR Services, indrap
            pn.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            pn.SearchNameDate = null; //Said to be always 0
            pn.FirstName = ToDprFirstName(currentName.FirstName_s, currentName.MiddleName, false);
            pn.LastName = currentName.LastName.NullIfEmpty();

            // Special logic for addressing name
            pn.AddressingName = ToDprAddressingName(currentName.AddressingName);
            pn.SearchName = null; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            return pn;
        }

        public static PersonName ToDpr(this HistoricalNameType historicalName/*, PersonInformationType personInformation*/)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(historicalName.PNR);
            pn.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.Registration.RegistrationDate, 12);
            pn.NameAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod
            //TODO: Tjek om status angives på historiske data
            pn.Status = null;//personInformation.Status;
            //pn.StatusDate = CprBroker.Utilities.Dates.DateToDecimal(personInformation.StatusStartDate.Value, 13);
            if (!char.IsWhiteSpace(historicalName.FirstNameMarker))
                pn.FirstNameMarker = historicalName.FirstNameMarker;
            if (!char.IsWhiteSpace(historicalName.LastNameMarker))
                pn.SurnameMarker = historicalName.LastNameMarker;

            pn.NameStartDate = historicalName.NameStartDateDecimal;
            pn.NameTerminationDate = historicalName.NameEndDateDecimal;

            pn.AddressingNameDate = null; //TODO: Can be fetched in CPR Services, adrnvnhaenstart
            if (!char.IsWhiteSpace(historicalName.CorrectionMarker))
                pn.CorrectionMarker = historicalName.CorrectionMarker;
            pn.AddressingNameReportingMarker = null; //TODO: Can be fetched in CPR Services, indrap
            pn.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            pn.SearchNameDate = null; //Said to be always 0
            pn.FirstName = ToDprFirstName(historicalName.FirstName_s, historicalName.MiddleName, false);
            pn.LastName = historicalName.LastName;
            pn.AddressingName = null; // Seems not available in historical records....
            pn.SearchName = null; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            return pn;
        }

        public static string ToDprAddressingName(string addressingName)
        {
            if (!string.IsNullOrEmpty(addressingName))
            {
                if (addressingName.Contains(","))
                {
                    return addressingName;
                }
                else
                {
                    int indexAfterFirstName = addressingName.IndexOf(" ");
                    string firstName = addressingName.Substring(0, indexAfterFirstName);
                    string remainingAddressingNameParts = addressingName.Substring(
                        indexAfterFirstName, 
                        addressingName.Length - indexAfterFirstName).Remove(0, 1);
                    string DprAddressingName = string.Format("{1},{0}", remainingAddressingNameParts, firstName);
                    return DprAddressingName;
                }
            }
            return null;
        }

        public static string ToDprFirstName(string firstName, string middleName, bool upper)
        {
            int maxLength = 50;
            firstName = string.Format("{0}", firstName).Trim();
            middleName = string.Format("{0}", middleName).Trim();

            var parts = firstName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            parts.AddRange(middleName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            int lastIndex = parts.Count - 1;

            while (
                lastIndex >= 0 &&
                parts.Sum(p => p.Length) + parts.Count - 1 > maxLength)
            {
                parts[lastIndex] = parts[lastIndex][0].ToString().ToUpper();
                lastIndex--;
            }

            string ret = null;
            if (parts.Count > 0)
                ret = string.Join(" ", parts.ToArray());
            if (upper && ret != null)
                ret = ret.ToUpper();

            return ret;
        }

    }
}
