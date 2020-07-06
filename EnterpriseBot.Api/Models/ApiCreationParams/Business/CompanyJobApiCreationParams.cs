﻿using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Business
{
    public class CompanyJobApiCreationParams
    {
        public long NameLocalizedStringId { get; set; }
        public long DescriptionLocalizedStringId { get; set; }

        public long CompanyId { get; set; }
        public long RecipeId { get; set; }
        public long StorageId { get; set; }

        public bool HasRecipe { get; set; }

        public CompanyJobPermissions Permissions { get; set; }

        public decimal Salary { get; set; }
    }
}