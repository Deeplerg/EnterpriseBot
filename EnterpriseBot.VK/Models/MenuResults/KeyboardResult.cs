using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Infrastructure;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseBot.VK.Models.MenuResults
{
    [JsonObject(MemberSerialization.Fields)]
    public class KeyboardResult : IMenuResult
    {
        [JsonIgnore] // No need to serialize this, because there is MenuContext.LocalPlayer.CurrentKeyboard
        private readonly LocalKeyboard localKeyboard;
        private readonly string text;

        public bool IsSuccessfulResult { get; } = true;
        
        public KeyboardResult(string text, LocalKeyboard localKeyboard)
        {
            this.localKeyboard = localKeyboard;
            this.text = text;
        }

        public KeyboardResult(string text, LocalKeyboardButton button)
                       : this(text, new List<LocalKeyboardButton> { button }) { }
        
        public KeyboardResult(string text, IEnumerable<LocalKeyboardButton> buttons)
                       : this(text, new List<IEnumerable<LocalKeyboardButton>> { buttons }) { }

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
            return KeyboardUtils.GetNextActionFromKeyboard(context, localKeyboard);
        }

        public object Clone()
        {
            return new KeyboardResult(text, (LocalKeyboard)localKeyboard.Clone());
        }
    }
}
