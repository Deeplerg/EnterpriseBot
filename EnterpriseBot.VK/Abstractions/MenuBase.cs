using EnterpriseBot.ApiWrapper;
using EnterpriseBot.VK.Attributes;
using EnterpriseBot.VK.Infrastructure;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.MenuResults;
using EnterpriseBot.VK.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using VkNet.Abstractions;

namespace EnterpriseBot.VK.Abstractions
{
    public abstract class MenuBase : IMenu
    {
        [MenuBaseProperty]
        public EntbotApi BotApi { get; set; }

        [MenuBaseProperty]
        public MenuContext MenuContext { get; set; }

        [MenuBaseProperty]
        public IVkApi VkApi { get; set; }

        protected MenuBase() { }

        protected MenuBase(EntbotApi botApi, MenuContext menuContext, IVkApi vkApi)
        {
            this.BotApi = botApi;
            this.MenuContext = menuContext;
            this.VkApi = vkApi;
        }

        public abstract IMenuResult DefaultMenuLayout();

        protected TextResult Text(string text, NextAction next)
        {
            return new TextResult(text, next);
        }

        protected TextResult Text(string text, Type nextMenu, MethodInfo nextMenuAction)
        {
            return new TextResult(text, new NextAction
            {
                Menu = nextMenu,
                MenuAction = nextMenuAction
            });
        }

        protected TextResult Text(string text, Type nextMenu, string nextMenuActionName)
        {
            var method = nextMenu.GetMethod(nextMenuActionName)
                         ?? throw new ArgumentException(
                                      string.Format(ExceptionTemplates.MethodDoesNotExistInTypeTemplate,
                                                    nextMenuActionName, nextMenu));

            return Text(text, nextMenu, method);
        }

        protected TextResult Text(string text, Func<MenuContext, IMenuResult> nextAction)
        {
            return new TextResult(text, new NextAction
            {
                PlainAction = nextAction
            });
        }

        protected KeyboardResult Keyboard(string text, LocalKeyboard keyboard)
        {
            return new KeyboardResult(text, keyboard);
        }

        protected KeyboardResult Keyboard(string text, IEnumerable<IEnumerable<LocalKeyboardButton>> buttons)
        {
            return new KeyboardResult(text, buttons);
        }

        protected KeyboardResult Keyboard(string text, IEnumerable<LocalKeyboardButton> buttons)
        {
            return new KeyboardResult(text, new List<IEnumerable<LocalKeyboardButton>>
            {
                buttons
            });
        }

        protected KeyboardResult Keyboard(string text, LocalKeyboardButton button)
        {
            var builder = new LocalKeyboardBuilder();
            builder.AddButton(button);
            return new KeyboardResult(text, builder.Build());
        }

        protected InputResult Input(string message, Type nextMenu, MethodInfo nextMenuAction, params MenuParameter[] additionalParams)
        {
            return new InputResult(message, nextMenu, nextMenuAction, additionalParams);
        }

        protected InputResult Input(string message, Type nextMenu, string nextMenuActionName, params MenuParameter[] additionalParams)
        {
            var method = nextMenu.GetMethod(nextMenuActionName)
                         ?? throw new ArgumentException(
                                      string.Format(ExceptionTemplates.MethodDoesNotExistInTypeTemplate,
                                                    nextMenuActionName, nextMenu));
            return Input(message, nextMenu, method, additionalParams);
        }
    }
}
