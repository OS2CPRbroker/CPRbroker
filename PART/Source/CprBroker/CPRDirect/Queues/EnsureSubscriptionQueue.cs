using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                // TODO: Get municipalities in subscription from config.

                foreach (var person in items)
                {
                    
                    var response = Extract.ToIndividualResponseType(person.Extract, person.ExtractItems.AsQueryable(), Constants.DataObjectMap);
                    decimal currentMunCode = response.CurrentAddressInformation.MunicipalityCode;
                    // TODO: Check if currentMunCode is in subscribed municipalities, if it is we dont need to check historical adresses

                    // If currentMunCode is not in subscribed municipalities, but latestMunCode is, then we need to subscribe to the CPR number

                    HistoricalAddressType identity = new HistoricalAddressType()
                    {
                        LeavingDate = DateTime.MinValue,
                        MunicipalityCode = 0
                    };
                    decimal latestMunCode = response.HistoricalAddress.Aggregate((a, b) => Newest(a, b)).MunicipalityCode;
                }
            }

            return items;
        }
    }
}
