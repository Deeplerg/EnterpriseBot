using EnterpriseBot.Api.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Abstractions
{
    public interface IGameController<TModel, TCreationParams> where TModel : class
                                                              where TCreationParams : class
    {
        /// <summary>
        /// Returns <typeparamref name="TModel"/> with specified <paramref name="idpar"/>
        /// </summary>
        /// <param name="idpar">Id of <typeparamref name="TModel"/></param>
        /// <returns><typeparamref name="TModel"/> with specified <paramref name="idpar"/></returns>
        Task<GameResult<TModel>> Get(IdParameter idpar);

        /// <summary>
        /// Creates <typeparamref name="TModel"/> from <typeparamref name="TCreationParams"/> and returns it back
        /// </summary>
        /// <param name="pars">Parameters from which the <typeparamref name="TModel"/> will be created</param>
        /// <returns>Created <typeparamref name="TModel"/></returns>
        Task<GameResult<TModel>> Create(TCreationParams pars);
    }
}
