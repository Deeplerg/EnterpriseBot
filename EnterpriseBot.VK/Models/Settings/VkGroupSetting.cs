namespace EnterpriseBot.VK.Models.Settings
{
    public class VkGroupSetting
    {
        public ulong GroupId { get; set; }
        public string AccessToken { get; set; }
        public string SecretKey { get; set; }
        public string Confirmation { get; set; }
    }
}
