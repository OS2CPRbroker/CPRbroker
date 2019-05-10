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

using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.PartInterface;
using CprBroker.Engine;

namespace CprBroker.DBR
{
    public partial class NewRequestType
    {
        public NewRequestType(string contents)
        {
            Contents = contents.PadRight(this.Length);
        }

        public override DiversionResponseType Process(string dprConnectionString, bool skipAddressIfDead)
        {
            NewResponseType ret;
            try
            {
                // Register an operation, if this is not a 'remove subscription' call
                if (this.Type != InquiryType.DeleteAutomaticDataUpdateFromCpr)
                {
                    var uuid = CprBroker.Engine.Part.ReadSubMethodInfo.CprToUuid(this.PNR);
                    BrokerContext.Current.RegisterOperation(Data.Applications.OperationType.Types.DprDiversion, CprBroker.Utilities.Strings.GuidToString(uuid));
                }

                var dataProviders = new ICprDirectPersonDataProvider[0].AsEnumerable();

                if (Convert.ToBoolean(ForceDiversion))
                    dataProviders = LoadDataProviders<CPRDirectClientDataProvider>().Select(p => p as ICprDirectPersonDataProvider);
                else
                    dataProviders = LoadDataProviders<ICprDirectPersonDataProvider>();

                ICprDirectPersonDataProvider usedProvider;
                var person = this.GetPerson(dataProviders, out usedProvider);
                if (person == null)
                {
                    return ToErrorResponse(5);
                }

                switch (this.Type)
                {
                    case InquiryType.DataNotUpdatedAutomatically:
                        break;
                    case InquiryType.DataUpdatedAutomaticallyFromCpr:
                        break;
                    case InquiryType.DeleteAutomaticDataUpdateFromCpr:
                        break;
                    default:
                        break;
                }
                // Regardless of the data source, make sure there is a subscription to keep getting changes in CPR Broker
                if (!CanPutSubscription(usedProvider))
                {
                    // We have to create a subscription elsewhere
                    var subscriptionResult = this.PutSubscription();
                    if (!subscriptionResult)
                    {
                        return ToErrorResponse(5);
                    }
                }
                IList<object> objectsToInsert;
                try
                {
                    // Update the the broker's database, only the source itself is not an extract
                    if (!(usedProvider is IExtractDataProvider))
                        this.SaveAsExtract(person);

                    // Update the DPR database
                    objectsToInsert = this.GetDatabaseInserts(dprConnectionString, person, skipAddressIfDead);
                    this.UpdateDprDatabase(dprConnectionString, objectsToInsert);
                }
                catch (Exception e)
                {
                    Engine.Local.Admin.LogException(e);
                    return ToErrorResponse(20);
                }

                switch (LargeData) // Irrelevant
                {
                    default:
                        break;
                }

                ret = new NewResponseType()
                {
                    Type = this.Type,
                    LargeData = this.LargeData,
                    ErrorNumber = "00",
                };

                switch (this.ReponseData)
                {
                    case ResponseType.None:
                        ret.Data = new NewResponseNoDataType(person);
                        break;

                    case ResponseType.Basic:
                        ret.Data = new NewResponseBasicDataType(person, PersonInfoExtended.FromObjects(objectsToInsert), dprConnectionString);
                        break;

                    case ResponseType.Enriched:
                        ret.Data = new NewResponseFullDataType(person, PersonInfoExtended.FromObjects(objectsToInsert));
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Engine.Local.Admin.LogException(e);
                ret = ToErrorResponse(9);
            }
            return ret;
        }

        public NewResponseType ToErrorResponse(int code)
        {
            var codeString = code.ToString().PadLeft(2, '0');
            return new NewResponseType()
            {
                Type = this.Type,
                LargeData = this.LargeData,
                ErrorNumber = codeString,
                Data = new NewResponseNoDataType()
                {
                    PNR = this.PNR,
                    OkOrError = DiversionErrorCodes.ErrorCodes_Dk()[codeString]
                }
            };
        }

    }
}
