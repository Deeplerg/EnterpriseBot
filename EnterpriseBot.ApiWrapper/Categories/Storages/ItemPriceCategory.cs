using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Storages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class ItemPriceCategory : StoragesCategoryBase<ItemPrice,
                                                          long,
                                                          ItemPriceCreationParams>
    {
        protected const string categoryName = "ItemPrice";

        public ItemPriceCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<ItemPrice> Get(long id)
        {
            return await api.Call<ItemPrice>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<ItemPrice> Create(ItemPriceCreationParams pars)
        {
            return await api.Call<ItemPrice>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<decimal> SetPrice(long modelId, decimal newPrice)
        {
            return await api.Call<decimal>(RequestInfo(nameof(SetPrice)), new
            {
                modelId = modelId,
                newPrice = newPrice
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
