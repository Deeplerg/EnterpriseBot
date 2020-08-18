using System;
using System.Linq;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading.Tasks;
using EnterpriseBot.VK.Models.Other;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;

namespace EnterpriseBot.VK.Controllers
{
    public class VkController : Controller
    {
        private readonly ILogger<VkController> logger;
        private readonly IMessageNewUpdateHandler messageNewHandler;
        private readonly IConfirmationUpdateHandler confirmationHandler;
        private readonly VkSettings vkSettings;

        public VkController(ILogger<VkController> logger,
                            IOptions<VkSettings> vkOptions,
                            IMessageNewUpdateHandler messageNewHandler,
                            IConfirmationUpdateHandler confirmationHandler)
        {
            this.logger = logger;
            this.messageNewHandler = messageNewHandler;
            this.confirmationHandler = confirmationHandler;
            this.vkSettings = vkOptions.Value;
        }

        public async Task<IActionResult> Callback([FromBody] GroupUpdate update)
        {
            var result = await HandleUpdateAsync(update);

            if (result.Successful)
            {
                if (!string.IsNullOrEmpty(result.Result))
                {
                    return Ok(result.Result);
                }

                return Ok("ok");
            }
            else
            {
                if (vkSettings.ReturnOkEvenIfItIsNot)
                {
                    return Ok("ok"); //According to VK API Documentation,
                                     //"If a server returns several errors in a row, Callback API will temporarily stop sending notifications to it."
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Something went wrong");
                }
            }
        }
        
        /// <summary>
        /// Changes the last <paramref name="amountOfCharsToHide"/> chars of the <paramref name="key"/> to <paramref name="replacingChar"/> <br/>
        /// <br/>
        /// For example, "mysuperkey" with <paramref name="amountOfCharsToHide"/> = 3 and <paramref name="replacingChar"/> = '*' will result in "mysuper***"
        /// </summary>
        /// <param name="key">A <see cref="string"/> which characters to cloak</param>
        /// <param name="amountOfCharsToHide">Amount of characters to hide. By default, replaces a half of a key with <paramref name="replacingChar"/></param>
        /// <param name="replacingChar">A char to replace characters with</param>
        /// <returns>The resulting key. <br/>
        /// If the given <paramref name="key"/> was <see langword="null"/>, empty or consisted only of white-space characters, returns it back.</returns>
        [NonAction]
        private string HideKey(string key, int amountOfCharsToHide = -1, char replacingChar = '*')
        {
            if (string.IsNullOrWhiteSpace(key))
                return key;

            if (amountOfCharsToHide < key.Length || amountOfCharsToHide <= 0)
                amountOfCharsToHide = key.Length / 2;

            return key.Substring(0, key.Length - amountOfCharsToHide) + string.Join("", Enumerable.Repeat(replacingChar, amountOfCharsToHide));
        }

        [NonAction]
        private async Task<HandleUpdateResult> HandleUpdateAsync(GroupUpdate update)
        {
            if (update == null)
            {
                logger.LogWarning("Update was null");
                return new HandleUpdateResult(success: false);
            }
            
            if (update.GroupId != vkSettings.GroupId)
            {
                logger.LogWarning($"Id of a group received in {nameof(GroupUpdate)} ({update.GroupId}) " +
                                  $"does not match id of the group in {nameof(VkSettings)} ({vkSettings.GroupId})");

                return new HandleUpdateResult(success: false);
            }
            
            if (update.Secret != vkSettings.SecretKey)
            {
                string trueKey = vkSettings.SecretKey;
                string hiddenTrueKey = HideKey(trueKey, trueKey.Length / 3);

                logger.LogWarning($"Wrong secret key! Excepted: {hiddenTrueKey}, but was: {update.Secret}");

                return new HandleUpdateResult(success: false);
            }

            try
            {
                switch (update.Type)
                {
                    case var _ when update.Type == GroupUpdateType.MessageNew:
                        bool success = await messageNewHandler.Handle(update.MessageNew);

                        return new HandleUpdateResult(success);

                    case var _ when update.Type == GroupUpdateType.Confirmation:
                        string confirmation = await confirmationHandler.Handle(update);
                        
                        return new HandleUpdateResult
                        {
                            Successful = true,
                            Result = confirmation
                        };

                    default:
                        return new HandleUpdateResult(success: false);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return new HandleUpdateResult(success: false);
            }
        }
    }
}