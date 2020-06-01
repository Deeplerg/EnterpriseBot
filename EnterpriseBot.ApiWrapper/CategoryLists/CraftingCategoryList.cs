using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Categories.Crafting;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class CraftingCategoryList : ICategoryList
    {
        public CraftingCategoryCategory CraftingCategory { get; set; }
        public ItemCategory Item { get; set; }
        public RecipeCategory Recipe { get; set; }
        public IngredientCategory Ingredient { get; set; }
    }
}
