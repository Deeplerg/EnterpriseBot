using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Utils;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace EnterpriseBot.VK.Extensions
{
    public static class VkMessageExtensions
    {
        public static MessagesSendParams ToMessagesSendParams(this VkMessage message, VkSettings settings, long peerId, ClientInfo clientInfo, bool isInline = false)
        {
            return MessageUtils.VkMessageToMessagesSendParams(message, settings, peerId, clientInfo, isInline);
        }
    }
}
