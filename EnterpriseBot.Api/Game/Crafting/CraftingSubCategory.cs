using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.Api.Models.Common.Enums;
using static EnterpriseBot.Api.Utils.UserInputUtils;
using EnterpriseBot.Api.Models.Settings;

namespace EnterpriseBot.Api.Game.Crafting
{
    public class CraftingSubCategory
    {
        protected CraftingSubCategory() { }

        #region model
        public long Id { get; protected set; }
        public virtual LocalizedString Name { get; protected set; }
        public virtual LocalizedString Description { get; protected set; }
        public virtual CraftingCategory MainCategory { get; protected set; }

        public virtual IReadOnlyCollection<Item> Items
        {
            get => new ReadOnlyCollection<Item>(items);
            protected set => items = value.ToList();
        }
        private List<Item> items = new List<Item>();
        #endregion

        #region actions
        public static GameResult<CraftingSubCategory> Create(CraftingSubCategoryCreationParams pars, GameSettings gameSettings)
        {
            var req = gameSettings.Localization.UserInputRequirements;

            foreach (var str in pars.Name.Localizations)
            {
                if (!CheckName(str.Text))
                {
                    return Errors.IncorrectNameInput(req);
                }
            }

            foreach (var str in pars.Description.Localizations)
            {
                if (!CheckDescription(str.Text))
                {
                    return Errors.IncorrectDescriptionInput(req);
                }
            }

            var craftingSubCategory = new CraftingSubCategory
            {
                Name = pars.Name,
                Description = pars.Description,
                MainCategory = pars.MainCategory
            };

            if (pars.Items != null && pars.Items.Any())
                craftingSubCategory.items = pars.Items;

            return craftingSubCategory;
        }

        public GameResult<StringLocalization> EditName(string newName, LocalizationLanguage language, GameSettings gameSettings)
        {
            var req = gameSettings.Localization.UserInputRequirements;

            if (!CheckName(newName))
            {
                return Errors.IncorrectNameInput(req);
            }

            var editResult = Name.Edit(newName, language);
            if (editResult.LocalizedError != null) return editResult.LocalizedError;

            return editResult;
        }

        public GameResult<StringLocalization> EditDescription(string newDescription, LocalizationLanguage language, GameSettings gameSettings)
        {
            var req = gameSettings.Localization.UserInputRequirements;

            if (!CheckDescription(newDescription))
            {
                return Errors.IncorrectDescriptionInput(req);
            }

            var editResult = Description.Edit(newDescription, language);
            if (editResult.LocalizedError != null) return editResult.LocalizedError;

            return editResult;
        }

        public GameResult<CraftingCategory> SetCategory(CraftingCategory newCraftingCategory)
        {
            MainCategory = newCraftingCategory;

            return MainCategory;
        }
        #endregion
    }
}
