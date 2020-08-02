using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Player;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.VK.Services
{
    public class InMemoryLocalPlayerManager : ILocalPlayerManager
    {
        private readonly Dictionary<long, LocalPlayer> players = new Dictionary<long, LocalPlayer>();

        private readonly ILogger<InMemoryLocalPlayerManager> logger;

        public InMemoryLocalPlayerManager(ILogger<InMemoryLocalPlayerManager> logger)
        {
            this.logger = logger;
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public LocalPlayer? Get(long vkId)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            players.TryGetValue(vkId, out LocalPlayer player);

            return player;
        }

        public LocalPlayer AddAuthorized(long vkId, long playerId)
        {
            LocalPlayer player = new LocalPlayer
            {
                PlayerId = playerId,
                VkId = vkId,
                IsAuthorized = true
            };

            players[vkId] = player;

            return player;
        }

        public LocalPlayer AddNonAuthorized(long vkId)
        {
            LocalPlayer player = new LocalPlayer
            {
                VkId = vkId,
                IsAuthorized = false
            };

            players[vkId] = player;

            return player;
        }

        public void Update(LocalPlayer localPlayer)
        {
            if(players.TryGetValue(localPlayer.VkId, out _) == false)
            {
                players[localPlayer.VkId] = localPlayer;
            }
        }
    }
}
