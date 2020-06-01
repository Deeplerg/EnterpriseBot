using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Essences;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Essences;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Essences
{
    public class BotCategory : EssencesCategoryBase<Bot>,
                               ICreatableCategory<Bot, BotCreationParams>
    {
        private static readonly string categoryName = "bot";

        public BotCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Bot> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Bot>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        /// <inheritdoc/>
        /// <remarks>Automatically creates a task and occupies the job</remarks>
        public async Task<Bot> Create(BotCreationParams pars)
        {
            var result = await api.Call<Bot>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Buys a bot for a company and automatically hires it on the specified job
        /// </summary>
        /// <param name="pars">Parameters that determine the bot to be created</param>
        /// <returns>Bot instance after hiring</returns>
        public async Task<Bot> BuyBotAndHire(BotCreationParams pars)
        {
            var result = await api.Call<Bot>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(BuyBotAndHire).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Removes the bot
        /// </summary>
        /// <param name="botId">Id of the bot to be removed</param>
        public async Task Remove(long botId)
        {
            var pars = new
            {
                botId = botId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Remove).ToLower()
            }, pars);
        }
    }
}
