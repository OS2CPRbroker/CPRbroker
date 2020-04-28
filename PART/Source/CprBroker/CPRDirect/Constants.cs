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
 * Dennis Isaksen
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

namespace CprBroker.Providers.CPRDirect
{
    public static class Constants
    {
        static Constants()
        {
            // Reference: "CPR Direkte Grænsefladebeskrivelse OFF4.pdf"(v7.0), p.11.
            _ErrorCodes = new Dictionary<string, string>();
            _ErrorCodes["00"] = "No error / Success";
            _ErrorCodes["01"] = "Incorrect user ID / remote server";
            _ErrorCodes["02"] = "Remote server expired, new remote server required";
            _ErrorCodes["03"] = "New remote server does not meet the format (8 characters, at least 2 numbers and 2 letters and not previously used)";
            _ErrorCodes["04"] = "No access to CPR";
            _ErrorCodes["05"] = "PNR unknown in CPR";
            _ErrorCodes["06"] = "Unknown Customer Number";
            _ErrorCodes["07"] = "Timeout - new LOGON required";
            _ErrorCodes["08"] = "'DEAD-LOCK' while reading the CPR system";
            _ErrorCodes["09"] = "Serious problem. Meaning: There is no connection between the client and CPR system; contact CSC Service Center by phone 36146192";
            _ErrorCodes["10"] = "ABON_TYPE unknown";
            _ErrorCodes["11"] = "DATA_TYPE unknown";
            _ErrorCodes["12"] = "Error code 12 (reserved error number)";
            _ErrorCodes["13"] = "(reserved error number)";
            _ErrorCodes["14"] = "BRUGER-ID does not have access to transaction(CTPROFILE is temporarily closed)";
            _ErrorCodes["15"] = "Error code 15 (reserved error number)";
            _ErrorCodes["16"] = "IP address is incorrect";
            _ErrorCodes["17"] = "PNR not specified";
            _ErrorCodes["18"] = "BRUGER-ID does not have access to transaction(CTPROFILE is not active)";
            _ErrorCodes["19"] = "Error code 19 (reserved error number)";
            _ErrorCodes["20"] = "Error code 20 (reserved error number)";
            _ErrorCodes["21"] = "Error code 21 (reserved error number)";
            _ErrorCodes["22"] = "Error code 22 (reserved error number)";
            _ErrorCodes["23"] = "Error code 23 (reserved error number)";
            _ErrorCodes["24"] = "BRUGER-ID does not have access to transaction(CTPERSKOD_PROFIL is temporarily closed)";
            _ErrorCodes["25"] = "Error code 25 (reserved error number)";
            _ErrorCodes["26"] = "Error code 26 (reserved error number)";
            _ErrorCodes["27"] = "Error code 27 (reserved error number)";
            _ErrorCodes["28"] = "BRUGER-ID does not have access to transaction(CTPERSKOD_PROFIL is not active)";

            for (int err = 29; err < 100; err++)
            {
                _ErrorCodes[err.ToString()] = "(reserved error number)";
            }


            _DataObjectMap = new Dictionary<string, Type>();
            _DataObjectMap["000"] = typeof(StartRecordType);
            _DataObjectMap["001"] = typeof(PersonInformationType);
            _DataObjectMap["002"] = typeof(CurrentAddressInformationType);
            _DataObjectMap["003"] = typeof(ClearWrittenAddressType);
            _DataObjectMap["004"] = typeof(ProtectionType);
            _DataObjectMap["005"] = typeof(CurrentDepartureDataType);
            _DataObjectMap["006"] = typeof(ContactAddressType);
            _DataObjectMap["007"] = typeof(CurrentDisappearanceInformationType);
            _DataObjectMap["008"] = typeof(CurrentNameInformationType);
            _DataObjectMap["009"] = typeof(BirthRegistrationInformationType);
            _DataObjectMap["010"] = typeof(CurrentCitizenshipType);
            _DataObjectMap["011"] = typeof(ChurchInformationType);
            _DataObjectMap["012"] = typeof(CurrentCivilStatusType);
            _DataObjectMap["013"] = typeof(CurrentSeparationType);
            _DataObjectMap["014"] = typeof(ChildType);
            _DataObjectMap["015"] = typeof(ParentsInformationType);
            _DataObjectMap["016"] = typeof(ParentalAuthorityType);
            _DataObjectMap["017"] = typeof(DisempowermentType);
            _DataObjectMap["018"] = typeof(MunicipalConditionsType);
            _DataObjectMap["019"] = typeof(NotesType);
            _DataObjectMap["020"] = typeof(ElectionInformationType);
            _DataObjectMap["021"] = typeof(RelocationOrderType);
            _DataObjectMap["022"] = typeof(HistoricalPNRType);
            _DataObjectMap["023"] = typeof(HistoricalAddressType);
            _DataObjectMap["024"] = typeof(HistoricalDepartureType);
            _DataObjectMap["025"] = typeof(HistoricalDisappearanceType);
            _DataObjectMap["026"] = typeof(HistoricalNameType);
            _DataObjectMap["027"] = typeof(HistoricalCitizenshipType);
            _DataObjectMap["028"] = typeof(HistoricalChurchInformationType);
            _DataObjectMap["029"] = typeof(HistoricalCivilStatusType);
            _DataObjectMap["030"] = typeof(HistoricalSeparationType);
            _DataObjectMap["031"] = typeof(MotherWithClearWrittenAddressType);
            _DataObjectMap["032"] = typeof(FatherWithClearWrittenAddressType);
            _DataObjectMap["033"] = typeof(ChildWithClearWrittenAddressType);
            _DataObjectMap["099"] = typeof(EventsType);
            _DataObjectMap["910"] = typeof(ErrorRecordType);
            _DataObjectMap["997"] = typeof(SubscriptionDeletionReceiptType);
            _DataObjectMap["999"] = typeof(EndRecordType);

            _RelationshipMap = new Dictionary<string, bool>();
            _ReversibleRelationshipMap = new Dictionary<string, bool>();
            _MultiRelationshipMap = new Dictionary<string, bool>();

            foreach (var kvp in _DataObjectMap)
            {
                var type = kvp.Value;
                _RelationshipMap[kvp.Key] = typeof(IRelationship).IsAssignableFrom(type);
                _ReversibleRelationshipMap[kvp.Key] = typeof(IReversibleRelationship).IsAssignableFrom(type);
                _MultiRelationshipMap[kvp.Key] = typeof(IMultipleRelationship).IsAssignableFrom(type);
            }

            _DataObjectMap_P02680 = new Dictionary<string, Type>();
            _DataObjectMap_P02680["001"] = typeof(AuthorityType);

            _DataObjectMap_P05780 = new Dictionary<string, Type>();
            _DataObjectMap_P05780["000"] = typeof(GeoStartRecordType);
            _DataObjectMap_P05780["001"] = typeof(StreetType);
            _DataObjectMap_P05780["002"] = typeof(ResidenceType);
            _DataObjectMap_P05780["003"] = typeof(CityType);
            _DataObjectMap_P05780["004"] = typeof(PostDistrictType);
            _DataObjectMap_P05780["005"] = typeof(NoteType);
            _DataObjectMap_P05780["006"] = typeof(AreaRestorationDistrictType);
            _DataObjectMap_P05780["007"] = typeof(DiverseDistrictType);
            _DataObjectMap_P05780["008"] = typeof(EvacuationDistrictType);
            _DataObjectMap_P05780["009"] = typeof(ChurchDistrictType);
            _DataObjectMap_P05780["010"] = typeof(SchoolDistrictType);
            _DataObjectMap_P05780["011"] = typeof(PopulationDistrictType);
            _DataObjectMap_P05780["012"] = typeof(SocialDistrictType);
            _DataObjectMap_P05780["013"] = typeof(ChurchAdministrationDistrictType);
            _DataObjectMap_P05780["014"] = typeof(ElectionDistrictType);
            _DataObjectMap_P05780["015"] = typeof(HeatingDistrictType);
            _DataObjectMap_P05780["999"] = typeof(GeoEndRecordType);

            _DataObjectMap_P11980 = new Dictionary<string, Type>();
            _DataObjectMap_P11980["000"] = typeof(PostStartRecordType);
            _DataObjectMap_P11980["001"] = typeof(PostNumberType);
            _DataObjectMap_P11980["999"] = typeof(PostEndRecordType);
        }

