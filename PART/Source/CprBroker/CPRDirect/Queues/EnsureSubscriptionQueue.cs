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
                }
            }

            return items;
        }
    }
}
