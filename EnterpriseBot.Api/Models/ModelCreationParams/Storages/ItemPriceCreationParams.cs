using System;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Storages;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class ItemPriceCreationParams
    {
        public decimal Price { get; set; }
        public Item Item { get; set; }
        public ShowcaseStorage OwningShowcase { get; set; }
    }
}