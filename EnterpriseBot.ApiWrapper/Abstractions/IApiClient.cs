using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Abstractions
{
    public interface IApiClient
    {
        Task<T> Call<T>(ApiRequestInfo reqInfo, object objToSerialize);

        Task<object> Call(ApiRequestInfo reqInfo, object objToSerialize, Type typeToDeserialize);

        Task<dynamic> Call(ApiRequestInfo reqInfo, object objToSerialize);
    }
}
