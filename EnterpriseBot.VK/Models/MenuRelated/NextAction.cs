using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Utils;
using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace EnterpriseBot.VK.Models.MenuRelated
{
    public class NextAction : ICloneable
    {
        private NextAction() { }

        public NextAction(Type menu, string methodName = nameof(IMenu.DefaultMenuLayout), params MenuParameter[] pars)
                   : this(menu, TypeUtils.GetMethod(menu, methodName), pars) { }

        public NextAction(Type menu, MethodInfo action, params MenuParameter[] pars)
        {
            MenuAssemblyName = menu.Assembly.GetName().Name;
            MenuTypeName = menu.Name;
            MenuNamespace = menu.Namespace;
            MenuActionMethodName = action?.Name;
            Parameters = pars;
        }

        public string MenuAssemblyName { get; set; }
        public string MenuNamespace { get; set; }
        public string MenuTypeName { get; set; }
        public string MenuActionMethodName { get; set; }
        public MenuParameter[] Parameters { get; set; }

        public object Clone()
        {
            return new NextAction
            {
                MenuAssemblyName = MenuAssemblyName,
                MenuNamespace = MenuNamespace,
                MenuTypeName = MenuTypeName,
                MenuActionMethodName = MenuActionMethodName
            };
        }
    }
}
