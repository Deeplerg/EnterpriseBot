using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class CompanyContractRequest
    {
        protected CompanyContractRequest() { }

        #region model
        public long Id { get; protected set; }

        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public virtual Company RequestedCompany { get; protected set; }
        public virtual Company RequestingCompany { get; protected set; }
        public CompanyContractIssuer RequestingCompanyRelationSide { get; protected set; }

        public virtual Item ContractItem { get; protected set; }
        public int ContractItemQuantity { get; protected set; }

        public decimal ContractOverallCost { get; protected set; }
        public sbyte TerminationTermInWeeks { get; protected set; }
        #endregion

        #region actions
        public static GameResult<CompanyContractRequest> Create(ContractRequestCreationParams creationPars,
            UserInputRequirements inputRequirements)
        {
            return CreateBase(creationPars, inputRequirements);
        }

        public static GameResult<CompanyContractRequest> Create(ContractRequestCreationParams creationPars,
            UserInputRequirements inputRequirements, Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.SignContracts, creationPars.RequestingCompany))
            {
                return Errors.DoesNotHavePermission();
            }

            return CreateBase(creationPars, inputRequirements);
        }

        public GameResult<string> SetName(string newName, UserInputRequirements req)
        {
            if(!UserInputUtils.CheckName(newName))
            {
                return Errors.IncorrectNameInput(req);
            }

            Name = newName;

            return Name;
        }

        public GameResult<string> SetDescription(string newDesc, UserInputRequirements req)
        {
            if (!UserInputUtils.CheckDescription(newDesc))
            {
                return Errors.IncorrectDescriptionInput(req);
            }

            Description = newDesc;

            return Description;
        }


        private static GameResult<CompanyContractRequest> CreateBase(ContractRequestCreationParams cp,
            UserInputRequirements req)
        {
            if (!UserInputUtils.CheckName(cp.Name))
            {
                return Errors.IncorrectNameInput(req);
            }
            if (!UserInputUtils.CheckDescription(cp.Description))
            {
                return Errors.IncorrectDescriptionInput(req);
            }

            if (cp.ContractItemQuantity < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Contract item quantity can't be lower than 1",
                    RussianMessage = "Количество предметов по контракту не может быть меньше 1"
                };
            }

            if (cp.ContractOverallCost < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Contract conclusion price can't be lower than 1",
                    RussianMessage = "Цена контракта не может быть ниже 1"
                };
            }

            if (cp.TerminationTermInWeeks < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Contract term can't be under 1 week",
                    RussianMessage = "Срок контракта не может быть меньше 1 недели"
                };
            }

            if(cp.RequestedCompany == cp.RequestingCompany)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Unable to conclude a contract with your own company",
                    RussianMessage = "Нельзя заключить контракт со своей же компанией"
                };
            }

            return new CompanyContractRequest
            {
                Name = cp.Name,
                Description = cp.Description,

                RequestedCompany = cp.RequestedCompany,
                RequestingCompany = cp.RequestingCompany,

                ContractItem = cp.ContractItem,
                ContractItemQuantity = cp.ContractItemQuantity,

                ContractOverallCost = cp.ContractOverallCost,
                TerminationTermInWeeks = cp.TerminationTermInWeeks
            };
        }
        #endregion
    }
}
