using Newtonsoft.Json;
using System.Net;

namespace EnterpriseBot.ApiWrapper.Models.Other
{
    public class Result
    {
        [JsonProperty("result")]
        public object JsonResult { get; set; }

        [JsonProperty("localizedError")]
        public LocalizedError LocalizedError { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        [JsonIgnore]
        public bool IsSuccess
        {
            get => (((int)StatusCode >= 200) && ((int)StatusCode <= 299)) && LocalizedError == null;
        }

        [JsonIgnore]
        public string RawJson { get; set; }
    }
}
