using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using System;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IMenuResult : ICloneable
    {
        bool IsSuccessfulResult { get; }
        VkMessage GetMessage();
        NextAction GetNextAction(MenuContext context);
    }
}
