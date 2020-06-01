using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Utils;
using System;
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
                if (localPlayer?.PreviousResult == null)
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
            if (next.PlainAction != null)
            {
                return next.PlainAction.Invoke(context);
            }
            if (next.Menu == null)
            {
                throw new ArgumentNullException($"{nameof(next.Menu)} cannot be null unless a {nameof(next.PlainAction)} specified");
            }
            if (next.Menu.GetInterface(nameof(IMenu)) == null)
            {
                throw new ArgumentException($"{next.Menu} is not a menu");
            }

            MethodInfo action = next.MenuAction ?? next.Menu.GetMethod(nameof(IMenu.DefaultMenuLayout));

            var menuInstance = menuRouter.CreateMenuOfType(next.Menu, menuCreationParams);

            return await menuRouter.InvokeMenuActionAsync(action, menuInstance, invocationParams: next.Parameters);
        }

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

            return new NextAction
            {
                Menu = menuType,
                MenuAction = menuAction ?? menuType.GetMethod(nameof(IMenu.DefaultMenuLayout)),
                Parameters = pars
            };
        }

        #region old code
        //private async Task<IMenuResult> MapMenuAndInvokeAsync(IMenuRouter menuRouter, MenuContext context, params object[] creationParams)
        //{
        //    async Task<IMenuResult> CreateMenuAndInvoke(Type menuType, MethodInfo action, object[] creationPars, MenuParameter[] invocationPars)
        //    {
        //        var menuInstance = menuRouter.CreateMenuOfType(menuType, creationPars);

        //        return await menuRouter.InvokeMenuActionAsync(action, menuInstance, invocationPars);
        //    }
        //    async Task<IMenuResult> InvokeNextAction(NextAction next)
        //    {
        //        if (next.PlainAction != null)
        //        {
        //            return next.PlainAction.Invoke(context);
        //        }
        //        if (next.Menu == null)
        //        {
        //            throw new ArgumentNullException($"{nameof(next.Menu)} cannot be null unless a {nameof(next.PlainAction)} specified");
        //        }
        //        if (next.Menu.GetInterface(nameof(IMenu)) == null)
        //        {
        //            throw new ArgumentException($"{next.Menu} is not a menu");
        //        }

        //        MethodInfo action = next.MenuAction ?? next.Menu.GetMethod(nameof(IMenu.DefaultMenuLayout));

        //        return await CreateMenuAndInvoke(next.Menu, action, creationParams, next.Parameters);
        //    }

        //    var localPlayer = context.LocalPlayer;

        //    if (!localPlayer.IsAuthorized)
        //    {
        //        if (localPlayer.PreviousResult == null)
        //        {
        //            return await CreateMenuAndInvoke(Constants.AuthMenu, Constants.AuthMenuAuthAction, creationParams, null);
        //        }

        //        var next = localPlayer.PreviousResult.GetNextAction(context);
        //        return await InvokeNextAction(next);
        //    }
        //    else
        //    {
        //        if (localPlayer.PreviousResult == null)
        //        {
        //            return await CreateMenuAndInvoke(Constants.MainMenu, Constants.MainMenuAfterRestartAction, creationParams, null);
        //        }

        //        var next = localPlayer.PreviousResult.GetNextAction(context);
        //        return await InvokeNextAction(next);
        //    }
        //}
        #endregion
    }
}
