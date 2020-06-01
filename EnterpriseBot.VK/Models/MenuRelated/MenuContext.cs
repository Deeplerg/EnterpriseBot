using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Models.Player;

namespace EnterpriseBot.VK.Models.MenuRelated
{
    public class MenuContext
    {
        public LocalPlayer LocalPlayer { get; set; }
        public MessageInfo Message { get; set; }
    }
}
