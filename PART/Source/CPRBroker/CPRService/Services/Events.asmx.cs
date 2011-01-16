﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part.Events;

namespace CprService.Services
{
    /// <summary>
    /// Summary description for Events
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Events : System.Web.Services.WebService
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";

        [WebMethod]
        [SoapHeader(ApplicationHeaderName)]
        public DataChangeEventInfo[] DequeueDataChangeEvents(int maxCount)
        {
            return Manager.Events.DequeueDataChangeEvents(applicationHeader.UserToken, applicationHeader.ApplicationToken, maxCount);
        }

        [WebMethod]
        [SoapHeader(ApplicationHeaderName)]
        public PersonBirthdate[] GetPersonBirthdates(Guid? personUuidToStartAfter, int maxCount)
        {
            return Manager.Events.GetPersonBirthdates(applicationHeader.UserToken, applicationHeader.ApplicationToken, personUuidToStartAfter, maxCount);
        }
    }
}
