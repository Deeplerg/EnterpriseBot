namespace EnterpriseBot.VK.Models.Messages
{
    public class MessageInfo
    {
        public string Text { get; set; }
        public string ButtonPayload { get; set; }
        public long VkId { get; set; }
        public int? PressedButton { get; set; }
    }
}
