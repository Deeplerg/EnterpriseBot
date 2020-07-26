using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;

namespace EnterpriseBot.Api.Game.Storages
{
    public class ItemPrice
    {
        protected ItemPrice() { }

        #region model
        public long Id { get; protected set; }

        /// <summary>
        /// Price in units
        /// </summary>
        public decimal Price { get; protected set; }

        public virtual Item Item { get; protected set; }

        public virtual ShowcaseStorage OwningShowcase { get; protected set; }

        #region errors
        private static readonly LocalizedError priceLowerThanOrEqualTo0Error = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Price can't be lower than or equal to 0",
            RussianMessage = "Цена не может быть меньше или равна 0"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<ItemPrice> Create(ItemPriceCreationParams pars)
        {
            if (pars.Price <= 0)
            {
                return priceLowerThanOrEqualTo0Error;
            }

            return new ItemPrice
            {
                Price = pars.Price,
                Item = pars.Item,
                OwningShowcase = pars.OwningShowcase
            };
        }

        public GameResult<decimal> SetPrice(decimal newPrice)
        {
            if (newPrice <= 0)
            {
                return priceLowerThanOrEqualTo0Error;
            }

            Price = newPrice;

            return Price;
        }
        #endregion
    }
}
