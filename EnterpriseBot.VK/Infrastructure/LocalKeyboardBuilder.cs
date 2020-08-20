using EnterpriseBot.VK.Exceptions;
using EnterpriseBot.VK.Extensions;
using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VkNet.Enums.SafetyEnums;

namespace EnterpriseBot.VK.Infrastructure
{
    public class LocalKeyboardBuilder
    {
        private readonly List<List<LocalKeyboardButton>> buttons = new List<List<LocalKeyboardButton>>();
        private List<LocalKeyboardButton> currentLine = new List<LocalKeyboardButton>();

        public const int MaxButtonsPerLine = 4;
        public const int MaxButtonLines = 10;
        public const int MaxButtons = 20;

        public int CurrentButtonsCount
        {
            get
            {
                return buttons.Sum(buttonList => buttonList.Count) + currentLine.Count;
            }
        }

        public LocalKeyboardBuilder AddButton(LocalKeyboardButton button)
        {
            if (string.IsNullOrWhiteSpace(button.Text))
            {
                throw new ArgumentNullException(nameof(button), "The text must not be empty or consist exclusively of white-space characters");
            }

            if (CurrentButtonsCount + 1 > MaxButtons)
            {
                throw new KeyboardMaxButtonsException($"Max buttons: {MaxButtons}");
            }

            if (currentLine.Count + 1 > MaxButtonsPerLine)
            {
                throw new KeyboardMaxButtonsException($"Max buttons per line: {MaxButtonsPerLine}");
            }


            currentLine.Add(button);

            return this;
        }

        public LocalKeyboardBuilder AddButton(string text, Type nextMenu)
        {
            return AddButton(text, nextMenu, null);
        }

        public LocalKeyboardBuilder AddButton(string text, MethodInfo action, params MenuParameter[] actionParameters)
        {
            return AddButton(text, action.DeclaringType, action);
        }

        public LocalKeyboardBuilder AddButton(string text, Type nextMenu, MethodInfo nextMenuAction, params MenuParameter[] menuParameters)
        {
            return AddButton(new LocalKeyboardButton
            {
                Text = text,
                Next = new NextAction(nextMenu, nextMenuAction, menuParameters),
                Color = LocalKeyboardButtonColor.Default
            });
        }

        public LocalKeyboardBuilder AddLine()
        {
            if (buttons.Count + 1 > MaxButtonLines)
            {
                throw new KeyboardMaxButtonsException($"Max button lines: {MaxButtonLines}");
            }

            buttons.Add(currentLine);
            currentLine = new List<LocalKeyboardButton>();

            return this;
        }

        public LocalKeyboardBuilder Clear()
        {
            buttons.Clear();
            currentLine.Clear();

            return this;
        }

        public LocalKeyboard Build()
        {
            if (currentLine.Any())
            {
                AddLine();
            }

            return new LocalKeyboard
            {
                Buttons = buttons.Select(list => list.ToReadOnlyCollection()).ToReadOnlyCollection()
            };
        }
    }
}
