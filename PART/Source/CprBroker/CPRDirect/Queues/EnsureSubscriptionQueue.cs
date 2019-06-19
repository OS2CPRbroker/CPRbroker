using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                
                int[] subbedMunicipalities = PartInterface.Tracking.SettingsUtilities.ExcludedMunicipalityCodes;
                HashSet<String> personsToSubscribe = new HashSet<string>();

                foreach (var person in items)
                {
                    var response = Extract.ToIndividualResponseType(person.Extract, person.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                    decimal currentMunCode = response.CurrentAddressInformation.MunicipalityCode;
                    
                    HistoricalAddressType identity = new HistoricalAddressType()
                    {
                        LeavingDate = DateTime.MinValue,
                        MunicipalityCode = 0
                    };
                    decimal latestMunCode = response.HistoricalAddress.Aggregate((a, b) => Newest(a, b)).MunicipalityCode;
                    // If currentMunCode is not in subscribed municipalities, but latestMunCode is, then we need to subscribe to the CPR number
                    // TODO: make sure newest historical address is not current address
                    if (Array.Exists<int>(subbedMunicipalities, (a) => a == latestMunCode) 
                        && !Array.Exists<int>(subbedMunicipalities, (a) => a == currentMunCode))
                    {
                        personsToSubscribe.Add(person.PNR);
                    }
                }

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
                Guid[] guids = uuids.UUID.Select( u => Guid.Parse(u)).ToArray();
                BasicOutputType<bool> putsubs = pm.PutSubscription(bc.UserToken, bc.ApplicationToken, guids);
                if (!StandardReturType.IsSucceeded(putsubs.StandardRetur))
                {
                    CprBroker.Engine.Local.Admin.LogError("There was an error putting subscriptions of persons, in EnsureSubscriptionQueue. The error was: " + putsubs.StandardRetur.FejlbeskedTekst);
                    return new ExtractQueueItem[0];
                }
            }

            return items;
        }
    }
}
