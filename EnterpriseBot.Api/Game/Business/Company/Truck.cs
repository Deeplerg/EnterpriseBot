﻿using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Utils;
using System.Linq;
using System.Text.Json.Serialization;

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
            return CreateBase(creationPars);
        }

        public static EmptyGameResult Buy(Company company, GameSettings gameSettings, Player invoker)
        {
            var prices = gameSettings.BusinessPrices.CompanyFeatures;
            if (!invoker.HasPermission(CompanyJobPermissions.BuyTrucks, company))
            {
                return Errors.DoesNotHavePermission();
            }

            var reduceResult = company.ReduceBusinessCoins(prices.NewTruckSetup, gameSettings, invoker);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return new EmptyGameResult();
        }

        public EmptyGameResult Send(CompanyContract contract, Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.SendTrucks, contract.OutcomeCompany))
            {
                return Errors.DoesNotHavePermission();
            }

            CurrentState = TruckState.OnTheWayTowards;

            return new EmptyGameResult();
        }

        public EmptyGameResult Return()
        {
            CurrentState = TruckState.ReadyToGo;

            return new EmptyGameResult();
        }

        public EmptyGameResult Unload(CompanyStorage storage, CompanyContract contract)
        {
            if (storage.Type != CompanyStorageType.Income)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Storage type is {storage.Type}, but expected: {CompanyStorageType.Income}",
                    RussianMessage = $"Тип хранилища - {storage.Type}, но ожидалось: {CompanyStorageType.Income}"
                };
            }

            if (CurrentState != TruckState.OnTheWayTowards)
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

            if (Trunk.Items != null)
            {
                neededItemsAmountInTrunk = Trunk.Items.Sum(sItem => sItem.Item == contract.ContractItem ? sItem.Quantity : 0);
            }

            if (neededItemsAmountInTrunk == 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The truck does not have any items requested by a contract",
                    RussianMessage = "В грузовике нет предметов, запрошенных контрактом"
                };
            }

            if (needed > neededItemsAmountInTrunk)
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

        public GameResult<uint> Upgrade(GameSettings gameSettings, Player invoker)
        {
            var upgradeSetting = gameSettings.BusinessPrices.CompanyFeatures.TruckUpgrade;

            if (!invoker.HasPermission(CompanyJobPermissions.UpgradeTrucks, TruckGarage.Company))
            {
                return Errors.DoesNotHavePermission();
            }

            return Upgrade(upgradeSetting.StepInSeconds, gameSettings);
        }

        public GameResult<uint> Upgrade(uint stepInSeconds, GameSettings gameSettings, Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.UpgradeTrucks, TruckGarage.Company))
            {
                return Errors.DoesNotHavePermission();
            }

            return Upgrade(stepInSeconds, gameSettings);
        }

        public GameResult<uint> Upgrade(uint stepInSeconds, GameSettings gameSettings)
        {
            var prices = gameSettings.BusinessPrices.CompanyFeatures.TruckUpgrade;

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

            return ForceUpgrade(stepInSeconds, gameSettings);
        }

        public GameResult<uint> ForceUpgrade(uint stepInSeconds, GameSettings gameSettings)
        {
            if (DeliveringSpeedInSeconds - stepInSeconds <= gameSettings.Business.Company.Truck.MinTime)
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

        public GameResult<decimal> GetSpaceOccupiedByItemsForContract(CompanyContract contract)
        {
            return Trunk.Items.Where(sItem => sItem.Item == contract.ContractItem).Sum(sItem => sItem.Space);
        }


        private static GameResult<Truck> CreateBase(TruckCreationParams creationPars)
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