        private static Dictionary<string, Type> _DataObjectMap;
        public static Dictionary<string, Type> DataObjectMap
        {
            get { return _DataObjectMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }

        private static Dictionary<string, Type> _DataObjectMap_P02680;
        public static Dictionary<string, Type> DataObjectMap_P02680
        {
            get { return _DataObjectMap_P02680.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }

        private static Dictionary<string, Type> _DataObjectMap_P05780;
        public static Dictionary<string, Type> DataObjectMap_P05780
        {
            get { return _DataObjectMap_P05780.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }

        private static Dictionary<string, Type> _DataObjectMap_P11980;
        public static Dictionary<string, Type> DataObjectMap_P11980
        {
            get { return _DataObjectMap_P11980.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }

        private static Dictionary<string, bool> _RelationshipMap;
        public static Dictionary<string, bool> RelationshipMap
        {
            get { return _RelationshipMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }

        private static Dictionary<string, bool> _ReversibleRelationshipMap;
        public static Dictionary<string, bool> ReversibleRelationshipMap
        {
            get { return _ReversibleRelationshipMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }

        private static Dictionary<string, bool> _MultiRelationshipMap;
        public static Dictionary<string, bool> MultiRelationshipMap
        {
            get { return _MultiRelationshipMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); }
        }

        public const int DataObjectCodeLength = 3;

        public const int StartRecordCode = 0;
        public const int EndRecordCode = 999;


        /// <summary>
        /// Encoding for TCP client and test data file
        /// </summary>
        public static Encoding TcpClientEncoding
        {
            get { return Encoding.GetEncoding(1252); }
        }

        /// <summary>
        /// Encoding for change extracts
        /// </summary>
        public static Encoding ExtractEncoding
        {
            get { return Encoding.GetEncoding(28591); }
        }

        private static Dictionary<string, string> _ErrorCodes;
        public static Dictionary<string, string> ErrorCodes
        {
            get { return new Dictionary<string, string>(_ErrorCodes); }
        }

        public static readonly Guid ActorId = new Guid("{2B2C1518-F466-491F-8149-57AFEF48CC01}");
        public static readonly string CommentText = "";
        public static readonly short DenmarkCountryCode = 5100;

        public static class PropertyNames
        {
            public static readonly string Address = "Address";
            public static readonly string Port = "Port";
            public static readonly string DisableSubscriptions = "Disable subscription";
            public static readonly string ExtractsFolder = "Extracts folder";
            public static readonly string MultiLine = "Multi line";
            public static readonly string HasFtpSource = "Has FTP Source";
            public static readonly string FtpAddress = "FTP Address";
            public static readonly string FtpUser = "FTP User";
            public static readonly string FtpPassword = "FTP Password";
            public static readonly string FtpRegexFilter = "FTP Regex filter";
            public static readonly string LocalProxyUsage = "Local proxy usage";
            public static readonly string IsSharingSubscriptions = "Is Sharing Subscriptions";
        }

        public static class ResponseLengths
        {
            public static readonly int MaxResponseLength = 2880 + 28;
            public static readonly int ErrorCodeIndex = 22;
            public static readonly int ErrorCodeLength = 2;
        }

        public static System.Threading.ReaderWriterLockSlim AuthorityLock = new System.Threading.ReaderWriterLockSlim();
        
        public const string OnlineOperationName = "Online";

        public enum AuthorityTypes
        {
            Municipality = 5
        }
    }
}
