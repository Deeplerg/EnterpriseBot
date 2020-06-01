using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using System;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IMenuResult : ICloneable
    {
        VkMessage GetMessage();
        NextAction GetNextAction(MenuContext context);
    }
}
