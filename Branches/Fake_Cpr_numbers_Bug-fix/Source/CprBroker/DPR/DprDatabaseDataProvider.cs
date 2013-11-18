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
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Util;
using System.Linq.Expressions;


namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Implements the Read operation of Part standard
    /// </summary>
    public partial class DprDatabaseDataProvider : ClientDataProvider, IPutSubscriptionDataProvider, IPartReadDataProvider
    {

        public RegistreringType1 Read(PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out QualityLevel? ql)
        {
            CprBroker.Schemas.Part.RegistreringType1 ret = null;
            EnsurePersonDataExists(uuid.CprNumber);

            using (var dataContext = new DPRDataContext(this.ConnectionString))
            {
                var db = PersonInfo.GetPersonInfo(dataContext, decimal.Parse(uuid.CprNumber));
                if (db != null)
                {
                    ret = db.ToRegisteringType1(cpr2uuidFunc, dataContext, this);
                }
            }
            ql = QualityLevel.DataProvider;
            return ret;
        }

        public bool PutSubscription(PersonIdentifier personIdentifier)
        {
            if (!this.DisableDiversion)
            {
                decimal cprNum = Convert.ToDecimal(personIdentifier.CprNumber);

                using (DPRDataContext dataContext = new DPRDataContext(ConnectionString))
                {
                    var exists = (from personTotal in dataContext.PersonTotals
                                  select personTotal.PNR).Contains(cprNum);

                    if (exists)
                    {
                        Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "PutSubscription", string.Format("PNR {0} Exists in DPR, DPR Diversion not called", personIdentifier.CprNumber), null, null);
                    }
                    else
                    {
                        if (modulus11OK(personIdentifier.CprNumber))
                        {
                            Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "PutSubscription", string.Format("Calling DPR Diversion : {0}", personIdentifier.CprNumber), null, null);
                            CallDiversion(InquiryType.DataUpdatedAutomaticallyFromCpr, DetailType.MasterData, personIdentifier.CprNumber);
                        }
                        else
                        {
                            Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "PutSubscription", string.Format("PNR {0} is invalid, DPR Diversion not called", personIdentifier.CprNumber), null, null);
                        }
                    }
                    return true;
                }
            }
            else
            {
                Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "PutSubscription", string.Format("DPR Diversion is disabled: {0}", personIdentifier.CprNumber), null, null);
                return false;
            }
        }
    }
}