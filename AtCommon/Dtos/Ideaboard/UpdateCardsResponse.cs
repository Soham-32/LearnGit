﻿using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Ideaboard
{
    public class UpdateCardsResponse
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRUserId { get; set; }
        public List<CardResponse> Cards { get; set; }
    }
}