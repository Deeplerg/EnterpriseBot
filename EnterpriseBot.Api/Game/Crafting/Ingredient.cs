using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;

namespace EnterpriseBot.Api.Game.Crafting
{
    public class Ingredient
    {
        protected Ingredient() { }

        #region model
        public long Id { get; protected set; }

        public virtual Item Item { get; protected set; }

        public int Quantity { get; protected set; }
        public virtual Recipe Recipe { get; protected set; }

        #region errors
        private static readonly LocalizedError quantityLowerThan1Error = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Item quantity can't be lower than 1",
            RussianMessage = "Количество предметов не может быть ниже 1"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<Ingredient> Create(IngredientCreationParams pars)
        {
            if (pars.Quantity < 1)
            {
                return quantityLowerThan1Error;
            }

            return new Ingredient
            {
                Item = pars.Item,
                Quantity = pars.Quantity,
                Recipe = pars.Recipe
            };
        }

        public GameResult<int> SetQuantity(int newQuantity)
        {
            if (newQuantity < 1)
            {
                return quantityLowerThan1Error;
            }

            Quantity = newQuantity;

            return Quantity;
        }
        #endregion
    }
}
