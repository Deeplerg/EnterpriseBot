using System;

namespace EnterpriseBot.VK.Models.Messages
{
    public class MessageInfo : ICloneable
    {
        public string Text { get; set; }
        public string ButtonPayload { get; set; }
        public long VkId { get; set; }
        public int? PressedButton { get; set; }

        public object Clone()
        {
            return new MessageInfo
            {
                Text = Text,
                ButtonPayload = ButtonPayload,
                VkId = VkId,
                PressedButton = PressedButton
            };
        }
    }
}
