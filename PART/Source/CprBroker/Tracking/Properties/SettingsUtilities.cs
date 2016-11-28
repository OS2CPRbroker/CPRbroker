﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public static class SettingsUtilities
    {
        public static TimeSpan MaxInactivePeriod
        {
            get
            {
                return Properties.Settings.Default
                    .MaxInactivePeriod
                    .Duration();
            }
        }

        public static TimeSpan DprEmulationRemovalAllowance
        {
            get
            {
                return Properties.Settings.Default
                    .DprEmulationRemovalAllowance
                    .Duration();
            }
        }

        public static int[] ExcludedMunicipalityCodes
        {
            get
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
}
