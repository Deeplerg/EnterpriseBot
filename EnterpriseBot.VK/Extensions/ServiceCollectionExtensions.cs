using EnterpriseBot.ApiWrapper;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Services;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Linq;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace EnterpriseBot.VK.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVkApi(this IServiceCollection services, IOptions<VkSettings> vkOptions = null)
        {
            if (vkOptions is null)
            {
                vkOptions = services.BuildServiceProvider()
                                    .GetRequiredService<IOptions<VkSettings>>();
            }
            VkSettings settings = vkOptions.Value;

            ulong groupId = settings.GroupId;

            VkApi api = new VkApi(services);
            api.Authorize(new ApiAuthParams
            {
                AccessToken = settings.AccessToken
            });

            api.RequestsPerSecond = Constants.GroupMaxRequestsPerSecond;

            services.AddSingleton<IVkApi>(api);

            return services;
        }

        public static IServiceCollection AddEntbotApi(this IServiceCollection services,
                                                      string apiUri,
                                                      ushort? port = null,
                                                      CurrentLocalization localization = default)
        {
            return services.AddSingleton(new EntbotApi(apiUri, port, localization));
        }

        public static IServiceCollection AddEntbotApi(this IServiceCollection services,
                                                      Uri uri,
                                                      ushort? port = null,
                                                      CurrentLocalization localization = default)
        {
            return services.AddSingleton(new EntbotApi(uri, port, localization));
        }

        public static IServiceCollection AddEntbotApi(this IServiceCollection services, IOptions<ApiSettings> apiOptions = null)
        {
            if (apiOptions == null)
            {
                apiOptions = services.BuildServiceProvider()
                                     .GetRequiredService<IOptions<ApiSettings>>();
            }
            ApiSettings apiSettings = apiOptions.Value;

            return services.AddSingleton(new EntbotApi(apiUri: apiSettings.ApiServerUrl, localization: apiSettings.Localization));
        }

        public static IServiceCollection AddMenuRouter(this IServiceCollection services, bool enablePlugInMenus = false)
        {
            return services.AddSingleton<IMenuRouter, DefaultMenuRouter>(serviceProvider =>
            {
                return ActivatorUtilities.CreateInstance<DefaultMenuRouter>(serviceProvider, enablePlugInMenus);
            });
        }

        public static IServiceCollection AddVkMessageGateway(this IServiceCollection services)
        {
            return services.AddSingleton<IVkMessageGateway, SeparateThreadVkMessageGateway>();
        }

        public static IServiceCollection AddDefaultLocalPlayerManager(this IServiceCollection services)
        {
            return services.AddSingleton<ILocalPlayerManager, InMemoryLocalPlayerManager>();
        }

        public static IServiceCollection AddLocalPlayerManager<TImplementation>(this IServiceCollection services) where TImplementation : class, 
                                                                                                                                          ILocalPlayerManager
        {
            return services.AddSingleton<ILocalPlayerManager, TImplementation>();
        }

        public static IServiceCollection AddMenuMapper(this IServiceCollection services)
        {
            return services.AddTransient<IMenuMapper, DefaultMenuMapper>();
        }

        public static IServiceCollection AddConnectionMultiplexer(this IServiceCollection services, string connectionString)
        {
            return services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        }

        public static IServiceCollection AddVkUpdateHandlers(this IServiceCollection services)
        {
            services.AddTransient<IConfirmationUpdateHandler, ConfirmationUpdateHandler>();
            services.AddTransient<IMessageNewUpdateHandler, MessageNewUpdateHandler>();

            return services;
        }

        public static IServiceCollection AddMenuContextConfigurator(this IServiceCollection services)
        {
            return services.AddTransient<IMenuContextConfigurator, MenuContextConfigurator>();
        }
    }
}
