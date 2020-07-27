using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Models.Player;
using System;

namespace EnterpriseBot.VK.Models.MenuRelated
{
    public class MenuContext : ICloneable
    {
        public LocalPlayer LocalPlayer { get; set; }
        public MessageInfo Message { get; set; }

        public object Clone()
        {
            return new MenuContext
            {
                LocalPlayer = (LocalPlayer)LocalPlayer.Clone(),
                Message = (MessageInfo)Message.Clone()
            };
        }
    }
}
