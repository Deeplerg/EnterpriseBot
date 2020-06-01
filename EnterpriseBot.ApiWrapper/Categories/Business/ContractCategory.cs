using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Business;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Business;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business
{
    public class ContractCategory : BusinessCategoryBase<Contract>
    //ICreatableCategory<Contract, ContractCreationParams>
    {
        private const string categoryName = "contract";

        public ContractCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Contract> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Contract>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        ///// <inheritdoc/>
        //public async Task<Contract> Create(ContractCreationParams pars)
        //{
        //    var result = await api.Call<Contract>(new ApiRequestInfo
        //    {
        //        CategoryAreaName = categoryAreaName,
        //        CategoryName = categoryName,
        //        MethodName = nameof(Contract).ToLower()
        //    }, pars);

        //    return result;
        //}

        /// <summary>
        /// Breaks and removes the contract
        /// </summary>
        /// <param name="contractId">Contract's id to remove</param>
        public async Task Break(long contractId)
        {
            var pars = new
            {
                contractId = contractId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Break).ToLower()
            }, pars);
        }

        //requests can be received from corresponding Company property
        /// <summary>
        /// Creates a request for signing a contract
        /// </summary>
        /// <param name="pars">Contract request creation parameters from which the request will be created</param>
        /// <returns><see cref="ContractRequest"/> that was created</returns>
        public async Task<ContractRequest> CreateRequest(ContractRequestCreationParams pars)
        {
            var @params = new
            {
                contractRequestCreationParams = pars
            };

            var result = await api.Call<ContractRequest>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(CreateRequest).ToLower()
            }, @params);

            return result;
        }

        //also removes the request
        /// <summary>
        /// Applies request and creates contract from the request, removing the request
        /// </summary>
        /// <param name="requestId">Contract request's id to apply</param>
        /// <returns>Created contract</returns>
        public async Task<Contract> ApplyRequest(long requestId)
        {
            var pars = new
            {
                requestId = requestId
            };

            var result = await api.Call<Contract>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ApplyRequest).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Declines and removes contract request
        /// </summary>
        /// <param name="requestId">Request id</param>
        public async Task DeclineRequest(long requestId)
        {
            var pars = new
            {
                requestId = requestId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(DeclineRequest).ToLower()
            }, pars);
        }

        /// <summary>
        /// Checks contract completion and breaks the contract if it is not completed
        /// </summary>
        /// <param name="contractId">Id of the contract to check</param>
        public async Task CheckContractCompletionAndBreak(long contractId)
        {
            var pars = new
            {
                contractId = contractId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(CheckContractCompletionAndBreak).ToLower()
            }, pars);
        }
    }
}
