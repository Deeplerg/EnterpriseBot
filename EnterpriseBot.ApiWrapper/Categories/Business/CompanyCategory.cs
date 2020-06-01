using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Business;
using EnterpriseBot.ApiWrapper.Models.Common.Crafting;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Business;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business
{
    public class CompanyCategory : BusinessCategoryBase<Company>,
                                   ICreatableCategory<Company, CompanyCreationParams>
    {
        private static readonly string categoryName = "company";

        public CompanyCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Company> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Company>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        /// <inheritdoc/>
        public async Task<Company> Create(CompanyCreationParams pars)
        {
            var result = await api.Call<Company>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Withdraws units from the future general manager and creates a new company
        /// </summary>
        /// <returns>Created company</returns>
        /// <remarks>Customer is a general manager, so there's no need to specify one separately</remarks>
        public async Task<Company> Buy(CompanyCreationParams pars)
        {
            var result = await api.Call<Company>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Buy).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Changes company owner
        /// </summary>
        /// <param name="companyId">Company id which owner to change</param>
        /// <param name="newOwnerId">New owner player id</param>
        public async Task ChangeOwner(long companyId, long newOwnerId)
        {
            var pars = new
            {
                companyId = companyId,
                newOwnerId = newOwnerId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ChangeOwner).ToLower()
            }, pars);
        }

        /// <summary>
        /// Add output item to the company output items list
        /// </summary>
        /// <param name="companyId">Id of the company to add output item</param>
        /// <param name="itemId">Item id to add</param>
        /// <returns>Output item list</returns>
        public async Task<List<Item>> AddOutputItem(long companyId, long itemId)
        {
            var pars = new
            {
                companyId = companyId,
                itemId = itemId
            };

            var result = await api.Call<List<Item>>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(AddOutputItem).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Remove output item from the company output items list
        /// </summary>
        /// <param name="companyId">Id of the company to remove output item</param>
        /// <param name="itemInOutput_Id">Item id in the company output items list to remove</param>
        /// <returns>Output items list</returns>
        public async Task<List<Item>> RemoveOutputItem(long companyId, long itemInOutput_Id)
        {
            var pars = new
            {
                companyId = companyId,
                itemInOutputId = itemInOutput_Id
            };

            var result = await api.Call<List<Item>>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(RemoveOutputItem).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Withdraws units from a company owner account and adds them to the company account
        /// </summary>
        /// <param name="companyId">Id of the company to which to add units</param>
        /// <param name="amount">Amount of units to add to the company account</param>
        /// <returns>Company units</returns>
        public async Task<decimal> AddUnitsFromGeneralManager(long companyId, decimal amount)
        {
            var pars = new
            {
                companyId = companyId,
                amount = amount
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(AddUnitsFromGeneralManager).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Withdraws company units to the owner's account
        /// </summary>
        /// <param name="companyId">Company id</param>
        /// <param name="amount">Units amount to withdraw</param>
        /// <returns>Company units</returns>
        public async Task<decimal> WithdrawUnits(long companyId, decimal amount)
        {
            var pars = new
            {
                companyId = companyId,
                amount = amount
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(WithdrawUnits).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Adds units to the company
        /// </summary>
        /// <param name="companyId">Id of the company to which to add units</param>
        /// <param name="amount">Amount of units to add</param>
        /// <returns>Company units</returns>
        public async Task<decimal> AddUnits(long companyId, decimal amount)
        {
            var pars = new
            {
                companyId = companyId,
                amount = amount
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(AddUnits).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Diminishes units to the company
        /// </summary>
        /// <param name="companyId">Id of the company from which to remove units</param>
        /// <param name="amount">Amount of units to remove</param>
        public async Task<decimal> DiminishUnits(long companyId, decimal amount)
        {
            var pars = new
            {
                companyId = companyId,
                amount = amount
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(DiminishUnits).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Changes company description
        /// </summary>
        /// <param name="companyId">Company id which description to change</param>
        /// <param name="newDesc">New description</param>
        /// <returns>New company description</returns>
        public async Task<string> ChangeDescription(long companyId, string newDesc)
        {
            var pars = new
            {
                companyId = companyId,
                newDescription = newDesc
            };

            var result = await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ChangeDescription).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Adds a truck to the company truck garage
        /// </summary>
        /// <param name="pars">Truck creation parameters</param>
        /// <returns>New truck</returns>
        public async Task<Truck> AddTruck(TruckCreationParams pars)
        {
            var result = await api.Call<Truck>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(AddTruck).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Decreases company units and adds new truck with default properties to the company truck garage
        /// </summary>
        /// <param name="companyId">Id of the company to which to add the truck and from which to remove units</param>
        /// <returns>New truck</returns>
        public async Task<Truck> BuyTruck(long companyId)
        {
            var pars = new
            {
                companyId = companyId
            };

            var result = await api.Call<Truck>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(BuyTruck).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Sends a truck to deliver items to another business
        /// </summary>
        /// <param name="truckId">Truck id to send</param>
        /// <param name="contractId">Contract id in the context of which the delivery is being committed</param>
        public async Task SendTruck(long truckId, long contractId)
        {
            var pars = new
            {
                truckId = truckId,
                contractId = contractId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(SendTruck).ToLower()
            }, pars);
        }

        /// <summary>
        /// Immediately returns truck back. <br/>
        /// Used with background jobs. Not recommended to call explicitly.
        /// </summary>
        /// <param name="truckId">Id of a truck to be returned</param>
        public async Task ReturnTruck(long truckId)
        {
            var pars = new
            {
                truckId = truckId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ReturnTruck).ToLower()
            }, pars);
        }

        /// <summary>
        /// Returns a company which name matches the specified one
        /// </summary>
        /// <param name="name">Company name</param>
        /// <returns>Company which name matches the specified one</returns>
        public async Task<Company> GetByName(string name)
        {
            var pars = new
            {
                name = name
            };

            var result = await api.Call<Company>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(GetByName).ToLower()
            }, pars);

            return result;
        }
    }
}
