using EnterpriseBot.ApiWrapper.Models.Game.Essences;
using EnterpriseBot.ApiWrapper.Models.Enums;
using System.Collections.Generic;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Game.Crafting;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class CompanyJob
    {
        public long Id { get; set; }

        public LocalizedString Name { get; set; }
        public LocalizedString Description { get; set; }

        public Player Employee { get; set; }

        public Company Company { get; set; }

        public CompanyJobPermissions Permissions { get; set; }

        public decimal Salary { get; set; }

        public List<CompanyJobApplication> Applications { get; set; } = new List<CompanyJobApplication>();


        public bool IsOccupied { get; set; }
        public Recipe Recipe { get; set; }
        public CompanyStorage WorkingStorage { get; set; }
        public bool IsWorkingNow { get; set; }
    }
}
