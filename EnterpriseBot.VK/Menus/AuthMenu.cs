using EnterpriseBot.ApiWrapper.Models.CreationParams.Essences;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.MenuResults;
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
        
        private readonly IMenuResult alreadyAuthorizedResult = new KeyboardResult("😄 Авторизация прошла успешно!", 
                                                                                  new LocalKeyboardButton
                                                                                  {
                                                                                      Color = KeyboardButtonColor.Positive,
                                                                                      Next = new NextAction(menu: Constants.MainMenu, 
                                                                                                            action: Constants.MainMenuMainAction),
                                                                                      Text = "В главное меню"
                                                                                  });
        
        private bool IsAuthorized => MenuContext.LocalPlayer.IsAuthorized;

        public AuthMenu(ILogger<AuthMenu> logger, IOptions<VkSettings> vkOptions)
        {
            this.logger = logger;
            this.links = vkOptions.Value.Links;

            this.thisType = this.GetType();
        }

        public async Task<IMenuResult> Auth()
        {
            if (IsAuthorized) return alreadyAuthorizedResult;
            
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

            string message = greeting + "\n" + "Для начала необходимо авторизоваться.";

            return Keyboard(message, new List<LocalKeyboardButton>
            {
                new LocalKeyboardButton
                {
                    Text = "Зарегистрироваться",
                    Next = new NextAction(thisType, nameof(Register_EnterName)),
                    Color = KeyboardButtonColor.Primary
                },
                new LocalKeyboardButton
                {
                    Text = "Войти",
                    Next = new NextAction(thisType, nameof(Login_EnterNameOrId)),
                    Color = KeyboardButtonColor.Default
                }
            });
        }

        public IMenuResult Login_EnterNameOrId()
        {
            if (IsAuthorized) return alreadyAuthorizedResult;

            string message = "Введи ник или id своего аккаунта:";

            return new InputResult(message,
                                   nextAction: new NextAction(thisType, nameof(Login_PlayerName)),
                                   inputStringParameterName: "input",
                                   returnBackAction: new NextAction(thisType, nameof(Auth)));
        }

        public async Task<IMenuResult> Login_PlayerName(string input)
        {
            if (IsAuthorized) return alreadyAuthorizedResult;

            var player = await BotApi.Essences.Player.SearchByExactName(input);
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
                    Next = new NextAction(thisType, nameof(Auth))
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

            return new InputResult(message,
                                   nextAction: new NextAction(thisType, nameof(Login_Password), playerIdParameter),
                                   inputStringParameterName: "password",
                                   returnBackAction: new NextAction(thisType, nameof(Auth)));
        }

        public async Task<IMenuResult> Login_Password(string password, long playerId)
        {
            if (IsAuthorized) return alreadyAuthorizedResult;

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
                    Next = new NextAction(Constants.MainMenu, Constants.MainMenuMainAction),
                    Color = KeyboardButtonColor.Positive
                });
            }
            else
            {
                return Keyboard("😕 Пароль неверный.", new LocalKeyboardButton
                {
                    Text = "Назад",
                    Next = new NextAction(thisType, nameof(Auth))
                });
            }
        }


        public IMenuResult Register_EnterName()
        {
            if (IsAuthorized) return alreadyAuthorizedResult;

            string message = "Введи ник:";

            return new InputResult(message,
                                   nextAction: new NextAction(thisType, nameof(Register_PlayerName)),
                                   inputStringParameterName: "name",
                                   returnBackAction: new NextAction(thisType, nameof(Auth)));
        }

        public IMenuResult Register_PlayerName(string name)
        {
            if (IsAuthorized) return alreadyAuthorizedResult;

            string message = "Теперь нужно ввести пароль." +
                             "\n" +
                             "Все пароли хранятся только в зашифрованном виде." +
                             "После отправки пароля настоятельно рекомендуется удалить сообщение с ним." +
                             "\n" +
                             "Если что-то идёт не так, " +
                            $"{MessageUtils.HideVkNameIntoText(links.EntbotSupportVkName, "напиши в поддержку.")}" +
                             "\n" +
                             "Пароль:";

            var nameParameter = new MenuParameter(name);

            return new InputResult(message,
                                   nextAction: new NextAction(thisType, nameof(Register_Password), 
                                                              new MenuParameter(name, name: "name")),
                                   inputStringParameterName: "password",
                                   returnBackAction: new NextAction(thisType, nameof(Auth)));
        }

        public async Task<IMenuResult> Register_Password(string password, string name)
        {
            if (IsAuthorized) return alreadyAuthorizedResult;

            var player = await BotApi.Essences.Player.Create(new PlayerCreationParams
            {
                Name = name,
                RawPassword = password
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
                Next = new NextAction(Constants.MainMenu, Constants.MainMenuMainAction),
                Color = KeyboardButtonColor.Positive
            });
        }

        public override IMenuResult DefaultMenuLayout()
        {
            return Auth().GetAwaiter().GetResult();
        }
    }
}
