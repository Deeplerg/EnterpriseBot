using EnterpriseBot.ApiWrapper.Categories.Business.Company;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class CompanySubCategoryList
    {
        internal CompanySubCategoryList() { }

        public CompanyCategory Company { get; internal set; }
        public CompanyContractCategory CompanyContract { get; internal set; }
        public CompanyContractRequestCategory CompanyContractRequest { get; internal set; }
        public CompanyJobApplicationCategory CompanyJobApplication { get; internal set; }
        public CompanyJobCategory CompanyJob { get; internal set; }

        [Obsolete("Should not be used in a normal flow")]
        public CompanyWorkerCategory CompanyWorker { get; internal set; }

        public ProductionRobotCategory ProductionRobot { get; internal set; }
        public TruckCategory Truck { get; internal set; }
        public TruckGarageCategory TruckGarage { get; internal set; }
    }
}
