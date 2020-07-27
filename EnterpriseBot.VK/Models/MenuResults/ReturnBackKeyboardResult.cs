using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Infrastructure;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;

namespace EnterpriseBot.VK.Models.MenuResults
{
    [JsonObject(MemberSerialization.Fields)]
    public class ReturnBackKeyboardResult : IMenuResult
    {
        private string message;
        private string returnButtonText;
        private NextAction previousAction;

        public ReturnBackKeyboardResult(string message, MenuContext menuContext, string returnButtonText = Constants.ReturnBackMenuDefaultButtonText)
                                 : this(message, menuContext.LocalPlayer.PreviousAction, returnButtonText) { }

        public ReturnBackKeyboardResult(string message, NextAction previousAction, string returnButtonText = Constants.ReturnBackMenuDefaultButtonText)
        {
            this.message = message;
            this.returnButtonText = returnButtonText;
            this.previousAction = (NextAction)previousAction.Clone();
        }

        public VkMessage GetMessage()
        {
            return new VkMessage
            {
                Text = message,
                Keyboard = BuildKeyboard(returnButtonText)
            };
        }

        public NextAction GetNextAction(MenuContext context)
        {
            return KeyboardUtils.GetNextActionFromKeyboard(context);
        }

        public object Clone()
        {
            return new ReturnBackKeyboardResult(message, (NextAction)previousAction.Clone(), returnButtonText);
        }

        private LocalKeyboard BuildKeyboard(string returnButtonText)
        {
            var builder = new LocalKeyboardBuilder();
            builder.AddButton(new LocalKeyboardButton
            {
                Text = returnButtonText,
                Next = previousAction,
                Color = KeyboardButtonColor.Primary
            });
            builder.AddLine();
            builder.AddButton(new LocalKeyboardButton
            {
                Text = "В главное меню",
                Next = new NextAction(menu: Constants.MainMenu,
                                      action: Constants.MainMenuMainAction)
            });
            var keyboard = builder.Build();

            return keyboard;
        }
    }
}
