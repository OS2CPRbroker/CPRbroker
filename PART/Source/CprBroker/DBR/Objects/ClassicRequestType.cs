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
using System.Data.SqlClient;
using CprBroker.Engine;
using CprBroker.Data.DataProviders;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Utilities.Config;
using CprBroker.PartInterface;
using CprBroker.Schemas;
using CprBroker.Schemas.Wrappers;

namespace CprBroker.DBR
{
    public partial class ClassicRequestType
    {
        public ClassicRequestType()
            : base(new string(' ', 40))
        { }

        public ClassicRequestType(string contents)
            : this()
        {
            Contents = contents.PadRight(Length).Substring(0, base.Length);
            NewType = "MMXIII";
            ForceDiversion = 0;
            ReponseData = 'S';
            UserID = "";
        }

        public override DiversionResponseType Process(string dprConnectionString, bool skipAddressIfDead = false)
        {
            // Run the new request
            var response = base.Process(dprConnectionString, skipAddressIfDead) as NewResponseType;

            // Convert to classic response
            return new ClassicResponseType()
            {
                Type = this.Type,
                LargeData = this.LargeData,
                ErrorNumber = response.ErrorNumber,
                PNR = this.PNR,
                Data = response.ErrorNumber.Equals("00") ?
                    LargeData == DetailType.ExtendedData ? " Basen er opdateret" : new ClassicResponseBasicDataType(response.Data as NewResponseBasicDataType).Contents.Substring(10)
                    : (response.Data as NewResponseNoDataType).OkOrError
            };
        }
    }

}
