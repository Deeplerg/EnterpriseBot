using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Infrastructure;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using VkNet.Model.Keyboard;

namespace EnterpriseBot.VK.Models.MenuResults
{
    [JsonObject(MemberSerialization.Fields)]
    public class InputResult : IMenuResult
    {
        private readonly string message;
        private readonly NextAction nextAction;
        private string inputStringParameterName;
        private NextAction returnBackAction;

        public bool IsSuccessfulResult { get; } = true;
        
        public InputResult(string message, NextAction nextAction, string inputStringParameterName = null, NextAction returnBackAction = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(string.Format(ExceptionTemplates.StringNullOrEmptyTemplate,
                                                          nameof(message)));


            this.message = message;
            this.nextAction = nextAction;
            this.inputStringParameterName = inputStringParameterName;
            this.returnBackAction = returnBackAction;
        }

        public VkMessage GetMessage()
        {
            var vkMessage = new VkMessage
            {
                Text = message
            };

            if(returnBackAction != null)
            {
                var builder = new LocalKeyboardBuilder();
                builder.AddButton(new LocalKeyboardButton
                {
                    Text = "Вернуться назад",
                    Next = returnBackAction,
                });
                vkMessage.Keyboard = builder.Build();
            }

            return vkMessage;
        }

        public NextAction GetNextAction(MenuContext context)
        {
            var pars = new List<MenuParameter>();
            pars.Add(new MenuParameter
            {
                Value = context.Message.Text,
                Name = inputStringParameterName
            });
            if(nextAction.Parameters != null) 
                pars.AddRange(nextAction.Parameters);

            var next = (NextAction)nextAction.Clone();
            next.Parameters = pars.ToArray();

            return next;
        }

        public object Clone()
        {
            return new InputResult(message, (NextAction)nextAction.Clone(), inputStringParameterName, returnBackAction);
        }
    }
}
