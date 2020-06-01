using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Essences;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Essences;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Essences
{
    public class PlayerCategory : EssencesCategoryBase<Player>,
                                  ICreatableCategory<Player, PlayerCreationParams>
    {
        private static readonly string categoryName = "player";

        public PlayerCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Player> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Player>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        ///<inheritdoc/>
        public async Task<Player> Create(PlayerCreationParams pars)
        {
            var result = await api.Call<Player>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Sets a player's name
        /// </summary>
        /// <param name="playerId">Id of player which name to change</param>
        /// <param name="newName">New player name</param>
        /// <returns>New player name</returns>
        public async Task<string> SetName(long playerId, string newName)
        {
            var pars = new
            {
                playerId = playerId,
                newName = newName
            };

            var result = await api.Call<string>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(SetName).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Sets the password by sending it as unencrypted raw string
        /// </summary>
        /// <param name="playerId">Id of player which password to set</param>
        /// <param name="password">New password - unencrypted raw string</param>
        /// <returns>Encrypted password</returns>
        public async Task<string> SetPassword(long playerId, string newPassword)
        {
            var pars = new
            {
                playerId = playerId,
                newPassword = newPassword
            };

            var result = await api.Call<string>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(SetPassword).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Checks password correctness
        /// </summary>
        /// <param name="playerId">Id of the player which password correctness to check</param>
        /// <param name="password">Unencrypted password</param>
        /// <returns>Is password correct</returns>
        public async Task<bool> VerifyPassword(long playerId, string password)
        {
            var pars = new
            {
                playerId = playerId,
                password = password
            };

            var result = await api.Call<bool>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(VerifyPassword).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Links the VK
        /// </summary>
        /// <param name="playerId">Id of player whose VK to link</param>
        /// <param name="vkId">VK id which to link</param>
        /// <returns>Player</returns>
        public async Task<Player> LinkVk(long playerId, long vkId)
        {
            var pars = new
            {
                playerId = playerId,
                vkId = vkId
            };

            var result = await api.Call<Player>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(LinkVk).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Unlinks the VK
        /// </summary>
        /// <param name="playerId">Id of player whose VK to unlink</param>
        /// <returns></returns>
        public async Task<Player> UnlinkVk(long playerId)
        {
            var pars = new
            {
                playerId = playerId
            };

            var result = await api.Call<Player>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(UnlinkVk).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Adds units to player
        /// </summary>
        /// <param name="playerId">Id of player to add units to</param>
        /// <param name="amount">Amount of units to add</param>
        /// <returns>Player's units</returns>
        public async Task<decimal> AddUnits(long playerId, decimal amount)
        {
            var pars = new
            {
                playerId = playerId,
                amount = amount
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(AddUnits).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Sends player's units to another player
        /// </summary>
        /// <param name="senderId">Id of player who sends those units</param>
        /// <param name="receiverId">Id of player who receives these units</param>
        /// <param name="amount">Amount of units to send</param>
        /// <returns>Sender's units after sending</returns>
        public async Task<decimal> SendUnits(long senderId, long receiverId, decimal amount)
        {
            var pars = new
            {
                senderId = senderId,
                receiverId = receiverId,
                amount = amount
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(SendUnits).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Returns a player which id on a platform matches the specified one
        /// </summary>
        /// <param name="platformPlayerId">Id of the player on a platform</param>
        /// <param name="platform">Platform type</param>
        /// <returns>Player which id on a platform matches the specified one</returns>
        public async Task<Player> GetByPlatform(object platformPlayerId, BotPlatform platform)
        {
            var pars = new
            {
                platformPlayerId = platformPlayerId,
                platform = platform
            };

            var result = await api.Call<Player>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(GetByPlatform).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Returns a player which name matches the specified one
        /// </summary>
        /// <param name="name">Player name</param>
        /// <returns>Player which name matches the specified one</returns>
        public async Task<Player> GetByName(string name)
        {
            var pars = new
            {
                name = name
            };

            var result = await api.Call<Player>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(GetByName).ToLower()
            }, pars);

            return result;
        }
    }
}
