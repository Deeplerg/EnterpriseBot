using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Infrastructure;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Utils;

namespace EnterpriseBot.VK.Models.MenuResults
{
    public class ReturnBackKeyboardResult : IMenuResult
    {
        private const string defaultReturnButtonText = "Вернуться назад";

        private readonly string message;
        private readonly LocalKeyboard keyboard;
        private readonly IMenuResult previousResult;
        private string returnButtonText;

        public ReturnBackKeyboardResult(string message, IMenuResult previousResult, string returnButtonText = defaultReturnButtonText)
        {
            this.message = message;
            this.previousResult = previousResult;
            this.returnButtonText = returnButtonText;

            var builder = new LocalKeyboardBuilder();
            builder.AddButton(new LocalKeyboardButton
            {
                Text = this.returnButtonText,
                Next = new NextAction
                {
                    PlainAction = _ => previousResult
                }
            });

            this.keyboard = builder.Build();
        }

        public ReturnBackKeyboardResult(string message, MenuContext context, string returnButtonText = defaultReturnButtonText)
            : this(message, (IMenuResult)context.LocalPlayer.PreviousResult.Clone(), returnButtonText) { }

        public VkMessage GetMessage()
        {
            return new VkMessage
            {
                Text = message,
                Keyboard = keyboard
            };
        }

        public NextAction GetNextAction(MenuContext context)
        {
            if (context.Message.PressedButton == null)
            {
                return new NextAction
                {
                    Menu = Constants.PayloadEmptyMenu,
                    MenuAction = Constants.PayloadEmptyMenuAction,
                    Parameters = new MenuParameter[]
                    {
                        new MenuParameter((IMenuResult)previousResult.Clone())
                    }
                };
            }
            else
            {
                return keyboard[context.Message.PressedButton.Value].Next;
            }
        }

        public object Clone()
        {
            return new ReturnBackKeyboardResult(message, (IMenuResult)previousResult.Clone(), returnButtonText);
        }
    }
}
