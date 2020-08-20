using EnterpriseBot.ApiWrapper.Models.CreationParams.Essences;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Enums;
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
using VkNet.Model;

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
                                                                                      Color = LocalKeyboardButtonColor.Positive,
                                                                                      Next = new NextAction(menu: Constants.MainMenu, 
                                                                                                            action: Constants.MainMenuMainAction),
                                                                                      Text = "В главное меню"
                                                                                  });

        public AuthMenu(ILogger<AuthMenu> logger, IOptions<VkSettings> vkOptions)
        {
            this.logger = logger;
            this.links = vkOptions.Value.Links;

            this.thisType = this.GetType();
        }

        public async Task<IMenuResult> Auth()
        {
            if (MenuContext.LocalPlayer.IsAuthorized) return alreadyAuthorizedResult;
            
            long vkId = MenuContext.LocalPlayer.VkId;
            logger.LogInformation($"Authenticating {vkId}");

            var user = (await VkApi.Users.GetAsync(new long[] { vkId })).FirstOrDefault();


            string greeting;
            bool canRegisterWithVkName;

            if (user == null)
            {
                logger.LogWarning($"Could not find VK user {vkId}");
                greeting = $"Привет!";

                canRegisterWithVkName = false;
            }
            else
            {
                greeting = $"Привет, {user.FirstName}!";

                string vkName = GetNameFromVKUser(user);
                canRegisterWithVkName = !await BotApi.Essences.Player.CheckIsPlayerNameAlreadyTaken(vkName);
            }

            string message = greeting +
                             "\n" +
                             "Для начала необходимо авторизоваться.";


            var buttons = new List<LocalKeyboardButton>();

            LocalKeyboardButton registerButton = new LocalKeyboardButton
            {
                Text = "Зарегистрироваться",
                Color = LocalKeyboardButtonColor.Primary
            };

            if (canRegisterWithVkName)
            {
                registerButton.Next = new NextAction(thisType, nameof(Register_SelectRegisterOption));
            }
            else
            {
                registerButton.Next = new NextAction(thisType, nameof(Register_EnterName));
            }

            buttons.Add(registerButton);
            buttons.Add(new LocalKeyboardButton
            {
                Text = "Войти",
                Next = new NextAction(thisType, nameof(Login_EnterNameOrId)),
                Color = LocalKeyboardButtonColor.Default
            });

            return Keyboard(message, buttons);
        }

        #region login

        public IMenuResult Login_EnterNameOrId()
        {
            string message = "Введи ник или id своего аккаунта:";

            return new InputResult(message,
                                   nextAction: new NextAction(thisType, nameof(Login_EnterPassword)),
                                   inputStringParameterName: "input",
                                   returnBackAction: new NextAction(thisType, nameof(Auth)));
        }


        public async Task<IMenuResult> Login_EnterPassword(string input)
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
                    Next = new NextAction(thisType, nameof(Auth))
                });
            }

            if(!player.HasPassword)
            {
                string messageNoPassword = "😕 У этого игрока не установлен пароль.\n" +
                                           "\n" +
                                          $"Если это Ваш аккаунт, " +
                                          $"{MessageUtils.HideVkNameIntoText(links.EntbotSupportVkName, "обратитесь в поддержку.")}";

                return Keyboard(messageNoPassword, new LocalKeyboardButton
                {
                    Text = "Назад",
                    Next = new NextAction(thisType, nameof(Auth))
                });
            }


            string message = "Теперь необходимо ввести пароль." +
                             "\n" +
                             "Все пароли хранятся в зашифрованном виде." +
                             "После отправки пароля рекомендуем удалить сообщение с ним." +
                             "\n" +
                             "Пароль:";

            var playerIdParameter = new MenuParameter
            {
                Name = "playerId",
                Value = player.Id
            };

            return new InputResult(message,
                                   nextAction: new NextAction(thisType, nameof(Login_Finish), playerIdParameter),
                                   inputStringParameterName: "password",
                                   returnBackAction: new NextAction(thisType, nameof(Auth)));
        }


        public async Task<IMenuResult> Login_Finish(string password, long playerId)
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
                    Next = new NextAction(Constants.MainMenu, Constants.MainMenuMainAction),
                    Color = LocalKeyboardButtonColor.Positive
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

        #endregion

        #region register

        public IMenuResult Register_SelectRegisterOption()
        {       
            string message = "Вы можете зарегистрироваться, выбрав любой ник, а можете войти, используя Ваше имя и фамилию ВКонтакте и изменить ник позже, если захотите.";

            return Keyboard(message, new List<LocalKeyboardButton>
            {
                new LocalKeyboardButton
                {
                    Text = "Ввести ник",
                    Next = new NextAction(thisType, nameof(Register_EnterName)),
                    Color = LocalKeyboardButtonColor.Primary
                },
                new LocalKeyboardButton
                {
                    Text = "Использовать мои имя и фамилию",
                    Next = new NextAction(thisType, nameof(Register_WithVKName))
                }
            });
        }

        public IMenuResult Register_EnterName()
        {
            string message = "Введи ник:";

            return new InputResult(message,
                                   nextAction: new NextAction(thisType, nameof(Register_EnterPassword)),
                                   inputStringParameterName: "name",
                                   returnBackAction: new NextAction(thisType, nameof(Auth)));
        }

        public IMenuResult Register_EnterPassword(string name)
        {
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
                                   nextAction: new NextAction(thisType, nameof(Register_Finish), 
                                                              new MenuParameter(name, name: "name")),
                                   inputStringParameterName: "password",
                                   returnBackAction: new NextAction(thisType, nameof(Auth)));
        }

        public async Task<IMenuResult> Register_Finish(string password, string name)
        {
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
                Color = LocalKeyboardButtonColor.Positive
            });
        }

        public async Task<IMenuResult> Register_WithVKName()
        {
            long vkId = MenuContext.LocalPlayer.VkId;
            var user = (await VkApi.Users.GetAsync(new long[] { vkId })).First();

            string name = GetNameFromVKUser(user);

            var player = await BotApi.Essences.Player.CreateWithNoPassword(name);

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
                Color = LocalKeyboardButtonColor.Positive
            });
        }

        #endregion

        public override IMenuResult DefaultMenuLayout()
        {
            return Auth().GetAwaiter().GetResult();
        }


        private string GetNameFromVKUser(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Nickname))
                return user.Nickname;
            else
                return user.FirstName + " " + user.LastName;
        }
    }
}
