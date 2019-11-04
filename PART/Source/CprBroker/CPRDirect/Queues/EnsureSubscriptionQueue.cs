using System;
using System.Collections.Generic;
using System.Linq;
using CprBroker.Engine;
using CprBroker.Schemas.Part;
using CprBroker.PartInterface;

namespace CprBroker.Providers.CPRDirect
{
    class EnsureSubscriptionQueue : Engine.Queues.Queue<ExtractQueueItem>
    {
        public const int QueueTypeId = 102;

        private HistoricalAddressType Newest(HistoricalAddressType a, HistoricalAddressType b)
        {

            if (a.LeavingDate >= b.LeavingDate)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            using (var dataContext = new ExtractDataContext())
            {
                items.LoadExtractAndItems(dataContext);
                
                int[] subbedMunicipalities = ExcludedMunicipalityCodes();
                HashSet<String> personsToSubscribe = new HashSet<string>();

                PartManager partManager = new PartManager();

                foreach (var person in items)
                {
                    try
                    {
                        // Used as citizen reference in log.
                        GetUuidOutputType UUIDOutput = partManager.GetUuid(
                        CprBroker.Utilities.Constants.EventBrokerApplicationToken.ToString(),
                        CprBroker.Utilities.Constants.BaseApplicationToken.ToString(),
                        person.PNR
                        );

                        var response = Extract.ToIndividualResponseType(person.Extract, person.ExtractItems.AsQueryable(), Constants.DataObjectMap);

                        /*
                        * ****************************************************
                        * ***** Checking if the municipality has changed *****
                        * ****************************************************
                        */

                        decimal currentMunCode = response.CurrentAddressInformation.MunicipalityCode;

                        HistoricalAddressType identity = new HistoricalAddressType()
                        {
                            LeavingDate = DateTime.MinValue,
                            MunicipalityCode = 0
                        };
                        decimal latestMunCode = response.HistoricalAddress.Aggregate(identity, (a, b) => Newest(a, b)).MunicipalityCode;
                        // If currentMunCode is not in subscribed municipalities, but latestMunCode is, then we need to subscribe to the CPR number
                        if (Array.Exists<int>(subbedMunicipalities, (a) => a == latestMunCode) 
                            && !Array.Exists<int>(subbedMunicipalities, (a) => a == currentMunCode))
                        {
                            personsToSubscribe.Add(person.PNR);
                            string logMsg = string.Format("Subscription put due to leaving municipality for citizen: {0}", UUIDOutput.UUID);
                            CprBroker.Engine.Local.Admin.LogSuccess(logMsg);
                        }

                        /*
                        * *********************************************************
                        * ***** If the person is reported leaving the country *****
                        * *********************************************************
                        */
                    
                        // 'IndividualResponseType.PersonInformation' contains data from CPR Direkte's record type '001'.
                        int status = Convert.ToInt32(response.PersonInformation.Status);

                        if (status == 80) //  "inactive, Departured".
                        {
                            personsToSubscribe.Add(person.PNR);
                            string logMsg = string.Format("Subscription put due to Status code 80 for citizen: {0}", UUIDOutput.UUID);
                            CprBroker.Engine.Local.Admin.LogSuccess(logMsg);
                        }

                        if (status == 20) // "inactive, without address in Denmark, or Greenland, but has cprno. due to tax reasons".
                        {
                            personsToSubscribe.Add(person.PNR);
                            string logMsg = string.Format("Subscription put due to Status code 20 for citizen: {0}", UUIDOutput.UUID);
                            CprBroker.Engine.Local.Admin.LogSuccess(logMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        CprBroker.Engine.Local.Admin.LogError(ex.ToString());
                    }
                }

                if (personsToSubscribe.Any())
                {
                    BrokerContext bc = BrokerContext.Current;
                    PartManager pm = new PartManager();
                    // Get UUIDs
                    GetUuidArrayOutputType uuids = pm.GetUuidArray(bc.UserToken, bc.ApplicationToken, personsToSubscribe.ToArray());
                    if (!StandardReturType.IsSucceeded(uuids.StandardRetur))
                    {
                        CprBroker.Engine.Local.Admin.LogError("There was an error getting UUIDS of persons, in EnsureSubscriptionQueue. The error was: " + uuids.StandardRetur.FejlbeskedTekst);
                        return new ExtractQueueItem[0];
                    }

                    // Subscribe persons
                    Guid[] guids = uuids.UUID.Select(u => Guid.Parse(u)).ToArray();
                    BasicOutputType<bool> putsubs = pm.PutSubscription(bc.UserToken, bc.ApplicationToken, guids);
                    if (!StandardReturType.IsSucceeded(putsubs.StandardRetur))
                    {
                        CprBroker.Engine.Local.Admin.LogError("There was an error putting subscriptions of persons, in EnsureSubscriptionQueue. The error was: " + putsubs.StandardRetur.FejlbeskedTekst);
                        return new ExtractQueueItem[0];
                    }
                }
            }

            return items;
        }

        public static int[] ExcludedMunicipalityCodes()
        {
                return Properties.Settings.Default.ExcludedMunicipalityCodes
                    .Cast<string>()
                    .Select(v => string.Format("{0}", v).TrimStart('0', ' '))
                    .Where(v => !string.IsNullOrEmpty(v))
                    .Select(v =>
                    {
                        int code;
                        if (int.TryParse(v, out code))
                            return code;
                        else
                            return (int?)null;
                    })
                    .Where(v => v.HasValue && v.Value > 0)
                    .Select(v => v.Value)
                    .ToArray();
        }
    }
}
