using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings.Company;
using EnterpriseBot.Api.Models.Settings.BusinessSettings.Company;
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class Truck
    {
        protected Truck() { }

        #region model
        public long Id { get; protected set; }

        public virtual TruckGarage TruckGarage { get; protected set; }

        public virtual TrunkStorage Trunk { get; protected set; }

        public uint DeliveringSpeedInSeconds { get; protected set; }

        public TruckState CurrentState { get; protected set; }

        [JsonIgnore]
        public string UnloadTruckJobId { get; set; }
        [JsonIgnore]
        public string ReturnTruckJobId { get; set; }
        #endregion

        #region actions
        public static GameResult<Truck> Create(TruckCreationParams creationPars)
        {
            return CreateTruck(creationPars);
        }

        public static EmptyGameResult Buy(TruckCreationParams creationPars,
            CompanyFeaturesPricesSettings pricesSettings, DonationSettings donationSettings, Player invoker)
        {
            var cp = creationPars;

            if (!invoker.HasPermission(CompanyJobPermissions.BuyTrucks, cp.TruckGarage.Company))
            {
                return Errors.DoesNotHavePermission();
            }

            var reduceResult = cp.TruckGarage.Company.ReduceBusinessCoins(pricesSettings.NewTruckSetup, donationSettings, invoker);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return new EmptyGameResult();
        }

        public EmptyGameResult SendTruck(CompanyContract contract, Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.SendTrucks, contract.OutcomeCompany))
            {
                return Errors.DoesNotHavePermission();
            }

            CurrentState = TruckState.OnTheWayTowards;

            return new EmptyGameResult();
        }

        public EmptyGameResult ReturnTruck()
        {
            CurrentState = TruckState.ReadyToGo;

            return new EmptyGameResult();
        }

        public EmptyGameResult Unload(CompanyStorage storage, CompanyContract contract)
        {
            if(storage.Type != CompanyStorageType.Income)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Storage type is {storage.Type}, but expected: {CompanyStorageType.Income}",
                    RussianMessage = $"Тип хранилища - {storage.Type}, но ожидалось: {CompanyStorageType.Income}"
                };
            }

            if(CurrentState != TruckState.OnTheWayTowards)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Truck state is {CurrentState}, but expected: {TruckState.OnTheWayTowards}",
                    RussianMessage = $"Статус грузовика - {CurrentState}, но ожидалось: {TruckState.OnTheWayTowards}"
                };
            }

            int quantity;
            int needed = contract.DeliveredAmount - contract.ContractItemQuantity;
            int neededItemsAmountInTrunk = 0;

            if(Trunk.Items != null)
            {
                neededItemsAmountInTrunk = Trunk.Items.Sum(sItem => sItem.Item == contract.ContractItem ? sItem.Quantity : 0 );
            }

            if(neededItemsAmountInTrunk == 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The truck does not have any items requested by a contract",
                    RussianMessage = "В грузовике нет предметов, запрошенных контрактом"
                };
            }

            if(needed > neededItemsAmountInTrunk)
            {
                quantity = neededItemsAmountInTrunk;
            }
            else
            {
                quantity = neededItemsAmountInTrunk - needed;
            }

            var transferResult = Trunk.TransferTo(storage, contract.ContractItem, quantity);
            if (transferResult.LocalizedError != null) return transferResult.LocalizedError;

            contract.AddDeliveredAmount(transferResult.Result);

            CurrentState = TruckState.OnTheWayBack;

            return new EmptyGameResult();
        }

        public GameResult<uint> Upgrade(TruckUpgradePriceSetting upgradePriceSetting, TruckSettings truckSettings, Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.UpgradeTrucks, TruckGarage.Company))
            {
                return Errors.DoesNotHavePermission();
            }

            return Upgrade(upgradePriceSetting.StepInSeconds, upgradePriceSetting, truckSettings);
        }

        public GameResult<uint> Upgrade(uint stepInSeconds, 
            TruckUpgradePriceSetting upgradePriceSetting, TruckSettings truckSettings, Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.UpgradeTrucks, TruckGarage.Company))
            {
                return Errors.DoesNotHavePermission();
            }

            return Upgrade(stepInSeconds, upgradePriceSetting, truckSettings);
        }

        public GameResult<uint> Upgrade(uint stepInSeconds, 
            TruckUpgradePriceSetting upgradePriceSetting, TruckSettings truckSettings)
        {
            var prices = upgradePriceSetting;

            if (stepInSeconds < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can't speed up the truck by less than 1 seconds",
                    RussianMessage = "Нельзя ускорить грузовик на менее чем 1 секунду"
                };
            }

            decimal price = prices.Price * (stepInSeconds / prices.StepInSeconds);

            var reducingResult = TruckGarage.Company.Purse.Reduce(price, Currency.BusinessCoins);
            if (reducingResult.LocalizedError != null) return reducingResult.LocalizedError;

            return ForceUpgrade(stepInSeconds, truckSettings);
        }

        public GameResult<uint> ForceUpgrade(uint stepInSeconds, TruckSettings truckSettings)
        {
            if(DeliveringSpeedInSeconds - stepInSeconds <= truckSettings.MinTime)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Delivering speed is already at the maximum",
                    RussianMessage = "Скорость доставки уже на максимуме"
                };
            }

            DeliveringSpeedInSeconds -= stepInSeconds;

            return DeliveringSpeedInSeconds;
        }


        private static GameResult<Truck> CreateTruck(TruckCreationParams creationPars)
        {
            var cp = creationPars;

            var truck = new Truck
            {
                TruckGarage = cp.TruckGarage,
                DeliveringSpeedInSeconds = cp.DeliveringSpeedInSeconds,
                CurrentState = TruckState.ReadyToGo
            };

            var trunkCreationResult = TrunkStorage.Create(new TrunkStorageCreationParams
            {
                OwningTruck = truck,
                Capacity = cp.Capacity
            });
            if (trunkCreationResult.LocalizedError != null) return trunkCreationResult.LocalizedError;

            truck.Trunk = trunkCreationResult;

            return truck;
        }
        #endregion
    }
}
