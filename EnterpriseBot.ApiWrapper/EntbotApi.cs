using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Categories.Business.Company;
using EnterpriseBot.ApiWrapper.Categories.Crafting;
using EnterpriseBot.ApiWrapper.Categories.Donation;
using EnterpriseBot.ApiWrapper.Categories.Essences;
using EnterpriseBot.ApiWrapper.Categories.Localization;
using EnterpriseBot.ApiWrapper.Categories.Money;
using EnterpriseBot.ApiWrapper.Categories.Reputation;
using EnterpriseBot.ApiWrapper.Categories.Storages;
using EnterpriseBot.ApiWrapper.Extensions;
using EnterpriseBot.ApiWrapper.Models.Other;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EnterpriseBot.ApiWrapper
{
    public class EntbotApi : IEntbotApi
    {
        private const CurrentLocalization DefaultLocalization = CurrentLocalization.English;

        private readonly Uri apiUri;
        private readonly IServiceProvider serviceProvider;
        private readonly CurrentLocalization currentLocalization;

        public CategoryLists.BusinessCategoryList Business { get; private set; }
        public CategoryLists.CraftingCategoryList Crafting { get; private set; }
        public CategoryLists.DonationCategoryList Donation { get; private set; }
        public CategoryLists.EssencesCategoryList Essences { get; private set; }
        public CategoryLists.LocalizationCategoryList Localization { get; private set; }
        public CategoryLists.MoneyCategoryList Money { get; private set; }
        public CategoryLists.ReputationCategoryList Reputation { get; private set; }
        public CategoryLists.StoragesCategoryList Storages { get; private set; }

        public EntbotApi(string apiUri, ushort? port = null, CurrentLocalization localization = DefaultLocalization)
        {
            if (port != null)
            {
                UriBuilder uriBuilder = new UriBuilder(new Uri(apiUri))
                {
                    Port = (int)port
                };
                this.apiUri = uriBuilder.Uri;
            }
            else
                this.apiUri = new Uri(apiUri);

            this.currentLocalization = localization;

            var services = new ServiceCollection();
            InitContainer(services);
            serviceProvider = services.BuildServiceProvider();

            InitCategoryLists(serviceProvider);
        }

        public EntbotApi(Uri apiUri, ushort? port = null, CurrentLocalization localization = DefaultLocalization)
        {
            if (port != null)
            {
                UriBuilder uriBuilder = new UriBuilder(apiUri)
                {
                    Port = (int)port
                };
                this.apiUri = uriBuilder.Uri;
            }
            else
                this.apiUri = apiUri;

            this.currentLocalization = localization;

            var services = new ServiceCollection();
            InitContainer(services);
            serviceProvider = services.BuildServiceProvider();

            InitCategoryLists(serviceProvider);
        }

        #region initialization methods
        private IServiceCollection InitContainer(IServiceCollection services)
        {
            services.AddApiClient(apiUri, currentLocalization);

            return services;
        }

        private void InitCategoryLists(IServiceProvider serviceProvider)
        {
            #region CategoryList init methods
            CategoryLists.BusinessCategoryList GetBusiness()
            {
                return new CategoryLists.BusinessCategoryList
                {
                    Company = new CategoryLists.CompanySubCategoryList
                    {
                        Company = ActivatorUtilities.GetServiceOrCreateInstance<CompanyCategory>(serviceProvider),
                        CompanyContract = ActivatorUtilities.GetServiceOrCreateInstance<CompanyContractCategory>(serviceProvider),
                        CompanyContractRequest = ActivatorUtilities.GetServiceOrCreateInstance<CompanyContractRequestCategory>(serviceProvider),
                        CompanyJobApplication = ActivatorUtilities.GetServiceOrCreateInstance<CompanyJobApplicationCategory>(serviceProvider),
                        CompanyJob = ActivatorUtilities.GetServiceOrCreateInstance<CompanyJobCategory>(serviceProvider),

#pragma warning disable CS0618 // Type or member is obsolete
                        CompanyWorker = ActivatorUtilities.GetServiceOrCreateInstance<CompanyWorkerCategory>(serviceProvider),
#pragma warning restore CS0618 // Type or member is obsolete

                        ProductionRobot = ActivatorUtilities.GetServiceOrCreateInstance<ProductionRobotCategory>(serviceProvider),
                        Truck = ActivatorUtilities.GetServiceOrCreateInstance<TruckCategory>(serviceProvider),
                        TruckGarage = ActivatorUtilities.GetServiceOrCreateInstance<TruckGarageCategory>(serviceProvider),
                    }
                };
            }

            CategoryLists.CraftingCategoryList GetCrafting()
            {
                return new CategoryLists.CraftingCategoryList
                {
                    CraftingCategory = ActivatorUtilities.GetServiceOrCreateInstance<CraftingCategoryCategory>(serviceProvider),
                    CraftingSubCategory = ActivatorUtilities.GetServiceOrCreateInstance<CraftingSubCategoryCategory>(serviceProvider),
                    Item = ActivatorUtilities.GetServiceOrCreateInstance<ItemCategory>(serviceProvider),
                    Recipe = ActivatorUtilities.GetServiceOrCreateInstance<RecipeCategory>(serviceProvider),
                    Ingredient = ActivatorUtilities.GetServiceOrCreateInstance<IngredientCategory>(serviceProvider)
                };
            }

            CategoryLists.DonationCategoryList GetDonation()
            {
                return new CategoryLists.DonationCategoryList
                {
                    Donation = ActivatorUtilities.GetServiceOrCreateInstance<DonationCategory>(serviceProvider),
                    DonationPurchase = ActivatorUtilities.GetServiceOrCreateInstance<DonationPurchaseCategory>(serviceProvider),
                };
            }

            CategoryLists.EssencesCategoryList GetEssences()
            {
                return new CategoryLists.EssencesCategoryList
                {
                    Player = ActivatorUtilities.GetServiceOrCreateInstance<PlayerCategory>(serviceProvider)
                };
            }

            CategoryLists.LocalizationCategoryList GetLocalization()
            {
                return new CategoryLists.LocalizationCategoryList
                {
                    LocalizedString = ActivatorUtilities.GetServiceOrCreateInstance<LocalizedStringCategory>(serviceProvider),
                    StringLocalization = ActivatorUtilities.GetServiceOrCreateInstance<StringLocalizationCategory>(serviceProvider)
                };
            }

            CategoryLists.MoneyCategoryList GetMoney()
            {
                return new CategoryLists.MoneyCategoryList
                {
                    Purse = ActivatorUtilities.GetServiceOrCreateInstance<PurseCategory>(serviceProvider),
                };
            }

            CategoryLists.ReputationCategoryList GetReputation()
            {
                return new CategoryLists.ReputationCategoryList
                {
                    Reputation = ActivatorUtilities.GetServiceOrCreateInstance<ReputationCategory>(serviceProvider),
                    Review = ActivatorUtilities.GetServiceOrCreateInstance<ReviewCategory>(serviceProvider)
                };
            }

            CategoryLists.StoragesCategoryList GetStorages()
            {
                return new CategoryLists.StoragesCategoryList
                {
                    CompanyStorage = ActivatorUtilities.GetServiceOrCreateInstance<CompanyStorageCategory>(serviceProvider),
                    InventoryStorage = ActivatorUtilities.GetServiceOrCreateInstance<InventoryStorageCategory>(serviceProvider),
                    ItemPrice = ActivatorUtilities.GetServiceOrCreateInstance<ItemPriceCategory>(serviceProvider),
                    ShowcaseStorage = ActivatorUtilities.GetServiceOrCreateInstance<ShowcaseStorageCategory>(serviceProvider),
                    Storage = ActivatorUtilities.GetServiceOrCreateInstance<StorageCategory>(serviceProvider),
                    StorageItem = ActivatorUtilities.GetServiceOrCreateInstance<StorageItemCategory>(serviceProvider),
                    TrunkStorage = ActivatorUtilities.GetServiceOrCreateInstance<TrunkStorageCategory>(serviceProvider),
                };
            }
            #endregion

            Business = GetBusiness();
            Crafting = GetCrafting();
            Donation = GetDonation();
            Essences = GetEssences();
            Localization = GetLocalization();
            Money = GetMoney();
            Reputation = GetReputation();
            Storages = GetStorages();
        }
        #endregion
    }
}
