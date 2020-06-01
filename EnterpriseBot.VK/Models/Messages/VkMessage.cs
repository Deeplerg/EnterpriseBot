using EnterpriseBot.VK.Models.Keyboard;

namespace EnterpriseBot.VK.Models.Messages
{
    public class VkMessage
    {
        public string Text { get; set; }
        public LocalKeyboard Keyboard { get; set; }

        public VkMessage() { }

        public VkMessage(string text)
        {
            Text = text;
        }

        public VkMessage(string text, LocalKeyboard keyboard)
        {
            Text = text;
            Keyboard = keyboard;
        }
    }
}
