using Newtonsoft.Json;

namespace EnterpriseBot.VK.Models.Keyboard
{
    public class VkButtonPayload
    {
        [JsonProperty("button")]
        public string Payload { get; set; }

        [JsonProperty("text")]
        private string PayloadText
        {
            set
            {
                Payload = value;
            }
        }
    }
}
