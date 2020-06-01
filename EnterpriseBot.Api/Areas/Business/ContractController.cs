using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Business;
using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.BackgroundJobs.Jobs;
using EnterpriseBot.BackgroundJobs.Params;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Business
{
    [Area(nameof(Business))]
    public class ContractController : Controller, IGameController<Contract>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public ContractController(ApplicationContext context,
                                  IOptions<GameplaySettings> gameplayOptions,
                                  IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Business.Contract;
        }

        [HttpPost]
        public async Task<GameResult<Contract>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Contract contract = await ctx.Contracts.FindAsync(id);
            //if (contract == null) return ContractDoesNotExist(id);

            return new GameResult<Contract>
            {
                Result = contract
            };
        }

        //[HttpPost]
        //public async Task<GameResult<Contract>> Create([FromBody] ContractCreationParams cp)
        //{  
        //}

        /// <summary>
        /// Breaks and removes the contract
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> Break([FromBody] string json, [FromServices] IBackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                contractId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Contract contract = await ctx.Contracts.FindAsync(d.contractId);
            if (contract == null) return ContractDoesNotExist(d.contractId);

            await BreakContract(contract, backgroundJobClient);

            return new EmptyGameResult();
        }

        /// <summary>
        /// Creates a request for signing a contract
        /// </summary>
        /// <returns><see cref="ContractRequest"/> that was created</returns>
        [HttpPost]
        public async Task<GameResult<ContractRequest>> CreateRequest([FromBody] ContractRequestCreationParams cp)
        {
            ContractInfo createdContractInfo = new ContractInfo();
            ContractRequest createdContractRequest = new ContractRequest();


            Item contractItem = await ctx.Items.FindAsync(cp.ContractItemId);
            if (contractItem == null) return Errors.DoesNotExist(cp.ContractItemId, localizationSettings.Crafting.Item);

            Company outcomeCompany = await ctx.Companies.FindAsync(cp.OutcomeCompanyId);
            if (outcomeCompany == null) return Errors.DoesNotExist(cp.OutcomeCompanyId, localizationSettings.Business.Company);

            if (cp.ContractItemQuantity < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Item quantity can't be lower than 1",
                    RussianMessage = "Количество предметов не может быть меньше 1"
                };
            }

            if (cp.ContractOverallCost < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Contract overall cost can't be lower than 1",
                    RussianMessage = "Общая стоимость контракта не может быть меньше 1"
                };
            }

            long incomeBusinessId = cp.IncomeBusinessId;

            //this switch:
            // - sets income business for contract info
            // - sets requested business for contract request
            switch (cp.IncomeBusinessType)
            {
                case BusinessType.Company:
                    Company company = await ctx.Companies.FindAsync(incomeBusinessId);
                    if (company == null) return Errors.DoesNotExist(incomeBusinessId, localizationSettings.Business.Company);

                    createdContractInfo.IncomeCompany = company;
                    createdContractRequest.RequestedCompany = company;
                    break;

                case BusinessType.Shop:
                    Shop shop = await ctx.Shops.FindAsync(incomeBusinessId);
                    if (shop == null) return Errors.DoesNotExist(incomeBusinessId, localizationSettings.Business.Shop);

                    createdContractInfo.IncomeShop = shop;
                    createdContractRequest.RequestedShop = shop;
                    break;
            }


            createdContractInfo.ContractOverallCost = cp.ContractOverallCost;
            createdContractInfo.ContractItem = contractItem;
            createdContractInfo.OutcomeCompany = outcomeCompany;
            createdContractInfo.ContractIncomeBusinessType = cp.IncomeBusinessType;
            createdContractInfo.ContractItemQuantity = cp.ContractItemQuantity;

            createdContractRequest.ContractInfo = createdContractInfo;


            await ctx.ContractInfos.AddAsync(createdContractInfo);
            await ctx.ContractRequests.AddAsync(createdContractRequest);

            await ctx.SaveChangesAsync();

            return await ctx.ContractRequests.FindAsync(createdContractRequest.Id);
        }

        /// <summary>
        /// Applies request and creates contract from the request, removing the request
        /// </summary>
        /// <returns>Created contract</returns>
        [HttpPost]
        public async Task<GameResult<Contract>> ApplyRequest([FromBody] string json, [FromServices] IBackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                requestId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            ContractRequest contractRequest = await ctx.ContractRequests.FindAsync(d.requestId);
            if (contractRequest == null) return Errors.DoesNotExist(d.requestId, localizationSettings.Business.ContractRequest);

            Contract createdContract = new Contract();

            switch (contractRequest.ContractInfo.ContractIncomeBusinessType)
            {
                case BusinessType.Company:
                    if (!contractRequest.ContractInfo.IncomeCompanyId.HasValue)
                    {
                        return new LocalizedError
                        {
                            ErrorSeverity = ErrorSeverity.Critical,
                            EnglishMessage = $"{nameof(ContractInfo)} (id: {contractRequest.ContractInfoId}) income business type is {nameof(Company)}, but the field is empty",
                            RussianMessage = $"{nameof(ContractInfo)} (id: {contractRequest.ContractInfoId}) тип приходящего бизнеса - {nameof(Company)}, но это поле пустое"
                        };
                    }
                    long companyId = contractRequest.ContractInfo.IncomeCompanyId.Value;

                    Company company = await ctx.Companies.FindAsync(companyId);
                    if (company == null) return Errors.DoesNotExist(companyId, localizationSettings.Business.Company);

                    if (company.CompanyUnits < contractRequest.ContractInfo.ContractOverallCost)
                    {
                        return new LocalizedError
                        {
                            ErrorSeverity = ErrorSeverity.Normal,
                            EnglishMessage = "The company does not have enough units to afford this contract",
                            RussianMessage = "У компании недостаточно юнитов, чтобы заключить этот контракт"
                        };
                    }

                    createdContract.IncomeCompany = company;
                    break;

                case BusinessType.Shop:
                    if (!contractRequest.ContractInfo.IncomeShopId.HasValue)
                    {
                        return new LocalizedError
                        {
                            ErrorSeverity = ErrorSeverity.Critical,
                            EnglishMessage = $"{nameof(ContractInfo)} (id: {contractRequest.ContractInfoId}) income business type is {nameof(Shop)}, but the field is empty",
                            RussianMessage = $"{nameof(ContractInfo)} (id: {contractRequest.ContractInfoId}) тип приходящего бизнеса - {nameof(Company)}, но это поле пустое"
                        };
                    }
                    long shopId = contractRequest.ContractInfo.IncomeShopId.Value;

                    Shop shop = await ctx.Shops.FindAsync(shopId);
                    if (shop == null) return Errors.DoesNotExist(shopId, localizationSettings.Business.Shop);

                    if (shop.ShopUnits < contractRequest.ContractInfo.ContractOverallCost)
                    {
                        return new LocalizedError
                        {
                            ErrorSeverity = ErrorSeverity.Normal,
                            EnglishMessage = "The shop does not have enough units to afford this contract",
                            RussianMessage = "У магазина недостаточно юнитов, чтобы заключить этот контракт"
                        };
                    }

                    createdContract.IncomeShop = shop;
                    break;
            }

            Company outcomeCompany = await ctx.Companies.FindAsync(contractRequest.ContractInfo.OutcomeCompany);
            if (outcomeCompany == null) return Errors.DoesNotExist(contractRequest.ContractInfo.OutcomeCompanyId, localizationSettings.Business.Company);

            Item contractItem = await ctx.Items.FindAsync(contractRequest.ContractInfo.ContractItem);
            if (contractItem == null) return Errors.DoesNotExist(contractRequest.ContractInfo.ContractItemId, localizationSettings.Crafting.Item);


            createdContract.Name = contractRequest.ContractInfo.Name;
            createdContract.Description = contractRequest.ContractInfo.Description;

            createdContract.ContractIncomeBusinessType = contractRequest.ContractInfo.ContractIncomeBusinessType;
            createdContract.OutcomeCompany = outcomeCompany;

            createdContract.TerminationTermInWeeks = contractRequest.ContractInfo.TerminationTermInWeeks;
            createdContract.ContractItem = contractItem;
            createdContract.ContractItemQuantity = contractRequest.ContractInfo.ContractItemQuantity;
            createdContract.DeliveredAmount = 0;


            await ctx.Contracts.AddAsync(createdContract);

            createdContract.ConclusionDate = DateTimeOffset.Now;

            var checkerJobParams = new ContractCheckerJobParams
            {
                ContractId = createdContract.Id
            };
            string checkerJobId = backgroundJobClient.Schedule<ContractCheckerJob>(j => j.Execute(checkerJobParams), delay: TimeSpan.FromDays(7));

            var breakerJobParams = new ContractBreakerJobParams
            {
                ContractId = createdContract.Id
            };
            string breakerJobId = backgroundJobClient.Schedule<ContractBreakerJob>(j => j.Execute(breakerJobParams), delay: TimeSpan.FromDays(createdContract.TerminationTermInWeeks * 7));

            createdContract.CompletionCheckerBackgroundJobId = checkerJobId;
            createdContract.BreakerBackgroundJobId = breakerJobId;

            await ctx.SaveChangesAsync();

            return await ctx.Contracts.FindAsync(createdContract.Id);
        }

        /// <summary>
        /// Checks contract completion and breaks the contract if it is not completed
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> CheckContractCompletionAndBreak([FromBody] string json, [FromServices] IBackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                contractId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Contract contract = await ctx.Contracts.FindAsync(d.contractId);
            if (contract == null) return ContractDoesNotExist(d.contractId);

            if (contract.DeliveredAmount < contract.ContractItemQuantity)
            {
                await BreakContract(contract, backgroundJobClient);
                return new EmptyGameResult();
            }

            var checkerJobParams = new ContractCheckerJobParams
            {
                ContractId = contract.Id
            };
            string checkerJobId = backgroundJobClient.Schedule<ContractCheckerJob>(j => j.Execute(checkerJobParams), delay: TimeSpan.FromDays(7));

            contract.CompletionCheckerBackgroundJobId = checkerJobId;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }


        /// <summary>
        /// Breaks the contract
        /// </summary>
        /// <param name="contract">Contract to break</param>
        /// <param name="backgroundJobClient"><see cref="IBackgroundJobClient"/> to remove completion checker background job</param>
        [NonAction]
        private async Task BreakContract(Contract contract, IBackgroundJobClient backgroundJobClient)
        {
            backgroundJobClient.Delete(contract.CompletionCheckerBackgroundJobId);
            backgroundJobClient.Delete(contract.BreakerBackgroundJobId);

            ctx.Contracts.Remove(contract);

            await ctx.SaveChangesAsync();
        }

        [NonAction]
        private LocalizedError ContractDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
