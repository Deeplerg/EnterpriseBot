using EnterpriseBot.ApiWrapper;
using EnterpriseBot.ApiWrapper.Exceptions;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Exceptions;
using EnterpriseBot.VK.Extensions;
using EnterpriseBot.VK.Models.Contexts;
using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.MenuResults;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Models.Other;
using EnterpriseBot.VK.Models.Player;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace EnterpriseBot.VK.Services
{
    public class DefaultVkUpdateHandler : IVkUpdateHandler
    {
        private readonly ILogger<DefaultVkUpdateHandler> logger;
        private readonly IVkApi vkApi;
        private readonly VkSettings vkSettings;
        private readonly ILocalPlayerManager playerManager;
        private readonly EntbotApi botApi;
        private readonly IVkMessageGateway messages;
        private readonly IMenuRouter menuRouter;
        private readonly ErrorDbContext errorCtx;
        private readonly IMenuMapper menuMapper;

        public DefaultVkUpdateHandler(ILogger<DefaultVkUpdateHandler> logger,
                                      IVkApi vkApi,
                                      IOptions<VkSettings> vkOptions,
                                      ILocalPlayerManager localPlayerManager,
                                      EntbotApi botApi,
                                      IVkMessageGateway vkMessageGateway,
                                      IMenuRouter menuRouter,
                                      ErrorDbContext errorCtx,
                                      IMenuMapper menuMapper)
        {
            this.logger = logger;
            this.vkApi = vkApi;
            this.vkSettings = vkOptions.Value;
            this.playerManager = localPlayerManager;
            this.botApi = botApi;
            this.messages = vkMessageGateway;
            this.menuRouter = menuRouter;
            this.errorCtx = errorCtx;
            this.menuMapper = menuMapper;
        }

        public async Task<HandleUpdateResult> HandleUpdateAsync(GroupUpdate update)
        {
            VkGroupSetting group = vkSettings.Groups.FirstOrDefault(g => g.GroupId == update.GroupId.Value);
            if (group is null)
            {
                logger.LogWarning($"Unable to find group {update.GroupId.Value}, can't handle {update.Type.ToString()} update");
                return new HandleUpdateResult(success: false);
            }

            if (update.Secret != group.SecretKey)
            {
                string trueKey = group.SecretKey;
                string hiddenTrueKey = HideKey(trueKey, trueKey.Length / 3);

                logger.LogWarning($"Wrong secret key! Excepted: {hiddenTrueKey}, but was: {update.Secret}");

                return new HandleUpdateResult(success: false);
            }

            switch (update.Type)
            {
                case var _ when update.Type == GroupUpdateType.MessageNew:
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var result = await MessageNew(update.MessageNew);
                    sw.Stop();
                    logger.LogInformation($"{update.Type.ToString()} took {sw.Elapsed} to complete");

                    return result;

                case var _ when update.Type == GroupUpdateType.Confirmation:
                    return await Confirmation(update.GroupId.Value);

                default:
                    return new HandleUpdateResult(success: false);
            }
        }

        private async Task<HandleUpdateResult> MessageNew(MessageNew messageNew)
        {
            void LogExceptionWithPeerId(Exception ex, long peerId)
            {
                string logErrorMessage = $"{ex.GetType().Name} from {peerId}: {string.Join("", ex.Message.Take(20))}";
                if (ex.Message.Length > 20)
                {
                    logErrorMessage += "...";
                }

                logger.LogError(ex, logErrorMessage);
            }
            async Task<Guid?> TrySaveExceptionAsync(Exception ex)
            {
                Error error = new Error
                {
                    Exception = ex.ToString()
                };

                try
                {
                    await errorCtx.Errors.AddAsync(error);
                    await errorCtx.SaveChangesAsync();
                    return error.Id;
                }
                catch
                {
                    return null;
                }
            }

            bool success = false;

            var message = messageNew.Message;
            long peerId = message.PeerId.Value;

            try
            {
                var players = botApi.Essences.Player;


                MenuContext context = new MenuContext();

                try
                {
                    await ConfigureMenuContext(context, message, playerManager, botApi);
                }
                #region Exception handling
                catch (InvalidMessagePayloadException ex)
                {
                    LogExceptionWithPeerId(ex, peerId);

                    string invalidPayloadMessage = string.Format(ExceptionTemplates.InvalidMessagePayloadWarnTemplate,
                                                                 Path.Combine(
                                                                 vkSettings.Links.VkDomain,
                                                                 vkSettings.Links.EntbotSupportVkName));

                    messages.Send(peerId, invalidPayloadMessage);

                    return new HandleUpdateResult(success: true);
                }
                #endregion     

                IMenuResult result = null;
                NextAction next = null;

                try
                {
                    next = menuMapper.MapAction(context);
                    result = await menuMapper.InvokeAction(next, context, menuRouter,
                                                           context);

                    success = true;
                }
                #region Exception handling
                catch (ApiNormalException ex)
                {
                    LogExceptionWithPeerId(ex, peerId);

                    if (context.LocalPlayer?.PreviousAction != null)
                    {
                        result = new ReturnBackKeyboardResult(ex.Message, context.LocalPlayer.PreviousAction);
                    }
                    else
                    {
                        messages.Send(peerId, ex.Message);
                        return new HandleUpdateResult(success: false);
                    }

                    success = false;
                }

                //trying to return the user to the previous menu at all costs
                catch (Exception ex)
                {
                    Guid? errorId = await TrySaveExceptionAsync(ex);

                    LogExceptionWithPeerId(ex, peerId);

                    // Compose error info
                    string errorMessage;
                    if (errorId.HasValue)
                    {
                        errorMessage = string.Format(ExceptionTemplates.CriticalErrorSavedTemplate,
                                                     errorId.Value, Path.Combine(
                                                                    vkSettings.Links.VkDomain,
                                                                    vkSettings.Links.EntbotSupportVkName));
                    }
                    else
                    {
                        errorMessage = string.Format(ExceptionTemplates.CriticalErrorSaveFailedTemplate,
                                                     Path.Combine(vkSettings.Links.VkDomain,
                                                                  vkSettings.Links.EntbotSupportVkName));
                    }

                    // Send error info
                    if (context.LocalPlayer?.PreviousAction != null)
                    {
                        result = new ReturnBackKeyboardResult(errorMessage, context.LocalPlayer.PreviousAction);
                    }
                    else
                    {
                        messages.Send(peerId, errorMessage);
                        return new HandleUpdateResult(success: false);
                    }

                    success = false;
                }
                #endregion

                finally
                {
                    context.LocalPlayer.PreviousAction = next;
                    context.LocalPlayer.PreviousResult = result;

                    var resultMessage = result.GetMessage();
                    if (resultMessage.Keyboard != null && !resultMessage.Keyboard.IsEmpty)
                    {
                        context.LocalPlayer.CurrentKeyboard = resultMessage.Keyboard;
                    }

                    var messageSendParams = resultMessage.ToMessagesSendParams(vkSettings, peerId, messageNew.ClientInfo);

                    messages.Send(messageSendParams);

                    StringBuilder sb = new StringBuilder();
                    sb.Append(JsonConvert.SerializeObject(context, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        PreserveReferencesHandling = PreserveReferencesHandling.All,
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: true,
                                                                         overrideSpecifiedNames: false)
                        },
                        TypeNameHandling = TypeNameHandling.Auto
                    }));
                    sb.Append(string.Join("", Enumerable.Repeat("-", 10)));
                    sb.Append(Environment.NewLine);

                    logger.LogInformation(sb.ToString());
                }

                return new HandleUpdateResult(success);
            }
            catch (Exception ex)
            {
                #region Any other exception handling
                Guid? errorId = await TrySaveExceptionAsync(ex);

                LogExceptionWithPeerId(ex, peerId);

                // Send error info
                string errorMessage;
                if (errorId.HasValue)
                {
                    errorMessage = string.Format(ExceptionTemplates.CriticalErrorSavedTemplate,
                                                 errorId.Value, Path.Combine(
                                                                vkSettings.Links.VkDomain,
                                                                vkSettings.Links.EntbotSupportVkName));
                }
                else
                {
                    errorMessage = string.Format(ExceptionTemplates.CriticalErrorSaveFailedTemplate,
                                                 Path.Combine(vkSettings.Links.VkDomain,
                                                              vkSettings.Links.EntbotSupportVkName));
                }

                messages.Send(peerId, errorMessage);
                return new HandleUpdateResult(success: false);
                #endregion
            }
        }

        private Task<HandleUpdateResult> Confirmation(ulong groupId)
        {
            VkGroupSetting group = vkSettings.Groups.Single(g => g.GroupId == groupId);

            return Task.FromResult(new HandleUpdateResult
            {
                Successful = true,
                Result = group.Confirmation
            });
        }



        /// <summary>
        /// Changes the last <paramref name="amountOfCharsToHide"/> chars of the <paramref name="key"/> to <paramref name="replacingChar"/> <br/>
        /// <br/>
        /// For example, "mysuperkey" with <paramref name="amountOfCharsToHide"/> = 3 and <paramref name="replacingChar"/> = '*' will result in "mysuper***"
        /// </summary>
        /// <param name="key">A <see cref="string"/> which characters to cloak</param>
        /// <param name="amountOfCharsToHide">Amount of characters to hide. By default, replaces a half of a key with <paramref name="replacingChar"/></param>
        /// <returns>The resulting key. <br/>
        /// If the given <paramref name="key"/> was <see langword="null"/>, empty or consisted only of white-space characters, returns it back.</returns>
        private string HideKey(string key, int amountOfCharsToHide = -1, char replacingChar = '*')
        {
            if (string.IsNullOrWhiteSpace(key))
                return key;

            if (amountOfCharsToHide < key.Length || amountOfCharsToHide <= 0)
                amountOfCharsToHide = key.Length / 2;

            return key.Substring(0, key.Length - amountOfCharsToHide) + string.Join("", Enumerable.Repeat(replacingChar, amountOfCharsToHide));
        }


        private async Task<LocalPlayer> AuthByVkIdAsync(long peerId, ILocalPlayerManager playerManager, EntbotApi api)
        {
            var localPlayer = playerManager.Get(peerId, PlayerManagerFilter.VkId);
            if (localPlayer == null)
            {
                var player = await api.Essences.Player.GetByVK(peerId);
                if (player == null)
                {
                    localPlayer = playerManager.AddNonAuthorized(peerId);
                }
                else
                {
                    localPlayer = playerManager.AddAuthorized(peerId, player.Id);
                }
            }
            return localPlayer;
        }

        private async Task ConfigureMenuContext(MenuContext context, Message message, ILocalPlayerManager localPlayerManager, EntbotApi api)
        {
            long peerId = message.PeerId.Value;
            context.Message = new MessageInfo
            {
                Text = message.Text,
                VkId = peerId
            };

            if (!string.IsNullOrEmpty(message.Payload))
            {
                string buttonPayload = JsonConvert.DeserializeObject<VkButtonPayload>(message.Payload).Payload;

                if (!KeyboardUtils.TryDecipherPayload(buttonPayload, out int pressedButton))
                    throw new InvalidMessagePayloadException($"{peerId} is trying to change the payload: {message.Payload}");

                context.Message.PressedButton = pressedButton;
            }
            var localPlayer = await AuthByVkIdAsync(peerId, localPlayerManager, api);
            context.LocalPlayer = localPlayer;
        }
    }
}
