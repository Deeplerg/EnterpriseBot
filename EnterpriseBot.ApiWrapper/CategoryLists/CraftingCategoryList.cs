using EnterpriseBot.ApiWrapper.Categories.Crafting;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class CraftingCategoryList
    {
        internal CraftingCategoryList() { }

        public CraftingCategoryCategory CraftingCategory { get; internal set; }
        public CraftingSubCategoryCategory CraftingSubCategory { get; internal set; }
        public ItemCategory Item { get; internal set; }
        public RecipeCategory Recipe { get; internal set; }
        public IngredientCategory Ingredient { get; internal set; }
    }
}
