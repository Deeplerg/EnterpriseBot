using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Essences;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;

namespace EnterpriseBot.VK.Menus
{
    public class AuthMenu : MenuBase
    {
        private readonly ILogger<AuthMenu> logger;
        private readonly VkLinksSetting links;

        private readonly Type thisType;

        public AuthMenu(ILogger<AuthMenu> logger, IOptions<VkSettings> vkOptions)
        {
            this.logger = logger;
            this.links = vkOptions.Value.Links;

            this.thisType = this.GetType();
        }

        public async Task<IMenuResult> Auth()
        {
            long vkId = MenuContext.LocalPlayer.VkId;
            logger.LogInformation($"Authenticating {vkId}");

            var user = (await VkApi.Users.GetAsync(new long[] { vkId })).FirstOrDefault();

            string greeting;
            if (user == null)
            {
                logger.LogWarning($"Could not find VK user {vkId}");
                greeting = $"Привет!";
            }
            else
            {
                greeting = $"Привет, {user.FirstName}!";
            }

            string message = greeting + "\n" + "Для начала необходимо авторизоваться. У тебя уже есть аккаунт?";

            return Keyboard(message, new List<LocalKeyboardButton>
            {
                new LocalKeyboardButton
                {
                    Text = "Зарегистрироваться",
                    Next = new NextAction
                    {
                        PlainAction = (ctx) => Input("Введи ник:", thisType, nameof(Register_PlayerName))
                    },
                    Color = KeyboardButtonColor.Primary
                },
                new LocalKeyboardButton
                {
                    Text = "Войти",
                    Next = new NextAction
                    {
                        PlainAction = (ctx) => Input("Введи ник или id своего аккаунта:", thisType, nameof(Login_PlayerName))
                    },
                    Color = KeyboardButtonColor.Default
                }
            });
        }

        public async Task<IMenuResult> Login_PlayerName(string input)
        {
            var player = await BotApi.Essences.Player.GetByName(input);
            if (player == null)
            {
                if (long.TryParse(input, out long id))
                    player = await BotApi.Essences.Player.Get(id);
            }

            if (player == null)
            {
                return Keyboard($"😕 Такой игрок не найден", new LocalKeyboardButton
                {
                    Text = "Назад",
                    Next = new NextAction
                    {
                        Menu = thisType,
                        MenuAction = thisType.GetMethod(nameof(Auth))
                    }
                });
            }

            string message = "Теперь необходимо ввести пароль." +
                             "\n" +
                             "Напоминаем, что все пароли хранятся только в зашифрованном виде." +
                             "После отправки пароля и входа в аккаунт настоятельно рекомендуется удалить сообщение с паролем." +
                             "\n" +
                             "Пароль:";

            var playerIdParameter = new MenuParameter
            {
                Name = "playerId",
                Value = player.Id
            };

            return Input(message, thisType, nameof(Login_Password), playerIdParameter);
        }

        public async Task<IMenuResult> Login_Password(string password, long playerId)
        {
            var player = await BotApi.Essences.Player.Get(playerId);

            bool isPasswordValid = await BotApi.Essences.Player.VerifyPassword(playerId, password);

            if (isPasswordValid)
            {
                MenuContext.LocalPlayer.PlayerId = playerId;
                MenuContext.LocalPlayer.IsAuthorized = true;
                await BotApi.Essences.Player.LinkVk(player.Id, MenuContext.LocalPlayer.VkId);

                logger.LogInformation($"Player [{playerId}] {player.Name} has just logged in");

                return Keyboard("😄 Авторизация прошла успешно!", new LocalKeyboardButton
                {
                    Text = "В главное меню",
                    Next = new NextAction
                    {
                        Menu = Constants.MainMenu
                    }
                });
            }
            else
            {
                return Keyboard("😕 Пароль неверный.", new LocalKeyboardButton
                {
                    Text = "Назад",
                    Next = new NextAction
                    {
                        Menu = thisType,
                        MenuAction = thisType.GetMethod(nameof(Auth))
                    }
                });
            }
        }

        public IMenuResult Register_PlayerName(string name)
        {
            string message = "Теперь нужно ввести пароль." +
                             "\n" +
                             "Все пароли хранятся только в зашифрованном виде." +
                             "После отправки пароля настоятельно рекомендуется удалить сообщение с ним." +
                             "\n" +
                             "Если что-то идёт не так, напиши в " +
                            $"{MessageUtils.HideVkNameIntoText(links.EntbotSupportVkName, "техподдержку.")}" +
                             "\n" +
                             "Пароль:";

            var nameParameter = new MenuParameter(name);

            return Input(message, thisType, nameof(Register_Password), nameParameter);
        }

        public async Task<IMenuResult> Register_Password(string password, string name)
        {
            var player = await BotApi.Essences.Player.Create(new PlayerCreationParams
            {
                Name = name,
                Password = password
            });

            MenuContext.LocalPlayer.PlayerId = player.Id;
            MenuContext.LocalPlayer.IsAuthorized = true;
            await BotApi.Essences.Player.LinkVk(player.Id, MenuContext.LocalPlayer.VkId);

            logger.LogInformation($"Player [{player.Id}] {player.Name} has just registered");

            string message = "😄 Регистрация прошла успешно! " +
                            $"Добро пожаловать в EnterpriseBot, {player.Name}!";

            return Keyboard(message, new LocalKeyboardButton
            {
                Text = "В главное меню",
                Next = new NextAction
                {
                    Menu = Constants.MainMenu
                },
                Color = KeyboardButtonColor.Positive
            });
        }

        public override IMenuResult DefaultMenuLayout()
        {
            return Auth().GetAwaiter().GetResult();
        }
    }
}
