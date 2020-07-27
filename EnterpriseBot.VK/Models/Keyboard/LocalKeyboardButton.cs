using EnterpriseBot.VK.Models.MenuRelated;
using VkNet.Enums.SafetyEnums;

namespace EnterpriseBot.VK.Models.Keyboard
{
    public class LocalKeyboardButton
    {
        public string Text { get; set; }

        public NextAction Next { get; set; }

        public KeyboardButtonColor Color { get; set; }
    }
}
