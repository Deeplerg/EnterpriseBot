using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.ModelCreationParams.Donation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.BusinessSettings.Company;
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.ApiWrapper.Models.Common.Business;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Settings;

namespace EnterpriseBot.Api.Game.Donation
{
    public class Donation
    {
        protected Donation() { }

        #region model
        public long Id { get; protected set; }

        public Privilege Privilege { get; protected set; }

        public virtual IReadOnlyCollection<DonationPurchase> History
        {
            get => new ReadOnlyCollection<DonationPurchase>(history);
            protected set => history = value.ToList();
        }

        private List<DonationPurchase> history = new List<DonationPurchase>();

        public virtual Player Player { get; protected set; }

        public bool HasDonation
        {
            get => Privilege != Privilege.NoDonation;
        }
        #endregion

        #region actions
        public static GameResult<Donation> Create(DonationCreationParams pars)
        {
            return new Donation
            {
                Privilege = pars.Privilege,
                Player = pars.Player
            };
        }

        public static GameResult<decimal> GetBusinessPriceMultiplierForPrivelege(Privilege privilege, GameSettings gameSettings)
        {
            var donationSettings = gameSettings.Donation;

            switch(privilege)
            {
                case Privilege.NoDonation:
                    return donationSettings.BusinessPriceMultipliers.NoDonation;

                case Privilege.Pro:
                    return donationSettings.BusinessPriceMultipliers.Pro;

                case Privilege.VIP:
                    return donationSettings.BusinessPriceMultipliers.VIP;

                case Privilege.Premium:
                    return donationSettings.BusinessPriceMultipliers.Premium;

                case Privilege.Mega:
                    return donationSettings.BusinessPriceMultipliers.Mega;

                case Privilege.Gold:
                    return donationSettings.BusinessPriceMultipliers.Gold;

                default:
                    return Errors.UnknownEnumValue(privilege);
            }
        }

        public GameResult<decimal> GetBusinessPriceMultiplier(GameSettings gameSettings)
        {
            return GetBusinessPriceMultiplierForPrivelege(Privilege, gameSettings);
        }

        public static GameResult<uint> GetMaximumContractsForPrivelege(Privilege privilege, GameSettings gameSettings)
        {
            var donationSettings = gameSettings.Donation;
            var contractSettings = gameSettings.Business.Company.Contract;

            var max = donationSettings.MaxContracts;
            uint @default = contractSettings.MaxContracts;

            switch(privilege)
            {
                case Privilege.NoDonation:
                    return max.NoDonation ?? @default;

                case Privilege.Pro:
                    return max.Pro ?? @default;

                case Privilege.VIP:
                    return max.VIP ?? @default;

                case Privilege.Premium:
                    return max.Premium ?? @default;

                case Privilege.Mega:
                    return max.Mega ?? @default;

                case Privilege.Gold:
                    return max.Gold ?? @default;

                default:
                    return Errors.UnknownEnumValue(privilege);
            }
        }

        public GameResult<uint> GetMaximumContracts(GameSettings gameSettings)
        {
            return GetMaximumContractsForPrivelege(Privilege, gameSettings);
        }

        public static GameResult<uint> GetContractMaxTimeInDaysForPrivilege(Privilege privilege, GameSettings gameSettings)
        {
            var donationSettings = gameSettings.Donation;
            var contractSettings = gameSettings.Business.Company.Contract;

            var max = donationSettings.ContractMaxTimeInDays;
            uint @default = contractSettings.MaxTimeInDays;

            switch (privilege)
            {
                case Privilege.NoDonation:
                    return max.NoDonation ?? @default;

                case Privilege.Pro:
                    return max.Pro ?? @default;

                case Privilege.VIP:
                    return max.VIP ?? @default;

                case Privilege.Premium:
                    return max.Premium ?? @default;

                case Privilege.Mega:
                    return max.Mega ?? @default;

                case Privilege.Gold:
                    return max.Gold ?? @default;

                default:
                    return Errors.UnknownEnumValue(privilege);
            }
        }

        public GameResult<uint> GetContractMaxTimeInDays(GameSettings gameSettings)
        {
            return GetContractMaxTimeInDaysForPrivilege(Privilege, gameSettings);
        }

        public GameResult<Privilege> UpgradePrivilege(Privilege to)
        {
            if(!CanUpgradeToPrivilege(to))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can't upgrade to this privilege, as you already have this one",
                    RussianMessage = "Нельзя улучшить привилегию до этой, так как она уже есть у тебя"
                };
            }

            Privilege = to;

            return Privilege;
        }

        public GameResult<DonationPurchase> AddPurchase(DonationPurchaseCreationParams pars)
        {
            var purchaseCreationResult = DonationPurchase.Create(pars);
            if (purchaseCreationResult.LocalizedError != null) return purchaseCreationResult.LocalizedError;

            DonationPurchase purchase = purchaseCreationResult.Result;
            history.Add(purchase);

            return purchase;
        }

        public bool CanUpgradeToPrivilege(Privilege privilege)
        {
            if (privilege > Privilege) return true;
            else return false;
        }
        #endregion
    }
}
