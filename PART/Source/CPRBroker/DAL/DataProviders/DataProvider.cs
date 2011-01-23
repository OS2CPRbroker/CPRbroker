﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL.DataProviders
{
    public partial class DataProvider
    {
        private DataProviderProperty GetDataProviderProperty(string key)
        {
            return (from p in this.DataProviderProperties where p.Name == key select p).FirstOrDefault();
        }

        public string this[string key]
        {
            get
            {
                var prop = GetDataProviderProperty(key);
                if (key != null)
                {
                    return prop.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var prop = GetDataProviderProperty(key);
                if (prop != null)
                {
                    prop.Value = value;
                }
                else
                {
                    this.DataProviderProperties.Add(new DataProviderProperty()
                        {
                            DataProvider = this,
                            Name = key,
                            Value = value
                        });
                }
            }
        }
    }
}
