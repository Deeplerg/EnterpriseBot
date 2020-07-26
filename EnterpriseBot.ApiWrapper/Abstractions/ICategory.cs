using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Abstractions
{
    internal interface ICategory<TModel, TId, TCreationParams> where TModel : class
                                                               where TCreationParams : class
    {
        Task<TModel> Get(TId id);

        Task<TModel> Create(TCreationParams pars);
    }
}
