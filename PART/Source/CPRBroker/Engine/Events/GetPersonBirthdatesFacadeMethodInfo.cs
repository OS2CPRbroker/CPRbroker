﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Events
{
    public class GetPersonBirthdatesFacadeMethodInfo : FacadeMethodInfo<Schemas.Part.Events.PersonBirthdate[]>
    {
        Guid? PersonUuidToStartAfter;
        int MaxCount;

        public GetPersonBirthdatesFacadeMethodInfo(Guid? personUuidToStartAfter, int maxCount, string appToken, string userToken, bool appTokenRequired)
            : base(appToken, userToken, appTokenRequired)
        {
            this.PersonUuidToStartAfter = personUuidToStartAfter;
        }

        public override bool IsValidInput(ref CprBroker.Schemas.Part.Events.PersonBirthdate[] invaliInputReturnValue)
        {
            return MaxCount > 0;
        }

        public override void Initialize()
        {
            SubMethodInfos = new SubMethodInfo[] { new GetPersonBirthdatesSubmethodInfo(PersonUuidToStartAfter, MaxCount) };
        }
    }
}
