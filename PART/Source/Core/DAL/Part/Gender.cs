﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Part
{
    public partial class Gender
    {
        private static List<KeyValuePair<string, Schemas.Part.PersonGenderCodeType>> _PartValues = new List<KeyValuePair<string, Schemas.Part.PersonGenderCodeType>>();

        private static void LoadPartValues()
        {
            lock (_PartValues)
            {
                if (_PartValues.Count == 0)
                {
                    _PartValues.AddRange(CprBroker.Schemas.Util.Enums.GetEnumValues<Schemas.Part.PersonGenderCodeType>());
                }
            }
        }

        public static int GetPartCode(Schemas.Part.PersonGenderCodeType personGenderCode)
        {
            LoadPartValues();
            var code = (from kvp in _PartValues where kvp.Value == personGenderCode select kvp.Key).FirstOrDefault();
            return Convert.ToInt32(code);
        }

        public static Schemas.Part.PersonGenderCodeType GetPartGender(int code)
        {
            LoadPartValues();
            string sCode = code.ToString();
            return (from kvp in _PartValues where kvp.Key.Equals(sCode) select kvp.Value).FirstOrDefault();
        }
    }
}
