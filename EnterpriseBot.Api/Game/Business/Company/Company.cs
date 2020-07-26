using EnterpriseBot.Api.Game.Donation;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Game.Money;
using EnterpriseBot.Api.Game.Reputation;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.UserInputUtils;
using static EnterpriseBot.Api.Utils.Constants;
using EnterpriseBot.Api.Models.ModelCreationParams.Money;
using EnterpriseBot.Api.Models.Settings.BusinessSettings.Company;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings.Company;
using EnterpriseBot.Api.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using EnterpriseBot.Api.Game.Localization;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using EnterpriseBot.Api.Models.ModelCreationParams.Reputation;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class Company
    {
        protected Company() { }
        
        #region model
        public long Id { get; protected set; }

        public string Name { get; protected set; }
        public virtual LocalizedString Description { get; protected set; }

        public virtual Player Owner { get; protected set; }

        public virtual Purse Purse { get; protected set; }
        public virtual Reputation.Reputation Reputation { get; protected set; }

        public virtual TruckGarage TruckGarage { get; protected set; }

        public uint ContractsCompleted { get; protected set; }

        public CompanyExtensions Extensions { get; protected set; }


        public virtual IReadOnlyCollection<CompanyJob> Jobs
        {
            get => new ReadOnlyCollection<CompanyJob>(jobs);
            protected set => jobs = value.ToList();
        }
        public virtual IReadOnlyCollection<ProductionRobot> Robots
        {
            get => new ReadOnlyCollection<ProductionRobot>(robots);
            protected set => robots = value.ToList();
        }
        public virtual IReadOnlyCollection<CompanyContract> IncomeContracts
        {
            get => new ReadOnlyCollection<CompanyContract>(incomeContracts);
            protected set => incomeContracts = value.ToList();
        }
        public virtual IReadOnlyCollection<CompanyContract> OutcomeContracts
        {
            get => new ReadOnlyCollection<CompanyContract>(outcomeContracts);
            protected set => outcomeContracts = value.ToList();
        }
        public virtual IReadOnlyCollection<CompanyContractRequest> SentContractRequests
        {
            get => new ReadOnlyCollection<CompanyContractRequest>(sentContractRequests);
            protected set => sentContractRequests = value.ToList();
        }
        public virtual IReadOnlyCollection<CompanyContractRequest> InboxContractRequests
        {
            get => new ReadOnlyCollection<CompanyContractRequest>(inboxContractRequests);
            protected set => inboxContractRequests = value.ToList();
        }
        public virtual IReadOnlyCollection<CompanyStorage> Storages
        {
            get => new ReadOnlyCollection<CompanyStorage>(storages);
            protected set => storages = value.ToList();
        }
        public virtual IReadOnlyCollection<ShowcaseStorage> Showcases
        {
            get => new ReadOnlyCollection<ShowcaseStorage>(showcases);
            protected set => showcases = value.ToList();
        }


        private List<CompanyJob> jobs = new List<CompanyJob>();
        private List<ProductionRobot> robots = new List<ProductionRobot>();
        private List<CompanyContract> incomeContracts = new List<CompanyContract>();
        private List<CompanyContract> outcomeContracts = new List<CompanyContract>();
        private List<CompanyContractRequest> sentContractRequests = new List<CompanyContractRequest>();
        private List<CompanyContractRequest> inboxContractRequests = new List<CompanyContractRequest>();
        private List<CompanyStorage> storages = new List<CompanyStorage>();
        private List<ShowcaseStorage> showcases = new List<ShowcaseStorage>();
        #endregion

        #region actions
        public static GameResult<Company> Create(CompanyCreationParams pars, GameSettings gameSettings)
        {
            if (pars.Owner.Donation.Privilege == Privilege.Gold)
            {
                pars.Extensions |= CompanyExtensions.CanRobotsWorkInfinitely;
            }

            return CreateBase(pars, gameSettings);
        }

        public static EmptyGameResult Buy(CompanyExtensions exts, GameSettings gameSettings, Player owner)
        {
            //in business coins
            var overallPrice = GetOverallCreationPrice(exts, gameSettings, owner);
            if (overallPrice.LocalizedError != null) return overallPrice.LocalizedError;

            decimal currentBusinessCoinsQuantity = owner.Purse.GetMoneyAmount(Currency.BusinessCoins);
            if (currentBusinessCoinsQuantity < overallPrice)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,

                    EnglishMessage = $"Not enough money. You currently have: {currentBusinessCoinsQuantity}bc, " +
                                     $"but with options selected you need: {overallPrice}bc",
                    RussianMessage = $"Недостаточно денег. У тебя сейчас: {currentBusinessCoinsQuantity}bc," +
                                     $"но с выбранными параметрами необходимо: {overallPrice}bc"
                };
            }

            var reduceResult = owner.Purse.Reduce(overallPrice, Currency.BusinessCoins);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return new EmptyGameResult();
        }

        public static GameResult<decimal> GetOverallCreationPrice(CompanyExtensions extensions, GameSettings gameSettings, Player owner)
        {
            var priceMultiplier = Donation.Donation.GetBusinessPriceMultiplierForPrivelege(owner.Donation.Privilege, gameSettings);
            if (priceMultiplier.LocalizedError != null) return priceMultiplier.LocalizedError;

            var p = gameSettings.BusinessPrices.CompanyFeatures; //'p' stands for 'prices'

            decimal overallPrice = 0;

            // https://stackoverflow.com/a/1040826

            var extensionsWithPrices = new Dictionary<CompanyExtensions, decimal>
            {
                { CompanyExtensions.CanSignContracts,          p.CanSignContracts },
                { CompanyExtensions.CanUpgradeStorages,        p.CanUpgradeStorages },
                { CompanyExtensions.CanBuyStorages,            p.CanBuyStorages },
                { CompanyExtensions.CanHaveShowcase,           p.CanHaveShowcase },
                { CompanyExtensions.CanHire,                   p.CanHire },
                { CompanyExtensions.CanHaveTruckGarage,        p.CanHaveTruckGarage },
                { CompanyExtensions.CanExtendTruckGarage,      p.CanExtendTruckGarage },
                { CompanyExtensions.CanHaveRobots,             p.CanHaveRobots },
                { CompanyExtensions.CanExtendRobots,           p.CanExtendRobots },
                { CompanyExtensions.CanUpgradeRobots,          p.CanUpgradeRobots }
            };

            // All enum values
            var values = (IEnumerable<CompanyExtensions>)Enum.GetValues(typeof(CompanyExtensions));
            // All currently selected values
            var exts = values.Where(ext => (extensions & ext) == ext);

            foreach(CompanyExtensions ext in exts)
            {
                if (extensionsWithPrices.ContainsKey(ext))
                    overallPrice += (extensionsWithPrices[ext] * priceMultiplier);
            }

            overallPrice += p.Base;
            if((extensions & CompanyExtensions.CanHaveRobots) == CompanyExtensions.CanHaveRobots)
            {
                overallPrice += p.NewRobotSetup;
            }

            return overallPrice;
        } 

        public GameResult<CompanyStorage> GetCompanyStorageWithAvailableSpace(decimal space, CompanyStorageType storageType)
        {
            bool hasStorage = HasCompanyStorageWithAvailableSpace(space, storageType);

            if(!hasStorage)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"The company does not have a storage with enough space. Space needed: {space}",
                    RussianMessage = $"У компании нет хранилища, у которого достаточно свободного места. Необходимо: {space}"
                };
            }

            return Storages.First(s => s.AvailableSpace >= space && s.Type == storageType); // Or use FirstOrDefault instead of HasStorage to not waste database queries?
        }

        public bool HasCompanyStorageWithAvailableSpace(decimal space, CompanyStorageType storageType)
        {
            return Storages.Any(s => s.AvailableSpace >= space && s.Type == storageType);
        }

        public GameResult<decimal> ReduceBusinessCoins(decimal amount, GameSettings gameSettings, Player invoker = null)
        {
            var ownerMultiplierResult = Owner.Donation.GetBusinessPriceMultiplier(gameSettings);
            if (ownerMultiplierResult.LocalizedError != null) return ownerMultiplierResult.LocalizedError;

            decimal priceMultiplier = ownerMultiplierResult;

            if (invoker != null && invoker.HasDonation)
            {
                var invokerMultiplierResult = invoker.Donation.GetBusinessPriceMultiplier(gameSettings);
                if (invokerMultiplierResult.LocalizedError != null) return invokerMultiplierResult.LocalizedError;

                decimal employeeMultiplier = invokerMultiplierResult;

                if (employeeMultiplier < priceMultiplier)
                    priceMultiplier = employeeMultiplier;
            }

            return Purse.Reduce(amount * priceMultiplier, Currency.BusinessCoins);
        }

        public GameResult<StringLocalization> EditDescription(string newDescription, LocalizationLanguage language, GameSettings gameSettings)
        {
            if (!CheckDescription(newDescription))
            {
                return Errors.IncorrectDescriptionInput(gameSettings.Localization.UserInputRequirements);
            }

            var editResult = Description.Edit(newDescription, language);
            if (editResult.LocalizedError != null) return editResult.LocalizedError;

            return editResult;
        }

        public EmptyGameResult SetOwner(Player newOwner)
        {
            if (Owner == newOwner)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can’t transfer company ownership to yourself",
                    RussianMessage = "Нельзя передать компанию себе самому"
                };
            }

            Owner = newOwner;

            return new EmptyGameResult();
        }

        public EmptyGameResult CompleteAndRemoveContract(CompanyContract contract)
        {
            bool completed = incomeContracts.Remove(contract) ? true
                           : outcomeContracts.Remove(contract) ? true
                           : false;

            if (!completed)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Contract {contract.Id} not found in company {Id}",
                    RussianMessage = $"Контракт {contract.Id} не найден в компании {Id}"
                };
            }

            ContractsCompleted++;

            return new EmptyGameResult();
        }

        public GameResult<bool> CanConcludeOneMoreContract(GameSettings gameSettings, Player invoker = null)
        {
            var maxContractsOwnerResult = Owner.Donation.GetMaximumContracts(gameSettings);
            if (maxContractsOwnerResult.LocalizedError != null) return maxContractsOwnerResult.LocalizedError;

            uint maxContracts = maxContractsOwnerResult;

            if (invoker != null && invoker.HasDonation)
            {
                var maxContractsInvokerResult = invoker.Donation.GetMaximumContracts(gameSettings);
                if (maxContractsInvokerResult.LocalizedError != null) return maxContractsInvokerResult.LocalizedError;

                if (maxContractsInvokerResult > maxContracts)
                {
                    maxContracts = maxContractsInvokerResult;
                }
            }

            if(IncomeContracts.Count() + OutcomeContracts.Count() >= maxContracts)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public GameResult<uint> GetContractMaxTimeInDays(GameSettings gameSettings, Player invoker = null)
        {
            var maxTimeOwnerResult = Owner.Donation.GetContractMaxTimeInDays(gameSettings);
            if (maxTimeOwnerResult.LocalizedError != null) return maxTimeOwnerResult.LocalizedError;

            uint maxTime = maxTimeOwnerResult;

            if (invoker != null && invoker.HasDonation)
            {
                var maxTimeInvokerResult = invoker.Donation.GetContractMaxTimeInDays(gameSettings);
                if (maxTimeInvokerResult.LocalizedError != null) return maxTimeInvokerResult.LocalizedError;

                if (maxTimeInvokerResult > maxTime)
                {
                    maxTime = maxTimeInvokerResult;
                }
            }

            return maxTime;
        }

        public GameResult<Review> WriteReview(Reputation.Reputation reputation, string text, sbyte rating, GameSettings gameSettings, Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.WriteReview, this))
            {
                return Errors.DoesNotHavePermission();
            }

            return reputation.AddReview(new ReviewCreationParams
            {
                Reviewer = Reviewer.Company,
                CompanyReviewer = this,
                Text = text,
                Rating = rating
            }, gameSettings);
        }

        public GameResult<Review> EditReview(Review review, string newText, sbyte newRating, GameSettings gameSettings, Player invoker)
        {
            if(invoker.HasPermission(CompanyJobPermissions.EditReview, this))
            {
                return Errors.DoesNotHavePermission();
            }

            if(review.CompanyReviewer != this)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Reviewer of review {review.Id} is not Company {this.Id}",
                    RussianMessage = $"Рецензент отзыва {review.Id} - не компания {this.Id}"
                };
            }

            review.Edit(newText, newRating, gameSettings);

            return review;
        }


        private static GameResult<Company> CreateBase(CompanyCreationParams pars, GameSettings gameSettings)
        {
            var p = pars;
            var e = p.Extensions;

            var req = gameSettings.Localization.UserInputRequirements;
            var companySettings = gameSettings.Business.Company;

            var owner = pars.Owner;

            if (!CheckBusinessName(p.Name))
            {
                return Errors.IncorrectBusinessNameInput(req);
            }

            foreach (var str in pars.Description.Localizations)
            {
                if (!CheckDescription(str.Text))
                {
                    return Errors.IncorrectDescriptionInput(req);
                }
            }


            var purseCreationResult = Purse.Create(new PurseCreationParams
            {
                UnitsAmount = companySettings.DefaultUnits,
                BusinessCoinsAmount = companySettings.DefaultBusinessCoins
            });
            if (purseCreationResult.LocalizedError != null) return purseCreationResult.LocalizedError;

            var reputationCreationResult = Game.Reputation.Reputation.Create(new ReputationCreationParams
            {
            });
            if (reputationCreationResult.LocalizedError != null) return reputationCreationResult.LocalizedError;


            var company = new Company
            {
                Name = p.Name,
                Description = p.Description,

                Owner = owner,
                Purse = purseCreationResult,

                Reputation = reputationCreationResult,

                Robots = new List<ProductionRobot>(),

                Extensions = p.Extensions
            };

            return company;
        }
        #endregion
    }
}
