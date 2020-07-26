using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Money;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Money;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Money
{
    public class PurseCategory : MoneyCategoryBase<Purse,
                                                   long,
                                                   PurseCreationParams>
    {
        protected const string categoryName = "Purse";

        public PurseCategory(IApiClient api) : base(api) { }

        public override async Task<Purse> Get(long id)
        {
            return await api.Call<Purse>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Purse> Create(PurseCreationParams pars)
        {
            return await api.Call<Purse>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<decimal> Add(long modelId, decimal amount, Currency currency)
        {
            return await api.Call<decimal>(RequestInfo(nameof(Add)), new
            {
                modelId = modelId,
                amount = amount,
                currency = currency
            });
        }

        public async Task<decimal> Reduce(long modelId, decimal amount, Currency currency)
        {
            return await api.Call<decimal>(RequestInfo(nameof(Reduce)), new
            {
                modelId = modelId,
                amount = amount,
                currency = currency
            });
        }

        public async Task TransferTo(long modelId, long receivingPurseId, decimal amount, Currency currency)
        {
            await api.Call(RequestInfo(nameof(TransferTo)), new
            {
                modelId = modelId,
                receivingPurseId = receivingPurseId,
                amount = amount,
                currency = currency,
            });
        }

        public async Task BuyBusinessCoins(long modelId, decimal amount)
        {
            await api.Call(RequestInfo(nameof(BuyBusinessCoins)), new
            {
                modelId = modelId,
                amount = amount
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
