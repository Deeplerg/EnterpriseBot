using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.MenuRelated;

namespace EnterpriseBot.VK.Models.Keyboard
{
    public class LocalKeyboardButton
    {
        public string Text { get; set; }

        public NextAction Next { get; set; }

        public LocalKeyboardButtonColor Color { get; set; }
    }
}
