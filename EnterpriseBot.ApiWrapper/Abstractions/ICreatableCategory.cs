using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Abstractions
{
    public interface ICreatableCategory<TModel, KParams>
            where TModel : class
            where KParams : class
    {
        /// <summary>
        /// Creates model asynchronously
        /// </summary>
        /// <param name="pars">Model creation params</param>
        /// <returns>Created model</returns>
        Task<TModel> Create(KParams pars);
    }
}
