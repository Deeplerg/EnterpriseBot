using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.BusinessSettings.Company
{
    public class CompanyContractSettings
    {
        public ushort MaxContracts { get; set; }
        public uint MaxTimeInDays { get; set; }
    }
}
