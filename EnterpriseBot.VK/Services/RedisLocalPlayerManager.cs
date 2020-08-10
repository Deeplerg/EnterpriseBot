using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace EnterpriseBot.VK.Services
{
    public class RedisLocalPlayerManager : ILocalPlayerManager
    {
        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: true,
                                                                         overrideSpecifiedNames: false)
            },
            TypeNameHandling = TypeNameHandling.All
        };

        private readonly IDatabase db;

        public RedisLocalPlayerManager(IConnectionMultiplexer connectionMultiplexer)
        {
            db = connectionMultiplexer.GetDatabase();
        }

        public LocalPlayer AddAuthorized(long vkId, long playerId)
        {
            LocalPlayer player = new LocalPlayer
            {
                VkId = vkId,
                PlayerId = playerId,
                IsAuthorized = true
            };

            db.StringSet(vkId.ToString(), Serialize(player));

            return player;
        }

        public LocalPlayer AddNonAuthorized(long vkId)
        {
            LocalPlayer player = new LocalPlayer
            {
                VkId = vkId,
                IsAuthorized = false
            };

            db.StringSet(vkId.ToString(), Serialize(player));

            return player;
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public LocalPlayer? Get(long vkId)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            string json = db.StringGet(vkId.ToString());

            if (string.IsNullOrEmpty(json))
                return null;

            return Deserialize(json);
        }

        public void Update(LocalPlayer localPlayer)
        {
            db.StringSet(localPlayer.VkId.ToString(), Serialize(localPlayer));
        }


        private string Serialize(LocalPlayer localPlayer)
        {
            return JsonConvert.SerializeObject(localPlayer, jsonSettings);
        }

        private LocalPlayer Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<LocalPlayer>(json, jsonSettings);
        }
    }
}
