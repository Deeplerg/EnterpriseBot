using Newtonsoft.Json;

namespace EnterpriseBot.Api.Models.Other
{
    public class EmptyGameResult
    {
        [JsonProperty("localizedError")]
        public LocalizedError LocalizedError { get; set; }

        public static implicit operator EmptyGameResult(LocalizedError localizedError)
        {
            return new EmptyGameResult
            {
                LocalizedError = localizedError
            };
        }
    }
}
