using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Categories.Business;
using EnterpriseBot.ApiWrapper.Categories.Crafting;
using EnterpriseBot.ApiWrapper.Categories.Essences;
using EnterpriseBot.ApiWrapper.Categories.Storages;
using EnterpriseBot.ApiWrapper.CategoryLists;
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

        public BusinessCategoryList Business { get; private set; }
        public CraftingCategoryList Crafting { get; private set; }
        public EssencesCategoryList Essences { get; private set; }
        public StoragesCategoryList Storages { get; private set; }

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
            BusinessCategoryList GetBusinessCtgList()
            {
                return new BusinessCategoryList
                {
                    Company = ActivatorUtilities.GetServiceOrCreateInstance<CompanyCategory>(serviceProvider),
                    Contract = ActivatorUtilities.GetServiceOrCreateInstance<ContractCategory>(serviceProvider),
                    Job = ActivatorUtilities.GetServiceOrCreateInstance<JobCategory>(serviceProvider),
                    Shop = ActivatorUtilities.GetServiceOrCreateInstance<ShopCategory>(serviceProvider)
                };
            }

            CraftingCategoryList GetCraftingCtgList()
            {
                return new CraftingCategoryList
                {
                    CraftingCategory = ActivatorUtilities.GetServiceOrCreateInstance<CraftingCategoryCategory>(serviceProvider),
                    Item = ActivatorUtilities.GetServiceOrCreateInstance<ItemCategory>(serviceProvider),
                    Recipe = ActivatorUtilities.GetServiceOrCreateInstance<RecipeCategory>(serviceProvider),
                    Ingredient = ActivatorUtilities.GetServiceOrCreateInstance<IngredientCategory>(serviceProvider)
                };
            }

            EssencesCategoryList GetEssencesCtgList()
            {
                return new EssencesCategoryList
                {
                    Player = ActivatorUtilities.GetServiceOrCreateInstance<PlayerCategory>(serviceProvider),
                    Bot = ActivatorUtilities.GetServiceOrCreateInstance<BotCategory>(serviceProvider)
                };
            }

            StoragesCategoryList GetStoragesCtgList()
            {
                return new StoragesCategoryList
                {
                    IncomeStorage = ActivatorUtilities.GetServiceOrCreateInstance<IncomeStorageCategory>(serviceProvider),
                    OutcomeStorage = ActivatorUtilities.GetServiceOrCreateInstance<OutcomeStorageCategory>(serviceProvider),
                    WorkerStorage = ActivatorUtilities.GetServiceOrCreateInstance<WorkerStorageCategory>(serviceProvider),
                    ShowcaseStorage = ActivatorUtilities.GetServiceOrCreateInstance<ShowcaseStorageCategory>(serviceProvider),
                    TrunkStorage = ActivatorUtilities.GetServiceOrCreateInstance<TrunkStorageCategory>(serviceProvider),
                    PersonalStorage = ActivatorUtilities.GetServiceOrCreateInstance<PersonalStorageCategory>(serviceProvider)
                };
            }
            #endregion

            Business = GetBusinessCtgList();
            Crafting = GetCraftingCtgList();
            Essences = GetEssencesCtgList();
            Storages = GetStoragesCtgList();
        }
        #endregion
    }
}
