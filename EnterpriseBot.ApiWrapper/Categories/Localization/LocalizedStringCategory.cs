using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Localization;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Localization
{
    public class LocalizedStringCategory : LocalizationCategoryBase<LocalizedString,
                                                                    long,
                                                                    LocalizedStringCreationParams>
    {
        protected const string categoryName = "LocalizedString";

        public LocalizedStringCategory(IApiClient api) : base(api) { }

        public override async Task<LocalizedString> Get(long id)
        {
            return await api.Call<LocalizedString>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<LocalizedString> Create(LocalizedStringCreationParams pars)
        {
            return await api.Call<LocalizedString>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<IEnumerable<StringLocalization>> AddLocalization(long modelId, StringLocalizationCreationParams localizationParams)
        {
            return await api.Call<IEnumerable<StringLocalization>>(RequestInfo(nameof(AddLocalization)), new
            {
                modelId = modelId,
                localizationParams = localizationParams
            });
        }

        public async Task<StringLocalization> Edit(long modelId, string newText, LocalizationLanguage language)
        {
            var pars = new
            {
                modelId = modelId,
                newText = newText,
                language = language
            };

            return await api.Call<StringLocalization>(RequestInfo(nameof(Edit)), pars);
        }

        public async Task<StringLocalization> GetLocalization(long modelId, LocalizationLanguage language)
        {
            return await api.Call<StringLocalization>(RequestInfo(nameof(GetLocalization)), new
            {
                modelId = modelId,
                language = language
            });
        }

        public async Task<bool> IsLocalizationPresent(long modelId, LocalizationLanguage language)
        {
            return await api.Call<bool>(RequestInfo(nameof(IsLocalizationPresent)), new
            {
                modelId = modelId,
                language = language
            });
        }


        private ApiRequestInfo RequestInfo(string methodName)
        {
            return new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = methodName
            };
        }

        private object IdParameter(long id)
        {
            return new
            {
                id = id
            };
        }
    }
}
