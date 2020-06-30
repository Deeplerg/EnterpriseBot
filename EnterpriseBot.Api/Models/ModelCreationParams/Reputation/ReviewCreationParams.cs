using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Reputation
{
    public class ReviewCreationParams
    {
        public string Text { get; set; }

        public Reviewer Reviewer { get; set; }

        public Company CompanyReviewer { get; set; }
        public Player PlayerReviewer { get; set; }

        public sbyte Rating { get; set; }
    }
}
