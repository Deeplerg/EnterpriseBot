using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;

namespace EnterpriseBot.Api.Game.Storages
{
    public class StorageItem
    {
        protected StorageItem() { }

        #region model
        public long Id { get; protected set; }

        public virtual Item Item { get; protected set; }

        public int Quantity { get; protected set; }

        /// <summary>
        /// How much space it takes or would take in a storage
        /// </summary>
        public decimal Space
        {
            get => Quantity * Item.Space;
        }

        #region errors
        private static readonly LocalizedError quanitityLowerThan1Error = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Item qauntity can't be lower than 1",
            RussianMessage = "Количество предметов не может быть ниже 1"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<StorageItem> Create(StorageItemCreationParams pars)
        {
            if (pars.Quantity < 1)
            {
                return quanitityLowerThan1Error;
            }

            return new StorageItem
            {
                Item = pars.Item,
                Quantity = pars.Quantity
            };
        }

        public GameResult<int> AddQuantity(int amount)
        {
            if (amount < 1)
            {
                return quanitityLowerThan1Error;
            }

            Quantity += amount;

            return Quantity;
        }

        public GameResult<int> ReduceQuantity(int amount)
        {
            if (amount < 1)
            {
                return quanitityLowerThan1Error;
            }

            if (amount > Quantity)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Not enough items to remove {amount}",
                    RussianMessage = $"Недостаточно предметов, чтобы убрать {amount}"
                };
            }

            Quantity -= amount;

            return Quantity;
        }
        #endregion
    }
}
