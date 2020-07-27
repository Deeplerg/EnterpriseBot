using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using System;

namespace EnterpriseBot.VK.Models.Player
{
    public class LocalPlayer : ICloneable
    {
        public string Id { get; set; }

        public long PlayerId { get; set; }
        public long VkId { get; set; }

        public LocalKeyboard CurrentKeyboard { get; set; }

        public bool IsAuthorized { get; set; }

        /// <summary>
        /// Should only be used to map the next action depending on the current context
        /// </summary>
        public IMenuResult PreviousResult { get; set; }
        public NextAction PreviousAction { get; set; }

        public object Clone()
        {
            return new LocalPlayer
            {
                Id = Id,
                PlayerId = PlayerId,
                VkId = VkId,
                CurrentKeyboard = (LocalKeyboard)CurrentKeyboard.Clone(),
                IsAuthorized = IsAuthorized,
                PreviousAction = (NextAction)PreviousAction.Clone()
            };
        }
    }
}
