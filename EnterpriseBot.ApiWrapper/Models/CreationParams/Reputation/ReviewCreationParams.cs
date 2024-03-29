﻿using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Reputation
{
    public class ReviewCreationParams
    {
        public string Text { get; set; }

        public Reviewer Reviewer { get; set; }

        public long ReviewerCompanyId { get; set; }
        public long ReviewerPlayerId { get; set; }

        public sbyte Rating { get; set; }
    }
}
