﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public RegistreringType1 ToRegistreringType1(Func<string, Guid> cpr2uuidFunc)
        {
            return new RegistreringType1()
            {
                AktoerRef = ToAktoerRefType(),
                AttributListe = ToAttributListeType(),
                CommentText = ToCommentText(),
                LivscyklusKode = ToLivscyklusKodeType(),
                RelationListe = ToRelationListeType(cpr2uuidFunc),
                Tidspunkt = ToTidspunktType(),
                TilstandListe = ToTilstandListeType(),
                Virkning = ToVirkningType()
            };
        }
    }
}
