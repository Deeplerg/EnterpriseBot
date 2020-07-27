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
        public static MessagesSendParams VkMessageToMessagesSendParams(VkMessage message, VkSettings settings, long peerId, ClientInfo clientInfo, bool isInline = false)
        {
            if (string.IsNullOrWhiteSpace(message.Text))
            {
                throw new ArgumentException($"Text shouldn't be null, empty or consist exclusively of white-space characters");
            }

            MessagesSendParams pars = new MessagesSendParams
            {
                RandomId = 0,
                PeerId = peerId,
                Message = message.Text
            };

            if (message.Keyboard != null)
            {
                if (clientInfo.Keyboard)
                {
                    pars.Keyboard = message.Keyboard.ToVkKeyboard(isInline);
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
