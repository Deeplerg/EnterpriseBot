using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.ApiCreationParams.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Storages.Controllers
{
    [Area(nameof(Storages))]
    public class ItemPriceController : Controller,
                                       IGameController<ItemPrice, ItemPriceApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<ItemPriceController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public ItemPriceController(ApplicationContext dbContext,
                                   ILogger<ItemPriceController> logger,
                                   IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Storages.ItemPrice;
        }

        ///<inheritdoc/>
        public async Task<GameResult<ItemPrice>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.ItemPrices.FindAsync(id);

            return model;
        }

        public async Task<GameResult<ItemPrice>> Create([FromBody] ItemPriceApiCreationParams pars)
        {
            var item = await ctx.Items.FindAsync(pars.ItemId);
            if (item == null) return Errors.DoesNotExist(pars.ItemId, localization.Crafting.Item);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(pars.OwningShowcaseStorageId);
            if (showcaseStorage == null) return Errors.DoesNotExist(pars.OwningShowcaseStorageId, localization.Storages.ShowcaseStorage);


            var creationResult = ItemPrice.Create(new ItemPriceCreationParams
            {
                OwningShowcase = showcaseStorage,
                Item = item,
                Price = pars.Price
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.ItemPrices.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<decimal>> SetPrice([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newPrice = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var itemPrice = await ctx.ItemPrices.FindAsync(d.modelId);
            if (itemPrice == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = itemPrice.SetPrice(d.newPrice);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
