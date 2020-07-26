using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using EnterpriseBot.ApiWrapper.Models.Game.Money;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using System;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Essences
{
    public class Player
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public LocalizedString Status { get; set; }
        public LocalizedString About { get; set; }

        public InventoryStorage Inventory { get; set; }

        public Purse Purse { get; set; }

        public List<CompanyJob> CompanyJobs { get; set; } = new List<CompanyJob>();
        public List<Company> OwnedCompanies { get; set; } = new List<Company>();
        public List<CompanyJobApplication> CompanyJobApplications { get; set; } = new List<CompanyJobApplication>();

        public Donation.Donation Donation { get; set; }

        public bool VkConnected { get; set; }
        public long? VkId { get; set; }

        public DateTime RegistrationDate { get; set; }

        public Reputation.Reputation Reputation { get; set; }


        public bool HasDonation { get; set; }
        public bool HasJob { get; set; }
    }
}
