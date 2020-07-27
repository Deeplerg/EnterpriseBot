using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Player;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EnterpriseBot.VK.Services
{
    public class DefaultLocalPlayerManager : ILocalPlayerManager
    {

        private readonly List<LocalPlayer> Players = new List<LocalPlayer>(); //performance suggestion:
                                                                              //make two separate dictionaries - by id and by vkid
                                                                              //Dictionary[key] is faster than iteration through a list
                                                                              //however, this solution might require more memory.

        private readonly ILogger<DefaultLocalPlayerManager> logger;

        public DefaultLocalPlayerManager(ILogger<DefaultLocalPlayerManager> logger)
        {
            this.logger = logger;
        }

#nullable enable
        public LocalPlayer? Get(object id, PlayerManagerFilter filter = PlayerManagerFilter.LocalId)
#nullable restore
        {
            switch (filter)
            {
                case PlayerManagerFilter.LocalId:
                    return Players.FirstOrDefault(player => player.Id == (string)id);

                case PlayerManagerFilter.PlayerId:
                    return Players.FirstOrDefault(player => player.PlayerId == (long)id);

                case PlayerManagerFilter.VkId:
                    return Players.FirstOrDefault(player => player.VkId == (long)id);

                default:
                    throw new InvalidEnumArgumentException($"Unknown {nameof(PlayerManagerFilter)} {nameof(filter)} value");
            }
        }

        public LocalPlayer AddAuthorized(long vkId, long playerId)
        {
            LocalPlayer newLocalPlayer = new LocalPlayer
            {
                Id = GenerateLocalId(),
                PlayerId = playerId,
                VkId = vkId,
                IsAuthorized = true
            };

            Players.Add(newLocalPlayer);

            return newLocalPlayer;
        }

        public LocalPlayer AddNonAuthorized(long vkId)
        {
            LocalPlayer newLocalPlayer = new LocalPlayer
            {
                Id = GenerateLocalId(),
                VkId = vkId,
                IsAuthorized = false
            };

            Players.Add(newLocalPlayer);

            return newLocalPlayer;
        }

        private string GenerateLocalId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
