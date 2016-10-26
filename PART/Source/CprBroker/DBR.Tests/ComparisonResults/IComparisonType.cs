﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR.ComparisonResults
{
    interface IComparisonType
    {
        PropertyComparisonResult[] ExcludedPropertiesInformation { get; }
        PropertyComparisonResult[] ExcludedPropertiesInformation90 { get; }
        Type TargetType { get; }
    }
}
