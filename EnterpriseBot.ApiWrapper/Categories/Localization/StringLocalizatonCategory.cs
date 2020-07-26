using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Localization;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Localization
{
    public class StringLocalizationCategory : LocalizationCategoryBase<StringLocalization,
                                                                       long,
                                                                       StringLocalizationCreationParams>
    {
        protected const string categoryName = "StringLocalization";

        public StringLocalizationCategory(IApiClient api) : base(api) { }

        public override async Task<StringLocalization> Get(long id)
        {
            return await api.Call<StringLocalization>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<StringLocalization> Create(StringLocalizationCreationParams pars)
        {
            return await api.Call<StringLocalization>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<string> SetText(long modelId, string newText)
        {
            return await api.Call<string>(RequestInfo(nameof(SetText)), new
            {
                modelId = modelId,
                newText = newText
            });
        }

        public async Task<LocalizationLanguage> SetLanguage(long modelId, LocalizationLanguage newLanguage)
        {
            return await api.Call<LocalizationLanguage>(RequestInfo(nameof(SetLanguage)), new
            {
                modelId = modelId,
                newLanguage = newLanguage
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
