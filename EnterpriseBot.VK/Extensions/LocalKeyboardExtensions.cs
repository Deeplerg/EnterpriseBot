using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Utils;
using VkNet.Model.Keyboard;

namespace EnterpriseBot.VK.Extensions
{
    public static class LocalKeyboardExtensions
    {
        public static MessageKeyboard ToVkKeyboard(this LocalKeyboard keyboard, bool isOneTime = false, bool isInline = false)
        {
            return KeyboardUtils.LocalToVkKeyboard(keyboard, isOneTime, isInline);
        }
    }
}
