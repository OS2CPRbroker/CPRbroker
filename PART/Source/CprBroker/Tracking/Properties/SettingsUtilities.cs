using System;
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
                return Providers.CPRDirect.SettingsUtilities.ExcludedMunicipalityCodes;
            }
        }
    }
}
