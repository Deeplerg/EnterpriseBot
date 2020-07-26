using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Essences;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Essences;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Essences
{
    public class PlayerCategory : EssencesCategoryBase<Player,
                                                       long,
                                                       PlayerCreationParams>
    {
        protected const string categoryName = "Player";

        public PlayerCategory(IApiClient api) : base(api) { }

        public override async Task<Player> Get(long id)
        {
            return await api.Call<Player>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Player> Create(PlayerCreationParams pars)
        {
            return await api.Call<Player>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<bool> HasPermission(long modelId, CompanyJobPermissions permission, long companyId)
        {
            return await api.Call<bool>(RequestInfo(nameof(HasPermission)), new
            {
                modelId = modelId,
                permission = permission,
                companyId = companyId
            });
        }

        public async Task<string> SetName(long modelId, string newName)
        {
            return await api.Call<string>(RequestInfo(nameof(SetName)), new
            {
                modelId = modelId,
                newName = newName
            });
        }

        public async Task<StringLocalization> EditAbout(long modelId, string newAbout, LocalizationLanguage localizationLanguage)
        {
            return await api.Call<StringLocalization>(RequestInfo(nameof(EditAbout)), new
            {
                modelId = modelId,
                newAbout = newAbout,
                language = localizationLanguage
            });
        }

        public async Task<StringLocalization> EditStatus(long modelId, string newStatus, LocalizationLanguage localizationLanguage)
        {
            return await api.Call<StringLocalization>(RequestInfo(nameof(EditStatus)), new
            {
                modelId = modelId,
                newStatus = newStatus,
                language = localizationLanguage
            });
        }

        public async Task ChangePassword(long modelId, string newRawPassword)
        {
            await api.Call(RequestInfo(nameof(ChangePassword)), new
            {
                modelId = modelId,
                newPassword = newRawPassword
            });
        }

        public async Task<bool> VerifyPassword(long modelId, string rawPassword)
        {
            return await api.Call<bool>(RequestInfo(nameof(VerifyPassword)), new
            {
                modelId = modelId,
                password = rawPassword
            });
        }

        public async Task<long> LinkVk(long modelId, long vkId)
        {
            return await api.Call<long>(RequestInfo(nameof(LinkVk)), new
            {
                modelId = modelId,
                vkId = vkId
            });
        }

        public async Task UnlinkVk(long modelId)
        {
            await api.Call(RequestInfo(nameof(UnlinkVk)), new
            {
                modelId = modelId
            });
        }

        public async Task<IEnumerable<Player>> SearchByName(string name)
        {
            return await api.Call<IEnumerable<Player>>(RequestInfo(nameof(SearchByName)), new
            {
                name = name
            });
        }

        public async Task<Player> GetByVK(long vkId)
        {
            return await GetByPlatform(BotPlatform.VK, vkId.ToString());
        }


        private async Task<Player> GetByPlatform(BotPlatform platform, string idOnPlatform)
        {
            return await api.Call<Player>(RequestInfo(nameof(GetByPlatform)), new
            {
                platform = platform,
                id = idOnPlatform
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
