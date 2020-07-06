using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Business
{
    public class CompanyApiCreationParams
    {
        public string Name { get; set; }
        public long DescriptionLocalizedStringId { get; set; }
        public long OwnerPlayerId { get; set; }
        public CompanyExtensions Extensions { get; set; }
    }
}
