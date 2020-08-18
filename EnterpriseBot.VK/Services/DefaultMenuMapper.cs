using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Utils;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EnterpriseBot.VK.Services
{
    public class DefaultMenuMapper : IMenuMapper
    {
        public NextAction MapAction(MenuContext context)
        {
            var localPlayer = context.LocalPlayer;

            if (localPlayer == null)
                throw new ArgumentNullException(nameof(context.LocalPlayer));

            if (!localPlayer.IsAuthorized)
            {
                if (localPlayer.PreviousResult == null)
                {
                    return CreateAction(Constants.AuthMenu, Constants.AuthMenuAuthAction);
                }

                return localPlayer.PreviousResult.GetNextAction(context);
            }
            else
            {
                if (localPlayer.PreviousResult == null)
                {
                    return CreateAction(Constants.MainMenu, Constants.MainMenuAfterRestartAction);
                }

                return localPlayer.PreviousResult.GetNextAction(context);
            }
        }

        public async Task<IMenuResult> InvokeAction(NextAction next, MenuContext context, IMenuRouter menuRouter, params object[] menuCreationParams)
        {
            Type menu = MapMenuType(next);
            MethodInfo action = MapMenuMethod(next, menu);

            var menuInstance = menuRouter.CreateMenuOfType(menu, menuCreationParams);

            return await menuRouter.InvokeMenuActionAsync(action, menuInstance, invocationParams: next.Parameters);
        }

        public Type GetMenuTypeForAction(NextAction next) => MapMenuType(next);

        private NextAction CreateAction(Type menuType, MethodInfo menuAction = null, MenuParameter[] pars = null)
        {
            if (menuType == null)
            {
                throw new ArgumentNullException($"{nameof(menuType)} cannot be null");
            }
            if (menuType.GetInterface(nameof(IMenu)) == null)
            {
                throw new ArgumentException($"{menuType} is not a menu");
            }

            //var menuAction ??= TypeUtils.GetMethod(menuType, nameof(IMenu.DefaultMenuLayout));
            if (menuAction is null) menuAction = TypeUtils.GetMethod(menuType, nameof(IMenu.DefaultMenuLayout));

            return new NextAction(menuType, menuAction, pars);
        }

        private Type MapMenuType(NextAction next)
        {
            var assembly = AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .FirstOrDefault(m => m.GetName().Name == next.MenuAssemblyName);
            if(assembly == null)
            {
                throw new ArgumentException($"Can't find assembly specified: {next.MenuAssemblyName}");
            }

            Type menu;

            string menuTypeName = next.MenuTypeName;
            if(string.IsNullOrEmpty(menuTypeName))
            {
                menuTypeName = nameof(IMenu.DefaultMenuLayout);
            }

            var menus = assembly.GetTypes().Where(m => m.Name == next.MenuTypeName).ToList();

            if(menus.Count > 1)
            {
                menu = menus.FirstOrDefault(m => m.Namespace == next.MenuNamespace);
            }
            else
            {
                menu = menus.SingleOrDefault();
            }

            if(menu == null)
            {
                throw new ArgumentException($"Can't find menu specified: {next.MenuTypeName}");
            }
            if (menu.GetInterface(nameof(IMenu)) == null)
            {
                throw new ArgumentException($"{menu} is not a menu");
            }

            return menu;
        }

        private MethodInfo MapMenuMethod(NextAction nextAction, Type menuType)
        {
            var method = TypeUtils.GetMethod(menuType, nextAction.MenuActionMethodName);

            MethodInfo action = method ?? TypeUtils.GetMethod(menuType, nameof(IMenu.DefaultMenuLayout));

            return action;
        }
    }
}
