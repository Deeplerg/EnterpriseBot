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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Crafting
{
    [Area(nameof(Crafting))]
    public class CraftingCategoryController : Controller, IGameController<CraftingCategory>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public CraftingCategoryController(ApplicationContext context,
                                          IOptions<GameplaySettings> gameplayOptions,
                                          IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Crafting.CraftingCategory;
        }

        ///<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<CraftingCategory>> Get([FromBody] IdParameter idpar)
        {
            string id = (string)idpar.Id;

            CraftingCategory craftingCategory = await ctx.CraftingCategories.FindAsync(id);
            //if (craftingCategory == null) return CraftingCategoryDoesNotExist(id);

            return craftingCategory;
        }

        /////<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<CraftingCategory>> Create([FromBody] CraftingCategoryCreationParams cp)
        {
            CraftingCategory createdCraftingCategory = (await ctx.CraftingCategories.AddAsync(new CraftingCategory
            {
                Name = cp.Name
            })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.CraftingCategories.FindAsync(createdCraftingCategory.Name);
        }

        /// <summary>
        /// Returns all crafting categories
        /// </summary>
        /// <returns>All crafting categories</returns>
        [HttpPost]
        public async Task<GameResult<List<CraftingCategory>>> GetAll()
        {
            return await ctx.CraftingCategories.ToListAsync();
        }


        [NonAction]
        private LocalizedError CraftingCategoryDoesNotExist(string id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
