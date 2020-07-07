using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings.Company;
using EnterpriseBot.Api.Models.Settings.BusinessSettings.Company;
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class TruckGarage
    {
        protected TruckGarage() { }

        #region model
        public long Id { get; protected set; }

        public virtual Company Company { get; protected set; }

        public sbyte Capacity { get; protected set; }

        public virtual IReadOnlyCollection<Truck> Trucks 
        {
            get => new ReadOnlyCollection<Truck>(trucks);
            protected set => trucks = value.ToList(); 
        }
        private List<Truck> trucks = new List<Truck>();
        #endregion

        #region actions
        public static GameResult<TruckGarage> Create(TruckGarageCreationParams creationPars, GameSettings gameSettings)
        {
            var truckGarageSettings = gameSettings.Business.Company.TruckGarage;

            if(creationPars.Capacity > truckGarageSettings.MaxCapacity)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Truck garage capacity can't be larger than {truckGarageSettings.MaxCapacity}",
                    RussianMessage = $"Вместимость гаража для грузовиков не может быть больше, чем {truckGarageSettings.MaxCapacity}"
                };
            }

            if(creationPars.Capacity < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal, // Critical?
                    EnglishMessage = "Truck garage capacity can't be lower than 1",
                    RussianMessage = "Вместимость гаража грузовиков не может быть меньше, чем 1"
                };
            }

            return new TruckGarage
            {
                Company = creationPars.OwningCompany,
                Capacity = creationPars.Capacity
            };
        }

        public GameResult<Truck> AddTruck(TruckCreationParams pars)
        {
            if(trucks.Count + 1 > Capacity)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The garage is full",
                    RussianMessage = "Гараж полон"
                };
            }

            return Truck.Create(pars);
        }

        public GameResult<Truck> BuyAndAddTruck(TruckCreationParams pars, GameSettings gameSettings, Player invoker)
        {
            var buyResult = Truck.Buy(pars, gameSettings, invoker);
            if (buyResult.LocalizedError != null) return buyResult.LocalizedError;

            return AddTruck(pars);
        }

        public GameResult<sbyte> Upgrade(GameSettings gameSettings, Player invoker)
        {
            var upgradePrice = gameSettings.BusinessPrices.CompanyFeatures.GarageUpgrade;

            if(!invoker.HasPermission(CompanyJobPermissions.UpgradeTruckGarage, Company))
            {
                return Errors.DoesNotHavePermission();
            }

            var reduceResult = Company.ReduceBusinessCoins(upgradePrice.Price, gameSettings, invoker);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return ForceUpgrade(gameSettings, upgradePrice.StepInSlots);
        }

        public GameResult<sbyte> ForceUpgrade(GameSettings gameSettings, sbyte step)
        {
            if(Capacity + step >= gameSettings.Business.Company.TruckGarage.MaxCapacity)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can't upgrade garage capacity, as it is already at the maximum",
                    RussianMessage = "Нельзя улучшить вместимость гаража, так как она уже на максимуме"
                };
            }

            Capacity += step;

            return Capacity;
        }
        #endregion
    }
}
