using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Queues;

namespace CprBroker.Providers.CPRDirect
{
    public class ExtractStagingQueue : Engine.Queues.Queue<ExtractQueueItem>
    {
        public const int QueueId = 100;

        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            var dbr = Queue.GetQueues<DbrBaseQueue>(DbrBaseQueue.BaseQueueTypeId).First();
            var cpr = Queue.GetQueues<PartConversionQueue>().First();
            var sub = Queue.GetQueues<EnsureSubscriptionQueue>().First();

            dbr.Enqueue(items);
            cpr.Enqueue(items);
            sub.Enqueue(items);

            return items.ToArray();
        }
    }
}
