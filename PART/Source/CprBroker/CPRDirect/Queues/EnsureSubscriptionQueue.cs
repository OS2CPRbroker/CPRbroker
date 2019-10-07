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

                foreach (var person in items)
                {
                    var response = Extract.ToIndividualResponseType(person.Extract, person.ExtractItems.AsQueryable(), Constants.DataObjectMap);
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
                    }

                    // If the person has reported leaving the country then put a subscription.

                    // 'IndividualResponseType.PersonInformation' contains data from CPR Direkte's record type '001'.
                    int status = Convert.ToInt32(response.PersonInformation.Status);

                    /*
                     * Statuskoder for record type 001: 
                     * 01 = Aktiv, bopæl i dansk folkeregister
                     * 03 = Aktiv, speciel vejkode(9900 - 9999) i dansk folkeregister
                     * 05 = Aktiv, bopæl i grønlandsk folkeregister
                     * 07 = Aktiv, speciel vejkode(9900 - 9999) i grønlandsk folkeregister
                     * 20 = Inaktiv, uden bopæl i dansk/ grønlandsk folkeregister men tildelt personnummer af skattehensyn(kommunekoderne 0010, 0011, 0012 og 0019)
                     * 30 = Inaktiv, anulleret personnummer
                     * 50 = Inaktiv, slettet personnummer ved dobbeltnummer
                     * 60 = Inaktiv, ændret personnummer ved ændring af fødselsdato og køn
                     * 70 = Inaktiv, forsvundet
                     * 80 = Inaktiv,udrejst
                     * 90 = Inaktiv, død 
                     */

                    if (status == 20 || status == 70 || status == 80) 
                    {
                        personsToSubscribe.Add(person.PNR);
                    }
                    else if (status == 30 || status == 50 || status == 60 || status == 90)
                    {
                        personsToSubscribe.Remove(person.PNR);
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
