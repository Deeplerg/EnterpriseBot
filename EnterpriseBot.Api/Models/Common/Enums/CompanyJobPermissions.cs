namespace EnterpriseBot.Api.Models.Common.Enums
{
    [System.Flags]
    public enum CompanyJobPermissions : ulong
    {
        ProduceItems                      = 0b_0000_0000_0000_0000_0000_0000_0000_0001,


        #region storages
        BuyStorages                       = 0b_0000_0000_0000_0000_0000_0000_0000_0010,
        UpgradeStorages                   = 0b_0000_0000_0000_0000_0000_0000_0000_0100,

        ManageTrunkStorages               = 0b_0000_0000_0000_0000_0000_0000_0000_1000,
        ManageShowcaseStorages            = 0b_0000_0000_0000_0000_0000_0000_0001_0000,
        ManageCompanyStorages             = 0b_0000_0000_0000_0000_0000_0000_0010_0000,

        ManageAllStorages                 = ManageTrunkStorages | ManageShowcaseStorages 
                                          | ManageCompanyStorages,

        ManageStoragesInAnyWay            = ManageAllStorages | BuyStorages | UpgradeStorages,
        #endregion


        #region jobs
        CreateJob                         = 0b_0000_0000_0000_0000_0000_0000_0100_0000,
        Hire                              = 0b_0000_0000_0000_0000_0000_0000_1000_0000,
        Fire                              = 0b_0000_0000_0000_0000_0000_0001_0000_0000,
        ChangeJobParameters               = 0b_0000_0000_0000_0000_0000_0010_0000_0000,

        ManageJobsInAnyWay                = CreateJob | Hire | Fire | ChangeJobParameters,
        #endregion


        #region contracts
        SignContracts                     = 0b_0000_0000_0000_0000_0000_0100_0000_0000,
        BreakContracts                    = 0b_0000_0000_0000_0000_0000_1000_0000_0000,

        ManageContractsInAnyWay           = SignContracts | BreakContracts,
        #endregion


        #region trucks
        BuyTrucks                         = 0b_0000_0000_0000_0000_0001_0000_0000_0000,
        UpgradeTrucks                     = 0b_0000_0000_0000_0000_0010_0000_0000_0000,
        UpgradeTruckGarage                = 0b_0000_0000_0000_0000_0100_0000_0000_0000,

        ManageTruckGarage                 = BuyTrucks | UpgradeTrucks | UpgradeTruckGarage,

        SendTrucks                        = 0b_0000_0000_0000_0000_1000_0000_0000_0000,
        ReturnTrucks                      = 0b_0000_0000_0000_0001_0000_0000_0000_0000,

        ManageTrucksState                 = SendTrucks | ReturnTrucks,

        ManageTrucksInAnyWay              = ManageTruckGarage | ManageTrucksState | ManageTrunkStorages,
        #endregion


        #region robots
        BuyRobots                         = 0b_0000_0000_0000_0010_0000_0000_0000_0000,
        UpgradeRobots                     = 0b_0000_0000_0000_0100_0000_0000_0000_0000,

        ManageRobotTasks                  = 0b_0000_0000_0000_1000_0000_0000_0000_0000,
        ChangeRobotParameters             = 0b_0000_0000_0001_0000_0000_0000_0000_0000,

        ManageRobotsInAnyWay              = BuyRobots | UpgradeRobots | ManageRobotTasks | ChangeJobParameters,
        #endregion

        ChangeDescription                 = 0b_0000_0000_0010_0000_0000_0000_0000_0000,
        ChangeName                        = 0b_0000_0000_0100_0000_0000_0000_0000_0000,                     

        GeneralManager                    = ProduceItems | ManageAllStorages | ManageJobsInAnyWay
                                          | ManageContractsInAnyWay | ManageTrucksInAnyWay | ManageRobotsInAnyWay
                                          | ChangeDescription | ChangeName
    }
}
