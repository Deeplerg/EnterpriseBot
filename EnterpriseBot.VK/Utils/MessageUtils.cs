using EnterpriseBot.VK.Extensions;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Models.Settings;
using System;
using System.IO;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace EnterpriseBot.VK.Utils
{
    public static class MessageUtils
    {
        public static MessagesSendParams VkMessageToMessagesSendParams(VkMessage message, VkSettings settings, long peerId, ClientInfo clientInfo)
        {
            MessagesSendParams pars = new MessagesSendParams();

            if (string.IsNullOrWhiteSpace(message.Text))
            {
                throw new ArgumentException($"Text shouldn't be null, empty or consist exclusively of white-space characters");
            }
            pars.RandomId = 0;
            pars.PeerId = peerId;
            pars.Message = message.Text;

            if (message.Keyboard != null)
            {
                if (clientInfo.Keyboard)
                {
                    pars.Keyboard = message.Keyboard.ToVkKeyboard(isInline: clientInfo.InlineKeyboard);
                }
                else
                {
                    pars.Message += "\n" + string.Format(ExceptionTemplates.KeyboardNotSupportedTemplate,
                                                         Path.Combine(settings.Links.VkDomain,
                                                                      settings.Links.EntbotSupportVkName));
                }
            }

            return pars;
        }

        public static string HideVkNameIntoText(string vkName, string text)
        {
            return string.Format("[{0}|{1}]",
                                 vkName, text);
        }
    }
}
