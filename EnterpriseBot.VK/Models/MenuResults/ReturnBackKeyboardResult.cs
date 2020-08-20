using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Infrastructure;
using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Utils;
using Newtonsoft.Json;

namespace EnterpriseBot.VK.Models.MenuResults
{
    [JsonObject(MemberSerialization.Fields)]
    public class ReturnBackKeyboardResult : IMenuResult
    {
        private string message;
        private string returnButtonText;
        private NextAction previousAction;
        private bool showReturnToMainMenuButton;
        
        public bool IsSuccessfulResult { get; } = false;

        public ReturnBackKeyboardResult(string message, MenuContext menuContext, string returnButtonText = Constants.ReturnBackMenuDefaultButtonText)
                                 : this(message, menuContext.LocalPlayer.LastSuccessfulAction, returnButtonText, menuContext.LocalPlayer.IsAuthorized) { }

        public ReturnBackKeyboardResult(string message, NextAction toAction, string returnButtonText = Constants.ReturnBackMenuDefaultButtonText, bool showReturnToMainMenuButton = true)
        {
            this.message = message;
            this.returnButtonText = returnButtonText;
            this.previousAction = (NextAction)toAction.Clone();
            this.showReturnToMainMenuButton = showReturnToMainMenuButton;
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
            return new ReturnBackKeyboardResult(message, (NextAction)previousAction.Clone(), returnButtonText, showReturnToMainMenuButton);
        }

        private LocalKeyboard BuildKeyboard(string returnButtonText)
        {
            var builder = new LocalKeyboardBuilder();
            builder.AddButton(new LocalKeyboardButton
            {
                Text = returnButtonText,
                Next = previousAction,
                Color = LocalKeyboardButtonColor.Primary
            });
            builder.AddLine();
            
            if(showReturnToMainMenuButton)
            {
                builder.AddButton(new LocalKeyboardButton
                {
                    Text = "В главное меню",
                    Next = new NextAction(menu: Constants.MainMenu,
                                          action: Constants.MainMenuMainAction)
                });
            }
            
            var keyboard = builder.Build();

            return keyboard;
        }
    }
}
