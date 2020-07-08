using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.ApiCreationParams.Crafting;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Crafting.Controllers
{
    [Area(nameof(Crafting))]
    public class IngredientController : Controller,
                                        IGameController<Ingredient, IngredientApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<IngredientController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public IngredientController(ApplicationContext dbContext,
                                    ILogger<IngredientController> logger,
                                    IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Crafting.Ingredient;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Ingredient>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Ingredients.FindAsync(id);

            return model;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Ingredient>> Create([FromBody] IngredientApiCreationParams pars)
        {
            var item = await ctx.Items.FindAsync(pars.ItemId);
            if (item == null) return Errors.DoesNotExist(pars.ItemId, modelLocalization);

            var recipe = await ctx.Recipes.FindAsync(pars.RecipeId);
            if (recipe == null) return Errors.DoesNotExist(pars.RecipeId, localization.Crafting.Recipe);

            var creationResult = Ingredient.Create(new IngredientCreationParams
            {
                Item = item,
                Recipe = recipe,
                Quantity = pars.Quantity
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Ingredients.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<int>> SetQuantity([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newQuantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var ingredient = await ctx.Ingredients.FindAsync(d.modelId);
            if (ingredient == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            return ingredient.SetQuantity(d.newQuantity);
        }
    }
}
