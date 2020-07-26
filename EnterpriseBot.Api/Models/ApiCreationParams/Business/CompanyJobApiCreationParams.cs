using EnterpriseBot.Api.Models.Common.Enums;
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

        public long InvokerPlayerId { get; set; }
    }
}
