using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Utils;
using Newtonsoft.Json;
using System;

namespace EnterpriseBot.VK.Models.MenuResults
{
    [JsonObject(MemberSerialization.Fields)]
    public class TextResult : IMenuResult
    {
        private readonly string text;
        private readonly NextAction nextAction;

        public bool IsSuccessfulResult { get; } = true; 

        public TextResult(string text, NextAction nextAction)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(string.Format(ExceptionTemplates.StringNullOrEmptyTemplate,
                                                          nameof(text)));
            }

            this.text = text;
            this.nextAction = nextAction ?? throw new ArgumentNullException(paramName: nameof(nextAction));
        }

        public VkMessage GetMessage()
        {
            return new VkMessage(text);
        }

        public NextAction GetNextAction(MenuContext context)
        {
            return nextAction;
        }

        public object Clone()
        {
            return new TextResult(text, (NextAction)nextAction.Clone());
        }
    }
}
