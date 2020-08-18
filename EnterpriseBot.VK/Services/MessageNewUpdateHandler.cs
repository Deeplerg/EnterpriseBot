using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.ApiWrapper.Exceptions;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Exceptions;
using EnterpriseBot.VK.Extensions;
using EnterpriseBot.VK.Models.Contexts;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.MenuResults;
using EnterpriseBot.VK.Models.Other;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VkNet.Model.GroupUpdate;

namespace EnterpriseBot.VK.Services
{
    public class MessageNewUpdateHandler : IMessageNewUpdateHandler
    {
        private readonly ILogger<MessageNewUpdateHandler> logger;
        private readonly IMenuContextConfigurator contextConfigurator;
        private readonly VkSettings vkSettings;
        private readonly IVkMessageGateway messages;
        private readonly IMenuMapper menuMapper;
        private readonly IMenuRouter menuRouter;
        private readonly ILocalPlayerManager playerManager;
        private readonly ErrorDbContext errorCtx;

        public MessageNewUpdateHandler(ILogger<MessageNewUpdateHandler> logger,
                                       IMenuContextConfigurator contextConfigurator,
                                       IOptions<VkSettings> vkSettings,
                                       IVkMessageGateway messages,
                                       IMenuMapper menuMapper,
                                       IMenuRouter menuRouter,
                                       ILocalPlayerManager playerManager,
                                       ErrorDbContext errorCtx)
        {
            this.logger = logger;
            this.contextConfigurator = contextConfigurator;
            this.vkSettings = vkSettings.Value;
            this.messages = messages;
            this.menuMapper = menuMapper;
            this.menuRouter = menuRouter;
            this.playerManager = playerManager;
            this.errorCtx = errorCtx;
        }
        
        public async Task<bool> Handle(MessageNew messageNew)
        {
            bool success = false;
            
            var message = messageNew.Message;
            long peerId = message.PeerId.Value;

            try
            {
                MenuContext context;

                try
                {
                    context = await contextConfigurator.Configure(message);
                }
                #region Exception handling
                catch (InvalidMessagePayloadException ex)
                {
                    LogExceptionWithPeerId(ex, peerId);

                    string supportLink = Path.Combine(vkSettings.Links.VkDomain,
                                                      vkSettings.Links.EntbotSupportVkName);
                    
                    string invalidPayloadMessage = string.Format(ExceptionTemplates.InvalidMessagePayloadWarnTemplate,
                                                                 supportLink);

                    messages.Send(peerId, invalidPayloadMessage);

                    return true;
                }
                #endregion

                IMenuResult result = null;
                NextAction action = null;
                Type menuType = null;
                
                try
                {
                    action = menuMapper.MapAction(context);
                    result = await menuMapper.InvokeAction(action, context, menuRouter,
                                       menuCreationParams: context);
                    
                    menuType = menuMapper.GetMenuTypeForAction(action);
                    
                    success = true;
                }
                
                #region Exception handling
                catch (ApiNormalException ex)
                {
                    LogExceptionWithPeerId(ex, peerId);

                    if (context.LocalPlayer?.PreviousAction != null)
                    {
                        result = new ReturnBackKeyboardResult(ex.Message, context);
                    }
                    else
                    {
                        messages.Send(peerId, ex.Message);
                        return false;
                    }

                    success = false;
                }
                
                catch (Exception ex)
                {
                    Guid? errorId = await TrySaveExceptionAsync(ex);

                    LogExceptionWithPeerId(ex, peerId);
                    
                    string errorMessage = ComposeCriticalErrorMessage(errorId);

                    // Send error info
                    if (context.LocalPlayer?.PreviousAction != null)
                    {
                        result = new ReturnBackKeyboardResult(errorMessage, context);
                    }
                    else
                    {
                        messages.Send(peerId, errorMessage);
                        return false;
                    }

                    success = false;
                }
                #endregion

                finally
                {
                    context.LocalPlayer.PreviousAction = action;
                    context.LocalPlayer.PreviousResult = result;
                    
                    var resultMessage = result.GetMessage();
                    if (resultMessage.Keyboard != null && !resultMessage.Keyboard.IsEmpty)
                    {
                        context.LocalPlayer.CurrentKeyboard = resultMessage.Keyboard;
                    }
                    
                    if (success && result.IsSuccessfulResult)
                    {
                        context.LocalPlayer.LastSuccessfulAction = action;
                        context.LocalPlayer.LastSuccessfulMenuType = menuType;
                    }
                    
                    var messageSendParams = resultMessage.ToMessagesSendParams(vkSettings, peerId, messageNew.ClientInfo);

                    playerManager.Update(context.LocalPlayer);

                    messages.Send(messageSendParams);
                }

                return success;
            }
            catch (Exception ex)
            {
                #region Any other exception handling
                Guid? errorId = await TrySaveExceptionAsync(ex);

                LogExceptionWithPeerId(ex, peerId);

                // Send error info
                string errorMessage = ComposeCriticalErrorMessage(errorId);

                messages.Send(peerId, errorMessage);
                return false;
                #endregion
            }
        }
        
        private void LogExceptionWithPeerId(Exception ex, long peerId)
        {
            string logErrorMessage = $"{ex.GetType().Name} from {peerId}: {string.Join("", ex.Message.Take(20))}";
            if (ex.Message.Length > 20)
            {
                logErrorMessage += "...";
            }

            logger.LogError(ex, logErrorMessage);
        }
        
        private async Task<Guid?> TrySaveExceptionAsync(Exception ex)
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

        private string ComposeCriticalErrorMessage(Guid? errorId = null)
        {
            string errorMessage;
            string supportLink = Path.Combine(vkSettings.Links.VkDomain,
                                              vkSettings.Links.EntbotSupportVkName);
                    
            if (errorId.HasValue)
            {
                errorMessage = string.Format(ExceptionTemplates.CriticalErrorSavedTemplate,
                                             errorId.Value, supportLink);
            }
            else
            {
                errorMessage = string.Format(ExceptionTemplates.CriticalErrorSaveFailedTemplate,
                                             supportLink);
            }

            return errorMessage;
        }
    }
}