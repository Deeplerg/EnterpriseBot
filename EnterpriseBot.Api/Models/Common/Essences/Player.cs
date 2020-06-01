using EnterpriseBot.Api.Models.Common.Business;
using EnterpriseBot.Api.Models.Common.Storages;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnterpriseBot.Api.Models.Common.Essences
{
    public class Player
    {
        public long Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        public string PasswordSaltBase64 { get; set; }


        public bool VkConnected { get; set; }
        public long? VkId { get; set; }


        public decimal Units { get; set; }

        public virtual PersonalStorage PersonalStorage { get; set; }


        public virtual List<Company> OwnedCompanies { get; set; }
        public virtual List<Shop> OwnedShops { get; set; }


        public bool HasJob { get; set; }

        public long? JobId { get; set; }
        public virtual Job Job { get; set; }
    }
}
