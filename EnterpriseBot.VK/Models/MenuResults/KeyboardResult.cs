using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Infrastructure;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseBot.VK.Models.MenuResults
{
    public class KeyboardResult : IMenuResult
    {
        private readonly LocalKeyboard localKeyboard;
        private readonly string text;

        public KeyboardResult(string text, LocalKeyboard localKeyboard)
        {
            this.localKeyboard = localKeyboard;
            this.text = text;
        }

        public KeyboardResult(string text, IEnumerable<IEnumerable<LocalKeyboardButton>> buttons)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(string.Format(ExceptionTemplates.StringNullOrEmptyTemplate,
                                                          nameof(text)));
            }
            if (!buttons.Any())
            {
                throw new ArgumentException($"{nameof(buttons)} cannot be empty");
            }

            int buttonsCount = buttons.Sum(buttonList => buttonList.Count());
            if (buttonsCount > LocalKeyboardBuilder.MaxButtons)
            {
                throw new ArgumentOutOfRangeException($"Max buttons: {LocalKeyboardBuilder.MaxButtons}, but was: {buttonsCount}");
            }


            var builder = new LocalKeyboardBuilder();
            int currentLineButtonsCount = 0;
            bool hasLastIterationAddedLine = false;

            foreach (var buttonList in buttons)
            {
                foreach (var button in buttonList)
                {
                    if (currentLineButtonsCount == LocalKeyboardBuilder.MaxButtonsPerLine)
                    {
                        builder.AddLine();
                        currentLineButtonsCount = 0;
                        hasLastIterationAddedLine = true;
                        continue;
                    }

                    builder.AddButton(button);
                    currentLineButtonsCount++;
                }

                if (!hasLastIterationAddedLine)
                {
                    builder.AddLine();
                }
                currentLineButtonsCount = 0;
            }

            this.localKeyboard = builder.Build();
            this.text = text;
        }

        public VkMessage GetMessage()
        {
            return new VkMessage
            {
                Text = text,
                Keyboard = localKeyboard
            };
        }

        public NextAction GetNextAction(MenuContext context)
        {
            if (context.Message.PressedButton == null)
            {
                return new NextAction
                {
                    Menu = Constants.PayloadEmptyMenu,
                    MenuAction = Constants.PayloadEmptyMenuAction
                };
            }

            if (context.Message.PressedButton >= localKeyboard.ButtonCount || context.Message.PressedButton < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(context.Message.PressedButton)} must not be more than or equal to button count or be lower than 0");
            }

            return localKeyboard[context.Message.PressedButton.Value].Next;
        }

        public object Clone()
        {
            return new KeyboardResult(text, (LocalKeyboard)localKeyboard.Clone());
        }
    }
}
