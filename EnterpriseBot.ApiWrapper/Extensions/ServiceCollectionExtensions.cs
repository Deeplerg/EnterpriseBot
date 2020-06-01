using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.ApiWrapper.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EnterpriseBot.ApiWrapper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiClient(this IServiceCollection services, Uri apiUri, CurrentLocalization currentLocalization)
        {
            services.AddTransient<IApiClient>(provider => new ApiClient(apiUri, currentLocalization));

            return services;
        }
    }
}
