using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Utils;
using System;
using System.Text.Json.Serialization;
using static EnterpriseBot.Api.Utils.UserInputUtils;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class CompanyContract
    {
        protected CompanyContract() { }

        #region model
        public long Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }


        public virtual Company OutcomeCompany { get; protected set; }
        public virtual Company IncomeCompany { get; protected set; }
        public CompanyContractIssuer Issuer { get; protected set; }

        public DateTimeOffset ConclusionDate { get; protected set; }
        public sbyte TerminationTermInDays { get; protected set; }

        public virtual Item ContractItem { get; protected set; }

        public int DeliveredAmount { get; protected set; } //how many items are already delivered
        public int ContractItemQuantity { get; protected set; } //items amount to be delivered
        public decimal ContractOverallCost { get; protected set; }

        public bool IsCompleted { get => DeliveredAmount >= ContractItemQuantity; }

        [JsonIgnore]
        public string CompletionCheckerBackgroundJobId { get; set; }
        #endregion

        #region actions
        public static GameResult<CompanyContract> Create(CompanyContractCreationParams creationPars, GameSettings gameSettings)
        {
            var cp = creationPars;
            var req = gameSettings.Localization.UserInputRequirements;

            var incomeCompanyCanConcludeResult = cp.IncomeCompany.CanConcludeOneMoreContract(gameSettings);
            if (incomeCompanyCanConcludeResult.LocalizedError != null) return incomeCompanyCanConcludeResult.LocalizedError;

            var outcomeCompanyCanConcludeResult = cp.OutcomeCompany.CanConcludeOneMoreContract(gameSettings);
            if (outcomeCompanyCanConcludeResult.LocalizedError != null) return outcomeCompanyCanConcludeResult.LocalizedError;

            if (!incomeCompanyCanConcludeResult
            || !outcomeCompanyCanConcludeResult)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Another company (in the context of this contract) has reached its maximum contracts limit",
                    RussianMessage = "Другая компания (в контексте данного контракта) достигла лимита на максимальное количество контрактов"
                };
            }

            var checkMaxTimeResult = CheckMaxTime(cp.IncomeCompany, cp.OutcomeCompany, gameSettings);
            if (checkMaxTimeResult.LocalizedError != null) return checkMaxTimeResult.LocalizedError;

            if (!CheckName(cp.Name))
            {
                return Errors.IncorrectNameInput(req);
            }
            if (!CheckDescription(cp.Description))
            {
                return Errors.IncorrectDescriptionInput(req);
            }

            if (cp.ItemQuantity < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Contract item quantity can't be lower than 1",
                    RussianMessage = "Количество предметов по контракту не может быть меньше 1"
                };
            }

            if (cp.OverallCost < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Contract conclusion price can't be lower than 1",
                    RussianMessage = "Цена контракта не может быть ниже 1"
                };
            }

            if (cp.TerminationTermInDays < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Contract term can't be under 1 day",
                    RussianMessage = "Срок контракта не может быть меньше 1 дня"
                };
            }

            if (cp.IncomeCompany.Purse.GetMoneyAmount(Currency.Units) < cp.OverallCost)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Income company does not have enough units to sign a contract"
                };
            }
            var reduceResult = cp.IncomeCompany.Purse.Reduce(cp.OverallCost, Currency.Units);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return new CompanyContract
            {
                Name = cp.Name,
                Description = cp.Description,

                DeliveredAmount = 0,
                ConclusionDate = DateTime.Now,
                ContractItem = cp.Item,
                ContractItemQuantity = cp.ItemQuantity,
                ContractOverallCost = cp.OverallCost,

                Issuer = cp.Issuer,

                IncomeCompany = cp.IncomeCompany,
                OutcomeCompany = cp.OutcomeCompany,

                TerminationTermInDays = cp.TerminationTermInDays
            };
        }

        public static GameResult<CompanyContract> Conclude(CompanyContractRequest contractRequest, GameSettings gameSettings, Player invoker = null)
        {
            var cReq = contractRequest;

            if (invoker != null && !invoker.HasPermission(CompanyJobPermissions.SignContracts, cReq.RequestedCompany))
            {
                return Errors.DoesNotHavePermission();
            }

            Company incomeCompany;
            Company outcomeCompany;
            switch (cReq.RequestingCompanyRelationSide)
            {
                case CompanyContractIssuer.IncomeCompany:
                    incomeCompany = cReq.RequestingCompany;
                    outcomeCompany = cReq.RequestedCompany;
                    break;

                case CompanyContractIssuer.OutcomeCompany:
                    outcomeCompany = cReq.RequestingCompany;
                    incomeCompany = cReq.RequestedCompany;
                    break;

                default:
                    return Errors.UnknownEnumValue(cReq.RequestingCompanyRelationSide);
            }

            CompanyContractIssuer issuer = cReq.RequestingCompanyRelationSide;

            var checkMaxTimeResult = CheckMaxTime(incomeCompany, outcomeCompany, gameSettings, invoker);
            if (checkMaxTimeResult.LocalizedError != null) return checkMaxTimeResult.LocalizedError;

            return Create(new CompanyContractCreationParams
            {
                Name = cReq.Name,
                Description = cReq.Description,

                IncomeCompany = incomeCompany,
                OutcomeCompany = outcomeCompany,

                Issuer = issuer,

                Item = cReq.ContractItem,
                ItemQuantity = cReq.ContractItemQuantity,

                OverallCost = cReq.ContractOverallCost,
                TerminationTermInDays = cReq.TerminationTermInDays
            }, gameSettings);
        }

        public GameResult<bool> CheckCompletion()
        {
            if (DeliveredAmount < ContractItemQuantity)
                return false;
            else
                return true;
        }

        public GameResult<int> AddDeliveredAmount(int amount)
        {
            if (amount < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"{Id} Contract delivered item amount is less than 1: {amount}",
                    RussianMessage = $"Количество доставленных предметов в контракте {Id} меньше 1: {amount}"
                };
            }

            DeliveredAmount += amount;

            return DeliveredAmount;
        }

        public EmptyGameResult Complete()
        {
            var incomeCompleteResult = IncomeCompany.CompleteAndRemoveContract(this);
            if (incomeCompleteResult.LocalizedError != null) return incomeCompleteResult.LocalizedError;

            var outcomeCompleteResult = OutcomeCompany.CompleteAndRemoveContract(this);
            if (outcomeCompleteResult.LocalizedError != null) return outcomeCompleteResult.LocalizedError;

            return new EmptyGameResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="incomeCompany"></param>
        /// <param name="outcomeCompany"></param>
        /// <param name="invoker"></param>
        /// <returns>If max time check wasn't passed, returns <see cref="LocalizedError"/>, otherwise, empty</returns>
        private static EmptyGameResult CheckMaxTime(Company incomeCompany, Company outcomeCompany, GameSettings gameSettings, Player invoker = null)
        {
            var contractSettings = gameSettings.Business.Company.Contract;

            var incomeMaxTimeResult = incomeCompany.GetContractMaxTimeInDays(gameSettings, invoker);
            if (incomeMaxTimeResult.LocalizedError != null) return incomeMaxTimeResult.LocalizedError;

            var outcomeMaxTimeResult = outcomeCompany.GetContractMaxTimeInDays(gameSettings, invoker);
            if (outcomeMaxTimeResult.LocalizedError != null) return outcomeMaxTimeResult.LocalizedError;

            uint maxTime = incomeMaxTimeResult;
            if (outcomeMaxTimeResult > maxTime)
                maxTime = outcomeMaxTimeResult;

            if (maxTime > contractSettings.MaxTimeInDays)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Contract term can't be more than {contractSettings.MaxTimeInDays} days",
                    RussianMessage = $"Срок контракта не может быть больше чем {contractSettings.MaxTimeInDays} дней"
                };
            }

            return new EmptyGameResult();
        }
        #endregion
    }
}
