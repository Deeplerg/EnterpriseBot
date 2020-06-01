using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Essences;
using EnterpriseBot.Api.Models.Common.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Essences;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.Constants;
using static EnterpriseBot.Api.Utils.Miscellaneous;

namespace EnterpriseBot.Api.Areas.Essences
{
    [Area(nameof(Essences))]
    public class PlayerController : Controller, IGameController<Player>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public LocalizationSettings LocalizationSettings => localizationSettings;

        public PlayerController(ApplicationContext context,
                             IOptions<GameplaySettings> gameplayOptions,
                             IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Essences.Player;
        }

        ///<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Player>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Player player = await ctx.Players.FindAsync(id);
            //if (player == null) return PlayerDoesNotExist(id);

            return player;
        }

        /////<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Player>> Create([FromBody] PlayerCreationParams cp)
        {
            UserInputRequirements req = localizationSettings.UserInputRequirements;

            if (!CheckName(cp.Name))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The name has not passed verification. " + string.Format(req.Name.English, NameMaxLength),
                    RussianMessage = "Имя не прошло проверку. " + string.Format(req.Name.Russian, NameMaxLength)
                };
            }

            //can be optimized by removing "ToList", which is not recommended (won't generate request with Trim and string.Equals)
            if ((await ctx.Players.ToListAsync()).Any(player => CompareNames(player.Name, cp.Name)))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "A player with the same name already exists",
                    RussianMessage = "Игрок с таким именем уже существует"
                };
            }

            PersonalStorage createdPersonalStorage = (await ctx.PersonalStorages.AddAsync(
                new PersonalStorage
                {
                    Capacity = gameplaySettings.Storages.Personal.DefaultCapacity
                })).Entity;

            byte[] passwordSalt = Hash.CreateSalt();
            string passwordHash = Hash.CreateHash(cp.Password, passwordSalt);
            Player createdPlayer = (await ctx.Players.AddAsync(
                new Player
                {
                    Name = cp.Name,
                    PasswordHash = passwordHash,
                    PasswordSaltBase64 = Convert.ToBase64String(passwordSalt),
                    PersonalStorage = createdPersonalStorage,
                    Units = gameplaySettings.Player.DefaultUnits
                })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.Players.FindAsync(createdPlayer.Id);
        }

        /// <summary>
        /// Sets player's name
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> SetName([FromBody] string json)
        {
            var parsModel = new
            {
                playerId = default(long),
                newName = default(string)
            };
            var d = JsonConvert.DeserializeAnonymousType(json, parsModel);

            UserInputRequirements req = localizationSettings.UserInputRequirements;

            if (!CheckName(d.newName))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The name has not passed verification. " + string.Format(req.Name.English, NameMaxLength),
                    RussianMessage = "Имя не прошло проверку. " + string.Format(req.Name.Russian, NameMaxLength)
                };
            }

            if (ctx.Players.Where(p => p.Name == d.newName).Any())
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "This name is already occupied",
                    RussianMessage = "Это имя уже занято"
                };
            }

            Player player = await ctx.Players.FindAsync(d.playerId);
            if (player == null) return PlayerDoesNotExist(d.playerId);

            player.Name = d.newName;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        /// <summary>
        /// Sets the password
        /// </summary>
        /// <returns>Encrypted password</returns>
        [HttpPost]
        public async Task<GameResult<string>> SetPassword([FromBody] string json)
        {
            var pars = new
            {
                playerId = default(long),
                newPassword = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            if (!IsPasswordSecure(d.newPassword))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Password must have at least 1 digit and be at least 8 characters long",
                    RussianMessage = "Пароль должен содержать как минимум 8 символов и иметь как минимум 1 цифру"
                };
            }

            Player player = await ctx.Players.FindAsync(d.playerId);
            if (player == null) return PlayerDoesNotExist(d.playerId);

            byte[] newPasswordSalt = Hash.CreateSalt();
            string newPasswordHash = Hash.CreateHash(d.newPassword, newPasswordSalt);
            player.PasswordHash = newPasswordHash;
            player.PasswordSaltBase64 = Convert.ToBase64String(newPasswordSalt);

            await ctx.SaveChangesAsync();

            return newPasswordHash;
        }

        /// <summary>
        /// Checks the password correctness
        /// </summary>
        /// <returns>Is password correct</returns>
        [HttpPost]
        public async Task<GameResult<bool>> VerifyPassword([FromBody] string json)
        {
            var pars = new
            {
                playerId = default(long),
                password = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Player player = await ctx.Players.FindAsync(d.playerId);
            if (player == null) return PlayerDoesNotExist(d.playerId);

            byte[] salt = Convert.FromBase64String(player.PasswordSaltBase64);
            string passwordHash = player.PasswordHash;

            if (Hash.Verify(d.password, salt, passwordHash))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Links the VK
        /// </summary>
        /// <returns>Player</returns>
        [HttpPost]
        public async Task<GameResult<Player>> LinkVk([FromBody] string json)
        {
            var pars = new
            {
                playerId = default(long),
                vkId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            if (await ctx.Players.AnyAsync(player => player.VkId == d.vkId))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "There is already an account connected to this VK user",
                    RussianMessage = "Уже есть аккаунт, привязанный к этому пользователю ВКонтакте"
                };
            }


            Player player = await ctx.Players.FindAsync(d.playerId);
            if (player == null) return PlayerDoesNotExist(d.playerId);

            player.VkId = d.vkId;
            player.VkConnected = true;

            await ctx.SaveChangesAsync();

            return await ctx.Players.FindAsync(d.playerId);
        }

        /// <summary>
        /// Unlinks the VK
        /// </summary>
        /// <returns>Player</returns>
        [HttpPost]
        public async Task<GameResult<Player>> UnlinkVk([FromBody] string json)
        {
            var pars = new
            {
                playerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Player player = await ctx.Players.FindAsync(d.playerId);
            if (player == null) return PlayerDoesNotExist(d.playerId);

            player.VkConnected = false;
            player.VkId = default;

            await ctx.SaveChangesAsync();

            return await ctx.Players.FindAsync(d.playerId);
        }

        /// <summary>
        /// Adds units to player
        /// </summary>
        /// <returns>Player's units</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> AddUnits([FromBody] string json)
        {
            var pars = new
            {
                playerId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Player player = await ctx.Players.FindAsync(d.playerId);
            if (player == null) return PlayerDoesNotExist(d.playerId);

            if (player.Units + d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = "Units amount can't be below 0",
                    RussianMessage = "Количество юнитов не может быть ниже 0"
                };
            }

            player.Units += d.amount;

            await ctx.SaveChangesAsync();
            decimal units = (await ctx.Players.FindAsync(d.playerId)).Units;

            return units;
        }

        /// <summary>
        /// Sends player's units to another player
        /// </summary>
        /// <returns>Sender's units after sending</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> SendUnits([FromBody] string json)
        {
            var pars = new
            {
                senderId = default(long),
                receiverId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Player sender = await ctx.Players.FindAsync(d.senderId);
            if (sender == null) return PlayerDoesNotExist(d.senderId);

            Player receiver = await ctx.Players.FindAsync(d.receiverId);
            if (receiver == null) return PlayerDoesNotExist(d.receiverId);

            if (d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can not send negative amount of units",
                    RussianMessage = "Нельзя отправить отрицательное количество юнитов"
                };
            }

            if (sender.Units - d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Not enough units to send. Current units: {sender.Units}; Need at least {d.amount}",
                    RussianMessage = $"Недостаточно юнитов для отправки. Сейчас юнитов: {sender.Units}; Необходимо как минимум {d.amount}"
                };
            }

            sender.Units -= d.amount;
            receiver.Units += d.amount;

            await ctx.SaveChangesAsync();

            return (await ctx.Players.FindAsync(d.senderId)).Units;
        }

        /// <summary>
        /// Returns a player which id on a platform matches the specified one
        /// </summary>
        /// <returns>Player which id on a platform matches the specified one</returns>
        [HttpPost]
        public async Task<GameResult<Player>> GetByPlatform([FromBody] string json)
        {
            var pars = new
            {
                platformPlayerId = default(object),
                platform = default(BotPlatform)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            switch (d.platform)
            {
                case BotPlatform.Vk:
                    return await ctx.Players.SingleOrDefaultAsync(player => player.VkId == (long)d.platformPlayerId);

                default:
                    throw new InvalidEnumArgumentException($"Unknown {nameof(BotPlatform)} value");
            }
        }

        /// <summary>
        /// Returns a player which name matches the specified one
        /// </summary>
        /// <returns>Player which name matches the specified one</returns>
        [HttpPost]
        public async Task<GameResult<Player>> GetByName([FromBody] string json)
        {
            var pars = new
            {
                name = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Player player = await ctx.Players.FirstOrDefaultAsync(p => p.Name.ToLower().Trim() == d.name.ToLower().Trim());
            //if (player == null) return PlayerDoesNotExist(d.name);

            return player;
        }

        #region private
        /// <summary>
        /// Checks whether or not password is secure
        /// </summary>
        /// <param name="password">Password to check security on</param>
        /// <returns>Is password secure</returns>
        [NonAction]
        private bool IsPasswordSecure(string password)
        {
            return password.Length >= 8
                && password.Any(c => char.IsDigit(c))
                && !string.IsNullOrWhiteSpace(password);
        }

        [NonAction]
        private LocalizedError PlayerDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
        #endregion
    }
}
