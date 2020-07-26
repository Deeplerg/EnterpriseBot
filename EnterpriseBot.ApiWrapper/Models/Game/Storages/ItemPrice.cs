using EnterpriseBot.ApiWrapper.Models.Game.Crafting;

namespace EnterpriseBot.ApiWrapper.Models.Game.Storages
{
    public class ItemPrice
    {
        public long Id { get; set; }

        /// <summary>
        /// Price in units
        /// </summary>
        public decimal Price { get; set; }

        public Item Item { get; set; }

        public ShowcaseStorage OwningShowcase { get; set; }
    }
}
