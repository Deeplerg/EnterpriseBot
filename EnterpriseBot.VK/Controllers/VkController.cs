using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading.Tasks;
using VkNet.Model.GroupUpdate;

namespace EnterpriseBot.VK.Controllers
{
    public class VkController : Controller
    {
        private readonly ILogger<VkController> logger;
        private readonly IVkUpdateHandler updateHandler;
        private readonly VkSettings vkSettings;

        public VkController(ILogger<VkController> logger, IVkUpdateHandler vkUpdateHandler, IOptions<VkSettings> vkOptions)
        {
            this.logger = logger;
            this.updateHandler = vkUpdateHandler;
            this.vkSettings = vkOptions.Value;
        }

        public async Task<IActionResult> Callback([FromBody] GroupUpdate update)
        {
            if (update == null)
            {
                logger.LogDebug("Update was null");
                return BadRequest("Update was null");
            }

            var result = await updateHandler.HandleUpdateAsync(update);
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
    }
}