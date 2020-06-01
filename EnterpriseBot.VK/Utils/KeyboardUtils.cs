using EnterpriseBot.VK.Models.Keyboard;
using System;
using VkNet.Model.Keyboard;

namespace EnterpriseBot.VK.Utils
{
    public static class KeyboardUtils
    {
        public static MessageKeyboard LocalToVkKeyboard(LocalKeyboard keyboard, bool isOneTime = false, bool isInline = false)
        {
            var builder = new KeyboardBuilder(isOneTime);

            int buttonCount = 0;
            foreach (var buttonLine in keyboard.Buttons)
            {
                foreach (var button in buttonLine)
                {
                    builder.AddButton(label: button.Text,
                                      extra: CreatePayload(buttonCount),
                                      color: button.Color,
                                      type: button.ButtonAction.ToString());

                    buttonCount++;
                }

                builder.AddLine();
            }

            if (isInline)
            {
                builder.SetInline(true);
            }

            return builder.Build();
        }

        public static string CreatePayload(int buttonNumber)
        {
            if (buttonNumber < 0 || buttonNumber > 99)
            {
                throw new ArgumentOutOfRangeException($"Invalid {nameof(buttonNumber)} range");
            }

            return buttonNumber.ToString();
        }

        public static bool TryDecipherPayload(string buttonPayload, out int buttonNumber)
        {
            if (!int.TryParse(buttonPayload, out buttonNumber)
                || buttonNumber < 0 || buttonNumber > 99)
            {
                return false;
            }

            return true;
        }
    }
}
