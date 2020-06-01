using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Crafting
{
    [Area(nameof(Crafting))]
    public class RecipeController : Controller, IGameController<Recipe>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public RecipeController(ApplicationContext context,
                                IOptions<GameplaySettings> gameplayOptions,
                                IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Crafting.Recipe;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Recipe>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Recipe recipe = await ctx.Recipes.FindAsync(id);
            //if (recipe == null) return RecipeDoesNotExist(id);

            return recipe;
        }

        ///// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Recipe>> Create([FromBody] RecipeCreationParams cp)
        {
            async Task<GameResult<List<Ingredient>>> GetIngredients(List<long> ids)
            {
                var ingredients = new List<Ingredient>();

                foreach (long id in ids)
                {
                    Ingredient ingredient = await ctx.Ingredients.FindAsync(id);
                    if (ingredient == null) return Errors.DoesNotExist(id, localizationSettings.Crafting.Ingredient);

                    ingredients.Add(ingredient);
                }

                return ingredients;
            }

            if (cp.ResultItemQuantity <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Result item quantity can not be lower than or equals to 0",
                    RussianMessage = "Количество конечного предмета не может быть меньше или равно 0"
                };
            }

            var ingredientsResult = await GetIngredients(cp.IngredientsIds);
            if (ingredientsResult.LocalizedError != null)
                return ingredientsResult.LocalizedError;

            var ingredients = ingredientsResult.Result;

            Item resultItem = await ctx.Items.FindAsync(cp.ResultItemId);
            if (resultItem == null) return Errors.DoesNotExist(cp.ResultItemId, localizationSettings.Crafting.Item);

            Recipe createdRecipe = (await ctx.Recipes.AddAsync(new Recipe
            {
                Ingredients = ingredients,
                ResultItem = resultItem,
                LeadTimeInSeconds = cp.LeadTimeInSeconds,
                ResultItemQuantity = cp.ResultItemQuantity
            })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.Recipes.FindAsync(createdRecipe.Id);
        }

        /// <summary>
        /// Returns recipes where result item matches the specified one
        /// </summary>
        /// <returns>Recipes for the specified item</returns>
        [HttpPost]
        public async Task<GameResult<List<Recipe>>> GetRecipesForItem([FromBody] string json)
        {
            var pars = new
            {
                itemId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            var recipes = await ctx.Recipes.Where(r => r.ResultItem == item).ToListAsync();
            return recipes;
        }


        [NonAction]
        private LocalizedError RecipeDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
