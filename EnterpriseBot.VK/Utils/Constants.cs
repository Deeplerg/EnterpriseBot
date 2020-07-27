using EnterpriseBot.VK.Menus;
using EnterpriseBot.VK.Menus.Service;
using System;
using System.Reflection;

namespace EnterpriseBot.VK.Utils
{
    public class Constants
    {
        public const int GroupMaxRequestsPerSecond = 20;

        public static readonly int GuidLength = Guid.NewGuid().ToString().Length;

        #region menu consts
        public static readonly Type MainMenu = typeof(MainMenu);
        public static readonly MethodInfo MainMenuAfterRestartAction = TypeUtils.GetMethod(MainMenu, nameof(Menus.MainMenu.AfterRestart));
        public static readonly MethodInfo MainMenuMainAction = TypeUtils.GetMethod(MainMenu, nameof(Menus.MainMenu.DefaultMenuLayout));

        public static readonly Type AuthMenu = typeof(AuthMenu);
        public static readonly MethodInfo AuthMenuAuthAction = TypeUtils.GetMethod(AuthMenu, nameof(Menus.AuthMenu.Auth));

        public static readonly Type PayloadEmptyMenu = typeof(BotDoesNotSupportTextCommandsMenu);
        public static readonly MethodInfo PayloadEmptyMenuAction = TypeUtils.GetMethod(PayloadEmptyMenu, nameof(Menus.BotDoesNotSupportTextCommandsMenu.ReturnBack));

        public static readonly Type ReturnBackMenu = typeof(ReturnBackMenu);
        public static readonly MethodInfo ReturnBackMenuReturnToResultAction = TypeUtils.GetMethod(ReturnBackMenu, nameof(Menus.Service.ReturnBackMenu.ReturnBackToResult));
        public static readonly MethodInfo ReturnBackMenuReturnResultAction = TypeUtils.GetMethod(ReturnBackMenu, nameof(Menus.Service.ReturnBackMenu.ReturnResult));
        #endregion

        #region menu specific
        public const string ReturnBackMenuDefaultButtonText = "Вернуться назад";
        #endregion
    }
}
