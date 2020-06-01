using Newtonsoft.Json;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Essences
{
    [JsonObject(Title = "creationParams", Id = "creationParams")]
    public class PlayerCreationParams
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
