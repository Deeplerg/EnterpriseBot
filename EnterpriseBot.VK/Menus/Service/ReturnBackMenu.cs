using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace EnterpriseBot.VK.Menus.Service
{
    public class ReturnBackMenu : MenuBase
    {
        private readonly Type thisType;
        private readonly VkLinksSetting links;

        public ReturnBackMenu(IOptions<VkSettings> vkOptions)
        {
            this.thisType = this.GetType();
            this.links = vkOptions.Value.Links;
        }

        public IMenuResult ReturnBackToResult(string message, IMenuResult result, string returnButtonText = Constants.ReturnBackMenuDefaultButtonText)
        {
            return Keyboard(message, new LocalKeyboardButton
            {
                Text = returnButtonText,
                Next = new NextAction(thisType, nameof(ReturnResult), new MenuParameter[]
                {
                    new MenuParameter(result)
                })
            });
        }

        public IMenuResult ReturnResult(IMenuResult result)
        {
            return result;
        }


        public override IMenuResult DefaultMenuLayout()
        {
            string message = string.Format(ExceptionTemplates.CriticalErrorSaveFailedTemplate,
                                           Path.Combine(links.VkDomain,
                                                        links.EntbotSupportVkName));

            return Keyboard(message, new LocalKeyboardButton
            {
                Text = "В главное меню",
                Next = new NextAction(Constants.MainMenu),
                Color = LocalKeyboardButtonColor.Primary
            });
        }
    }
}
