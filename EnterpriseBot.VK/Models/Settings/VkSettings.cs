using System.Collections.Generic;

namespace EnterpriseBot.VK.Models.Settings
{
    public class VkSettings
    {
        public List<VkGroupSetting> Groups { get; set; }
        public ulong DefaultGroupId { get; set; }

        public bool ReturnOkEvenIfItIsNot { get; set; } //According to VK API Documentation,
                                                        //"If a server returns several errors in a row, Callback API will temporarily stop sending notifications to it."

        public VkLinksSetting Links { get; set; }
    }
}
