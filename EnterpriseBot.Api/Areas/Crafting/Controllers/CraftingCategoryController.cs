using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Localization;
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
    public class CraftingCategoryController : Controller, 
                                              IGameController<CraftingCategory, CraftingCategoryApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CraftingCategoryController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public CraftingCategoryController(ApplicationContext dbContext, 
                                          ILogger<CraftingCategoryController> logger,
                                          IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Crafting.CraftingCategory;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CraftingCategory>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.CraftingCategories.FindAsync(id);

            return model;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CraftingCategory>> Create([FromBody] CraftingCategoryApiCreationParams pars)
        {
            var name = await ctx.LocalizedStrings.FindAsync(pars.NameLocalizedStringId);
            if (name == null) return Errors.DoesNotExist(pars.NameLocalizedStringId, localization.Localization.LocalizedString);

            var description = await ctx.LocalizedStrings.FindAsync(pars.DescriptionLocalizedStringId);
            if (description == null) return Errors.DoesNotExist(pars.DescriptionLocalizedStringId, localization.Localization.LocalizedString);

            List<CraftingSubCategory> subCategories = null;

            if(pars.CraftingSubCategoriesIds?.Any() is true)
            {
                subCategories = await ctx.CraftingSubCategories
                                         .Where(subCategory => 
                                                pars.CraftingSubCategoriesIds.Any(id => subCategory.Id == id))
                                         .ToListAsync();
            }

            var creationResult = CraftingCategory.Create(new CraftingCategoryCreationParams
            {
                Name = name,
                Description = description,
                SubCategories = subCategories
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.CraftingCategories.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<StringLocalization>> EditName([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newName = default(string),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var craftingCategory = await ctx.CraftingCategories.FindAsync(d.modelId);
            if (craftingCategory == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = craftingCategory.EditName(d.newName, d.language, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<StringLocalization>> EditDescription([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newDescription = default(string),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var craftingCategory = await ctx.CraftingCategories.FindAsync(d.modelId);
            if (craftingCategory == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = craftingCategory.EditDescription(d.newDescription, d.language, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
