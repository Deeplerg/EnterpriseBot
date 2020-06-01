using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Business;
using EnterpriseBot.ApiWrapper.Models.Common.Essences;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Business;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business
{
    public class ShopCategory : BusinessCategoryBase<Shop>,
                                ICreatableCategory<Shop, ShopCreationParams>
    {
        private static readonly string categoryName = "shop";

        public ShopCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Shop> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Shop>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        /// <inheritdoc/>
        public async Task<Shop> Create(ShopCreationParams pars)
        {
            var result = await api.Call<Shop>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Withdraws units from the future general manager and creates a new shop
        /// </summary>
        /// <returns>Created shop</returns>
        /// <remarks>Customer is a general manager, so there's no need to specify one separately</remarks>
        public async Task<Shop> Buy(ShopCreationParams pars)
        {
            var result = await api.Call<Shop>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Buy).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Changes shop owner
        /// </summary>
        /// <param name="shopId">Shop id which owner to change</param>
        /// <param name="newOwnerId">New owner's player id</param>
        public async Task ChangeOwner(long shopId, long newOwnerId)
        {
            var pars = new
            {
                shopId = shopId,
                newOwnerId = newOwnerId
            };

            await api.Call<Player>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ChangeOwner).ToLower()
            }, pars);
        }

        /// <summary>
        /// Changes shop description
        /// </summary>
        /// <param name="shopId">Shop id which description to change</param>
        /// <param name="newDesc">New description</param>
        /// <returns>New description</returns>
        public async Task<string> ChangeDescription(long shopId, string newDesc)
        {
            var pars = new
            {
                shopId = shopId,
                newDescription = newDesc
            };

            var result = await api.Call<string>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ChangeDescription).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Withdraws all shop units to the owner's account
        /// </summary>
        /// <param name="shopId">Shop id</param>
        /// <returns>Shop units</returns>
        public async Task<decimal> WithdrawUnits(long shopId) //currently, there's no need to specify the amount
        {
            var pars = new
            {
                shopId = shopId
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(WithdrawUnits).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Adds specified amount of units to the shop account
        /// </summary>
        /// <param name="shopId">Id of the shop to which to add units</param>
        /// <param name="amount">Amount of units to add</param>
        /// <returns>Shop's units after adding</returns>
        public async Task<decimal> AddUnits(long shopId, decimal amount)
        {
            var pars = new
            {
                shopId = shopId,
                amount = amount
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(AddUnits).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Adds an item to player inventory and takes units for the purchase, also adding them to the shop account
        /// </summary>
        /// <param name="shopId">Id of the shop from which to buy the item</param>
        /// <param name="playerId">Id of the player who makes a purchase</param>
        /// <param name="itemWithPriceId">Id of the item in the shop showcase which to buy</param>
        /// <param name="amount">Amount of items of this type to buy</param>
        /// <returns>Player's units after making the purchase</returns>
        public async Task<decimal> BuyItem(long shopId, long playerId, long itemWithPriceId, int amount)
        {
            var pars = new
            {
                shopId = shopId,
                playerId = playerId,
                itemId = itemWithPriceId,
                amount = amount
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(BuyItem)
            }, pars);

            return result;
        }

        /// <summary>
        /// Returns a shop which name matches the specified one
        /// </summary>
        /// <param name="name">Shop name</param>
        /// <returns>Shop which name matches the specified one</returns>
        public async Task<Shop> GetByName(string name)
        {
            var pars = new
            {
                name = name
            };

            var result = await api.Call<Shop>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(GetByName).ToLower()
            }, pars);

            return result;
        }
    }
}
