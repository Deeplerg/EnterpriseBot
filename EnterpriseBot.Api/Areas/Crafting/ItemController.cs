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
    public class ItemController : Controller, IGameController<Item>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public ItemController(ApplicationContext context,
                              IOptions<GameplaySettings> gameplayOptions,
                              IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Crafting.Item;
        }

        ///<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Item>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Item item = await ctx.Items.FindAsync(id);
            //if (item == null) return ItemDoesNotExist(id);

            return item;
        }

        /////<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Item>> Create([FromBody] ItemCreationParams cp)
        {
            CraftingCategory category = await ctx.CraftingCategories.FindAsync(cp.Category);
            if (category == null) return Errors.DoesNotExist(cp.Category, localizationSettings.Crafting.CraftingCategory);

            Item createdItem = (await ctx.Items.AddAsync(new Item
            {
                Category = category,
                Name = cp.Name,
                Space = cp.Space
            })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.Items.FindAsync(createdItem.Id);
        }

        /// <summary>
        /// Returns all items using the specified crafting category
        /// </summary>
        /// <returns>All items with the specified craftin category</returns>
        public async Task<GameResult<List<Item>>> GetAllByCategory([FromBody] string json)
        {
            var pars = new
            {
                category = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            CraftingCategory category = await ctx.CraftingCategories.FindAsync(d.category);
            if (category == null) return Errors.DoesNotExist(category, localizationSettings.Crafting.CraftingCategory);

            return await ctx.Items.Where(item => item.Category == category).ToListAsync();
        }

        /// <summary>
        /// Returns an item which name matches the specified one
        /// </summary>
        /// <returns>Item which name matches the specified one</returns>
        [HttpPost]
        public async Task<GameResult<Item>> GetByName([FromBody] string json)
        {
            var pars = new
            {
                name = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Item item = await ctx.Items.FirstOrDefaultAsync(item => item.Name.ToLower().Trim() == d.name.ToLower().Trim());
            //if (item == null) return ItemDoesNotExist(d.name);

            return item;
        }


        [NonAction]
        private LocalizedError ItemDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
