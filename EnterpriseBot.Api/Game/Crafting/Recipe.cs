using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnterpriseBot.Api.Game.Crafting
{
    public class Recipe
    {
        protected Recipe() { }

        #region model
        public long Id { get; protected set; }

        public virtual Item ResultItem { get; protected set; }

        public int ResultItemQuantity { get; protected set; }

        public int LeadTimeInSeconds { get; protected set; } //how much time in seconds it takes to produce it

        public RecipeCanBeDoneBy CanBeDoneBy { get; protected set; }

        public virtual IReadOnlyCollection<Ingredient> Ingredients
        {
            get => new ReadOnlyCollection<Ingredient>(ingredients);
            protected set => ingredients = value.ToList();
        }
        private List<Ingredient> ingredients = new List<Ingredient>();

        #region errors
        private static readonly LocalizedError leadTimeLowerThan1Error = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Lead time can't be lower than 1 second",
            RussianMessage = "Время, необходимое для создания, не может быть меньше 1 секунды"
        };

        private static readonly LocalizedError resultItemQuantityLowerThan1Error = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Result item quantity can't be lower than 1",
            RussianMessage = "Количество создаваемых предметов не может быть меньше 1"
        };

        #endregion
        #endregion

        #region actions
        public static GameResult<Recipe> Create(RecipeCreationParams pars)
        {
            if (pars.LeadTimeInSeconds < 1)
            {
                return leadTimeLowerThan1Error;
            }

            if (pars.ResultItemQuantity < 1)
            {
                return resultItemQuantityLowerThan1Error;
            }

            return new Recipe()
            {
                Ingredients = pars.Ingredients,

                ResultItem = pars.ResultItem,
                ResultItemQuantity = pars.ResultItemQuantity,

                LeadTimeInSeconds = pars.LeadTimeInSeconds,

                CanBeDoneBy = pars.CanBeDoneBy
            };
        }

        public GameResult<Ingredient> AddIngredient(IngredientCreationParams ingredientCreationParams)
        {
            var ingredientCreationResult = Ingredient.Create(ingredientCreationParams);
            if (ingredientCreationResult.LocalizedError != null) return ingredientCreationResult.LocalizedError;

            Ingredient ingredient = ingredientCreationResult;

            ingredients.Add(ingredient);

            return ingredient;
        }

        public EmptyGameResult RemoveIngredient(Ingredient ingredient)
        {
            ingredients.Remove(ingredient);

            return new EmptyGameResult();
        }

        public GameResult<int> SetLeadTimeInSeconds(int newLeadTimeInSeconds)
        {
            if (newLeadTimeInSeconds < 1)
            {
                return leadTimeLowerThan1Error;
            }

            LeadTimeInSeconds = newLeadTimeInSeconds;

            return LeadTimeInSeconds;
        }

        public GameResult<int> SetResultItemQuantity(int newResultItemQuantity)
        {
            if (newResultItemQuantity < 1)
            {
                return resultItemQuantityLowerThan1Error;
            }

            ResultItemQuantity = newResultItemQuantity;

            return ResultItemQuantity;
        }

        public GameResult<RecipeCanBeDoneBy> SetCanBeDoneBy(RecipeCanBeDoneBy newCanBeDoneBy)
        {
            CanBeDoneBy = newCanBeDoneBy;

            return CanBeDoneBy;
        }
        #endregion
    }
}
