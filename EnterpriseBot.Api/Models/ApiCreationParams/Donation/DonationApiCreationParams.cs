﻿using EnterpriseBot.Api.Game.Donation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Donation
{
    public class DonationApiCreationParams
    {
        public long PlayerId { get; set; }
        public Privilege Privilege { get; set; }
    }
}