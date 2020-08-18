using System.Threading.Tasks;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Exceptions;
using EnterpriseBot.VK.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VkNet.Model.GroupUpdate;

namespace EnterpriseBot.VK.Services
{
    public class ConfirmationUpdateHandler : IConfirmationUpdateHandler
    {
        private readonly ILogger<ConfirmationUpdateHandler> logger;
        private readonly VkSettings vkSettings;
        
        public ConfirmationUpdateHandler(ILogger<ConfirmationUpdateHandler> logger,
                                         IOptions<VkSettings> vkSettings)
        {
            this.logger = logger;
            this.vkSettings = vkSettings.Value;
        }
        
        public Task<string> Handle(GroupUpdate update)
        {
            if (update.GroupId != vkSettings.GroupId)
            {
                string message = $"Id of a group received in {nameof(GroupUpdate)} ({update.GroupId}) " +
                                 $"does not match id of the group in {nameof(VkSettings)} ({vkSettings.GroupId})";
                
                throw new InvalidGroupIdException(message);
            }
            
            return Task.FromResult(vkSettings.Confirmation);
        }
    }
}