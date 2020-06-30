using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.ModelCreationParams.Donation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using EnterpriseBot.Api.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        public static GameResult<decimal> GetBusinessPriceMultiplierForPrivelege(Privilege privilege,
            DonationSettings donationSettings)
        {
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

        public GameResult<decimal> GetBusinessPriceMultiplier(DonationSettings donationSettings)
        {
            return GetBusinessPriceMultiplierForPrivelege(Privilege, donationSettings);
        }

        //public GameResult<Privilege> MakePurchase(Privilege privilege, DonationPurchaseCreationParams pars)
        //{
            
        //}

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
