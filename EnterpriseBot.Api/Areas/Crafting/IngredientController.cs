using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Crafting
{
    [Area(nameof(Crafting))]
    public class IngredientController : Controller, IGameController<Ingredient>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public IngredientController(ApplicationContext context,
                                    IOptions<GameplaySettings> gameplayOptions,
                                    IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Crafting.Ingredient;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Ingredient>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Ingredient ingredient = await ctx.Ingredients.FindAsync(id);
            //if (ingredient == null) return IngredientDoesNotExist(id);

            return ingredient;
        }

        ///// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Ingredient>> Create([FromBody] IngredientCreationParams cp)
        {
            Item item = await ctx.Items.FindAsync(cp.ItemId);
            if (item == null) return Errors.DoesNotExist(cp.ItemId, localizationSettings.Crafting.Item);

            if (cp.Quantity <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Amount of items can not be lower or equal to 0",
                    RussianMessage = "Количество предметов не может быть меньше или равно 0"
                };
            }

            Ingredient createdIngredient = (await ctx.Ingredients.AddAsync(new Ingredient
            {
                Item = item,
                Quantity = cp.Quantity
            })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.Ingredients.FindAsync(createdIngredient.Id);
        }


        [NonAction]
        private LocalizedError IngredientDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
