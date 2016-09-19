﻿using CprBroker.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public interface ITrackingDataProvider : IDataProvider
    {
        PersonTrack[] GetTrack(Guid[] personUuids, DateTime? fromDate, DateTime? toDate);
    }
}
