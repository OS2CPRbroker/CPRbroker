﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.Providers.DPR
{
    public partial class CivilStatus
    {
        public MaritalStatusCodeType CurrentMaritalStatusCode
        {
            get
            {
                if (CIVST.HasValue)
                {
                    switch (CIVST.Value)
                    {
                        case Constants.MaritalStatus.Unmarried:
                            return MaritalStatusCodeType.unmarried;
                        case Constants.MaritalStatus.Married:
                            return MaritalStatusCodeType.married;
                        case Constants.MaritalStatus.Divorced:
                            return MaritalStatusCodeType.divorced;
                        case Constants.MaritalStatus.Deceased:
                            return MaritalStatusCodeType.deceased;
                        case Constants.MaritalStatus.Widow:
                            return MaritalStatusCodeType.widow;
                        case Constants.MaritalStatus.RegisteredPartnership:
                            return MaritalStatusCodeType.registeredpartnership;
                        case Constants.MaritalStatus.AbolitionOfRegisteredPartnership:
                            return MaritalStatusCodeType.abolitionofregistreredpartnership;
                        case Constants.MaritalStatus.LongestLivingPartner:
                            return MaritalStatusCodeType.longestlivingpartner;
                    }
                }
                return MaritalStatusCodeType.unmarried;
            }
        }
    }
}
