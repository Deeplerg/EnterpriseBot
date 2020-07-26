using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static EnterpriseBot.Api.Utils.UserInputUtils;

namespace EnterpriseBot.Api.Game.Crafting
{
    public class CraftingCategory
    {
        protected CraftingCategory() { }

        #region model
        public long Id { get; protected set; }
        public virtual LocalizedString Name { get; protected set; }
        public virtual LocalizedString Description { get; protected set; }

        public virtual IReadOnlyCollection<CraftingSubCategory> SubCategories
        {
            get => new ReadOnlyCollection<CraftingSubCategory>(subCategories);
            protected set => subCategories = value.ToList();
        }
        private List<CraftingSubCategory> subCategories = new List<CraftingSubCategory>();
        #endregion

        #region actions
        public static GameResult<CraftingCategory> Create(CraftingCategoryCreationParams pars, GameSettings gameSettings)
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

            var craftingCategory = new CraftingCategory
            {
                Name = pars.Name,
                Description = pars.Description
            };

            if (pars.SubCategories != null && pars.SubCategories.Any())
                craftingCategory.subCategories = pars.SubCategories;

            return craftingCategory;
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
        #endregion
    }
}
