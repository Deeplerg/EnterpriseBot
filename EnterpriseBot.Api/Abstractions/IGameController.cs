using EnterpriseBot.Api.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Abstractions
{
    public interface IGameController<TModel> where TModel : class
    {
        /// <summary>
        /// Returns an object with specified <paramref name="idpar"/>
        /// </summary>
        /// <param name="idpar">Id of the object</param>
        /// <returns>Object with specified <paramref name="idpar"/></returns>
        Task<GameResult<TModel>> Get(IdParameter idpar);
    }
}
