using System;

namespace EnterpriseBot.ApiWrapper.Models.Enums
{
    [Flags]
    public enum CompanyExtensions
    {
        #region shop-specific extensions
        CanSignContracts            = 0b_0000_0000_0000_0001,

        CanUpgradeStorages          = 0b_0000_0000_0000_0010,
        CanBuyStorages              = 0b_0000_0000_0000_0100,

        CanHaveShowcase             = 0b_0000_0000_0000_1000,
        #endregion

        #region company-specific extensions
        CanHire                     = 0b_0000_0000_0001_0000,

        CanHaveTruckGarage          = 0b_0000_0000_0010_0000,
        CanExtendTruckGarage        = 0b_0000_0000_0100_0000,

        CanHaveRobots               = 0b_0000_0000_1000_0000,
        CanExtendRobots             = 0b_0000_0001_0000_0000,
        CanUpgradeRobots            = 0b_0000_0010_0000_0000,
        CanRobotsWorkInfinitely     = 0b_0000_0100_0000_0000
        #endregion
    }
}
