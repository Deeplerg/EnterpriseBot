using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using EnterpriseBot.Api.Models.Common.Enums;
using static EnterpriseBot.Api.Utils.UserInputUtils;
using EnterpriseBot.Api.Models.Settings;

namespace EnterpriseBot.Api.Game.Crafting
{
    public class Item
    {
        protected Item() { }

        #region model
        public long Id { get; protected set; }
        public virtual LocalizedString Name { get; protected set; }

        public virtual CraftingSubCategory Category { get; protected set; }

        public decimal Space { get; protected set; } //how many space in the storage it takes

        #region errors
        private static readonly LocalizedError spaceLowerThanOrEqualTo0Error = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Item space cannot be lower than or equal to 0",
            RussianMessage = "Место, занимаемое предметом, не может быть ниже или равно 0"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<Item> Create(ItemCreationParams pars, GameSettings gameSettings)
        {
            var req = gameSettings.Localization.UserInputRequirements;

            foreach (var str in pars.Name.Localizations)
            {
                if (!CheckName(str.Text))
                {
                    return Errors.IncorrectNameInput(req);
                }
            }

            if (pars.Space <= 0)
            {
                return spaceLowerThanOrEqualTo0Error;
            }

            return new Item
            {
                Name = pars.Name,
                Space = pars.Space,
                Category = pars.Category
            };
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

        public GameResult<decimal> SetSpace(decimal newSpace)
        {
            if (newSpace <= 0)
            {
                return spaceLowerThanOrEqualTo0Error;
            }

            Space = newSpace;

            return Space;
        }

        public GameResult<CraftingSubCategory> SetCategory(CraftingSubCategory newCategory)
        {
            Category = newCategory;

            return Category;
        }
        #endregion
    }
}
