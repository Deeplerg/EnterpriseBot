﻿using EnterpriseBot.ApiWrapper;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Services;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            ulong groupId = settings.DefaultGroupId;
            VkGroupSetting group = settings.Groups.Single(g => g.GroupId == groupId);

            VkApi api = new VkApi(services);
            api.Authorize(new ApiAuthParams
            {
                AccessToken = group.AccessToken
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

        public static IServiceCollection AddVkUpdateHandler(this IServiceCollection services)
        {
            return services.AddScoped<IVkUpdateHandler, DefaultVkUpdateHandler>();
        }

        public static IServiceCollection AddVkMessageGateway(this IServiceCollection services)
        {
            return services.AddSingleton<IVkMessageGateway, SeparateThreadVkMessageGateway>();
        }

        public static IServiceCollection AddLocalPlayerManager(this IServiceCollection services)
        {
            return services.AddSingleton<ILocalPlayerManager, DefaultLocalPlayerManager>();
        }

        public static IServiceCollection AddMenuMapper(this IServiceCollection services)
        {
            return services.AddTransient<IMenuMapper, DefaultMenuMapper>();
        }
    }
}
