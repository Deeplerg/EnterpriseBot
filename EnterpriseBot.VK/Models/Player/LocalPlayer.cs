using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Keyboard;

namespace EnterpriseBot.VK.Models.Player
{
    public class LocalPlayer
    {
        public string Id { get; set; }

        public long PlayerId { get; set; }
        public long VkId { get; set; }

        public LocalKeyboard CurrentKeyboard { get; set; }

        public bool IsAuthorized { get; set; }

        public IMenuResult PreviousResult { get; set; }
    }
}
