﻿using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class CompanyJobCreationParams
    {
        public long NameLocalizedStringId { get; set; }
        public long DescriptionLocalizedStringId { get; set; }

        public long CompanyId { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public long? RecipeId { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public long? CompanyStorageId { get; set; }

        public CompanyJobPermissions Permissions { get; set; }

        public decimal Salary { get; set; }
    }
}
