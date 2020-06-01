using EnterpriseBot.VK.Abstractions;
using System;
using System.Reflection;

namespace EnterpriseBot.VK.Models.MenuRelated
{
    public class NextAction
    {
        public Type Menu { get; set; }
        public MethodInfo MenuAction { get; set; }
        public MenuParameter[] Parameters { get; set; }

        public Func<MenuContext, IMenuResult> PlainAction { get; set; }
    }
}
