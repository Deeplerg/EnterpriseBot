using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using System;
using VkNet.Enums.SafetyEnums;
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
                                      color: LocalButtonColorToVk(button.Color),
                                      type: KeyboardButtonActionType.Text.ToString());

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

        public static NextAction GetNextActionFromKeyboard(MenuContext context, LocalKeyboard keyboard = null)
        {
            if (keyboard == null) keyboard = context.LocalPlayer.CurrentKeyboard;

            if (context.Message.PressedButton == null)
            {
                return new NextAction(Constants.PayloadEmptyMenu, Constants.PayloadEmptyMenuAction);
            }

            if (context.Message.PressedButton >= keyboard.ButtonCount || context.Message.PressedButton < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(context.Message.PressedButton)} must not be more than or equal to button count or be lower than 0");
            }

            return keyboard[context.Message.PressedButton.Value].Next;
        }

        public static LocalKeyboardButtonColor VkButtonColorToLocal(KeyboardButtonColor vkColor)
        {
            switch(vkColor)
            {
                case var _ when vkColor == KeyboardButtonColor.Primary:
                    return LocalKeyboardButtonColor.Primary;

                case var _ when vkColor == KeyboardButtonColor.Positive:
                    return LocalKeyboardButtonColor.Positive;

                case var _ when vkColor == KeyboardButtonColor.Negative:
                    return LocalKeyboardButtonColor.Negative;

                default:
                    return LocalKeyboardButtonColor.Default;
            }
        }

        public static KeyboardButtonColor LocalButtonColorToVk(LocalKeyboardButtonColor localColor)
        {
            switch (localColor)
            {
                case LocalKeyboardButtonColor.Primary:
                    return KeyboardButtonColor.Primary;

                case LocalKeyboardButtonColor.Positive:
                    return KeyboardButtonColor.Positive;

                case LocalKeyboardButtonColor.Negative:
                    return KeyboardButtonColor.Negative;

                default:
                    return KeyboardButtonColor.Default;
            }
        }
    }
}
