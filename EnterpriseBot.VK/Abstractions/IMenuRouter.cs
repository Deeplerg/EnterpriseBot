using EnterpriseBot.VK.Models.MenuRelated;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IMenuRouter
    {
        IMenu GetMenu(Type menuType);
        IMenu CreateMenuOfType<T>(T menuType, params object[] parameters) where T : Type;
        Task<IMenuResult> InvokeMenuActionAsync(MethodInfo action, IMenu menuInstance, params MenuParameter[] invocationParams);
    }
}
