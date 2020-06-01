using EnterpriseBot.VK.Menus;
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
        public static readonly MethodInfo MainMenuAfterRestartAction = MainMenu.GetMethod(nameof(Menus.MainMenu.AfterRestart));

        public static readonly Type AuthMenu = typeof(AuthMenu);
        public static readonly MethodInfo AuthMenuAuthAction = AuthMenu.GetMethod(nameof(Menus.AuthMenu.Auth));

        public static readonly Type PayloadEmptyMenu = typeof(BotDoesNotSupportTextCommandsMenu);
        public static readonly MethodInfo PayloadEmptyMenuAction = PayloadEmptyMenu.GetMethod(nameof(Menus.BotDoesNotSupportTextCommandsMenu.ReturnBack));
        #endregion
    }
}
