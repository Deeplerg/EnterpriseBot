using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.ApiCreationParams.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Crafting.Controllers
{
    [Area(nameof(Crafting))]
    public class RecipeController : Controller,
                                    IGameController<Recipe, RecipeApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<RecipeController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public RecipeController(ApplicationContext dbContext,
                                ILogger<RecipeController> logger,
                                IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Crafting.Recipe;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Recipe>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Recipes.FindAsync(id);

            return model;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Recipe>> Create([FromBody] RecipeApiCreationParams pars)
        {
            var resultItem = await ctx.Items.FindAsync(pars.ResultItemId);
            if (resultItem == null) return Errors.DoesNotExist(pars.ResultItemId, localization.Crafting.Item);

            List<Ingredient> ingredients = null;

            if (pars.IngredientsIds?.Any() is true)
            {
                ingredients = await ctx.Ingredients
                                       .Where(ingredient =>
                                              pars.IngredientsIds.Any(id => ingredient.Id == id))
                                       .ToListAsync();
            }

            var creationResult = Recipe.Create(new RecipeCreationParams
            {
                ResultItem = resultItem,
                Ingredients = ingredients,
                ResultItemQuantity = pars.ResultItemQuantity,
                CanBeDoneBy = pars.CanBeDoneBy,
                LeadTimeInSeconds = pars.LeadTimeInSeconds
            });
            if (creationResult.LocalizedError == null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Recipes.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<EmptyGameResult> RemoveIngredient([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                ingredientId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var recipe = await ctx.Recipes.FindAsync(d.modelId);
            if (recipe == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var ingredient = await ctx.Ingredients.FindAsync(d.ingredientId);
            if (ingredient == null) return Errors.DoesNotExist(d.ingredientId, localization.Crafting.Ingredient);

            if (!recipe.Ingredients.Contains(ingredient))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Ingredient {ingredient.Id} does not exist in recipe {recipe.Id}",
                    RussianMessage = $"Ингредиент {ingredient.Id} не существует в рецепте {recipe.Id}"
                };
            }

            var removeResult = recipe.RemoveIngredient(ingredient);
            if (removeResult.LocalizedError != null) return removeResult.LocalizedError;

            ctx.Remove(ingredient);
            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<GameResult<int>> SetLeadTimeInSeconds([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newLeadTimeInSeconds = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var recipe = await ctx.Recipes.FindAsync(d.modelId);
            if (recipe == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = recipe.SetLeadTimeInSeconds(d.newLeadTimeInSeconds);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<int>> SetResultItemQuantity([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newResultItemQuantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var recipe = await ctx.Recipes.FindAsync(d.modelId);
            if (recipe == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = recipe.SetResultItemQuantity(d.newResultItemQuantity);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<RecipeCanBeDoneBy>> SetCanBeDoneBy([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newCanBeDoneBy = default(RecipeCanBeDoneBy)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var recipe = await ctx.Recipes.FindAsync(d.modelId);
            if (recipe == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = recipe.SetCanBeDoneBy(d.newCanBeDoneBy);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
