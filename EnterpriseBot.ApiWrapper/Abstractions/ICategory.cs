using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Abstractions
{
    public interface ICategory<TModel> where TModel : class
    {
        /// <summary>
        /// Get model asynchronously
        /// </summary>
        /// <param name="id">Model id</param>
        /// <returns>Model</returns>
        Task<TModel> Get(object id);
    }
}
