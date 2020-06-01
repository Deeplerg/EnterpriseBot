using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EnterpriseBot.VK.Models.MenuResults
{
    public class InputResult : IMenuResult
    {
        private readonly string message;
        private readonly Type nextMenu;
        private readonly MethodInfo nextMenuAction;
        private readonly MenuParameter[] additionalPars;

        public InputResult(string message, Type nextMenuType, MethodInfo nextMenuAction, params MenuParameter[] additionalParams)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(string.Format(ExceptionTemplates.StringNullOrEmptyTemplate,
                                                          nameof(message)));

            if (nextMenuType == null)
                throw new ArgumentNullException(paramName: nameof(nextMenuType));

            if (nextMenuAction == null)
                throw new ArgumentNullException(paramName: nameof(nextMenuAction));


            this.message = message;
            this.nextMenu = nextMenuType;
            this.nextMenuAction = nextMenuAction;
            this.additionalPars = additionalParams;
        }

        public VkMessage GetMessage()
        {
            return new VkMessage
            {
                Text = message
            };
        }

        public NextAction GetNextAction(MenuContext context)
        {
            var pars = new List<MenuParameter>();
            pars.Add(new MenuParameter
            {
                Value = context.Message.Text
            });
            pars.AddRange(additionalPars);

            return new NextAction
            {
                Menu = nextMenu,
                MenuAction = nextMenuAction,
                Parameters = pars.ToArray()
            };
        }

        public object Clone()
        {
            return new InputResult(message, nextMenu, nextMenuAction, additionalPars);
        }
    }
}
