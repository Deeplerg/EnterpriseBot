using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.VK.Menus
{
    public class MainMenu : MenuBase
    {
        private readonly VkLinksSetting links;
        private readonly Type thisType;

        public MainMenu(IOptions<VkSettings> vkOptions)
        {
            this.links = vkOptions.Value.Links;
            this.thisType = typeof(MainMenu);
        }

        public async Task<IMenuResult> AfterRestart()
        {
            var player = await BotApi.Essences.Player.Get(MenuContext.LocalPlayer.PlayerId);

            string message = $"Привет, {player.Name}! Похоже, произошла перезагрузка бота. " +
                              "Информация о меню недоступна. 🙁\n" +
                              "\n" +
                              "Если что, " +
                             $"{MessageUtils.HideVkNameIntoText(links.EntbotSupportVkName, "можешь написать в поддержку.")}";

            return Keyboard(message, new LocalKeyboardButton
            {
                Text = "В главное меню",
                Color = LocalKeyboardButtonColor.Default,
                Next = new NextAction(thisType)
            });
        }

        private IMenuResult ThrowException()
        {
            throw new Exception($"Exception from {thisType}");
        }


        public override IMenuResult DefaultMenuLayout()
        {
            var player = BotApi.Essences.Player.Get(MenuContext.LocalPlayer.PlayerId).GetAwaiter().GetResult();

            var vkUsers = VkApi.Users.GetAsync(new long[] { MenuContext.LocalPlayer.VkId }).GetAwaiter().GetResult();
            var vkUser = vkUsers.FirstOrDefault();

            var units = player.Purse.Money.Single(m => m.Currency == Currency.Units);
            var bc = player.Purse.Money.Single(m => m.Currency == Currency.BusinessCoins);

            string message = "Главное меню бота. В разработке...\n\n" +
                             "Информация:\n" +
                            $"Id игрока: {MenuContext.LocalPlayer.PlayerId}\n" +
                            $"Имя игрока: {player.Name}\n" +
                            $"Дата регистрации: {player.RegistrationDate}\n" +
                            $"Привязан ли ВКонтакте: {(player.VkConnected ? "да" : "нет")}\n" +
                            $"{(player.VkConnected ? $"Id аккаунта ВКонтакте: {player.VkId}\n" : "")}" +
                            $"Баланс:\n" +
                            $"{units.Amount}u; {bc.Amount}bc\n" +
                            $"Есть ли работа: {(player.HasJob ? "да" : "нет")}\n" +
                            $"Есть ли донат: {(player.HasDonation ? "да" : "нет")}\n" +
                            $"Репутация: {player.Reputation.Rating}\n" +
                            $"Количество отзывов: {player.Reputation.Reviews.Count}\n" +
                            $"Владеет ли хоть одной компанией: {(player.OwnedCompanies.Any() ? "да" : "нет")}\n\n" +

                            $"{string.Join("", Enumerable.Repeat('-', 10))}\n\n" +

                            $"Авторизован ли: {(MenuContext.LocalPlayer.IsAuthorized ? "да" : "нет")}\n" +
                            $"VKId из кэша: {MenuContext.LocalPlayer.VkId}\n" +
                            $"Случайное число, проверка уникальности меню: {new Random().Next(int.MinValue, int.MaxValue)}\n";
            if (vkUser != null)
                message += $"Имя ВКонтакте: {vkUser.FirstName} {vkUser.LastName}\n";
            else
                message += "Пользователь ВКонтакте не найден\n";

            return Keyboard(message, new List<LocalKeyboardButton[]>
            {
                new LocalKeyboardButton[]
                {
                    new LocalKeyboardButton
                    {
                        Text = "Вызвать исключение",
                        Next = new NextAction(thisType, nameof(ThrowException)),
                        Color = LocalKeyboardButtonColor.Negative
                    },
                    new LocalKeyboardButton
                    {
                        Text = "Сюда же",
                        Next = new NextAction(thisType)
                    }
                },
                new LocalKeyboardButton[]
                {
                    new LocalKeyboardButton
                    {
                        Text = "Давай по новой, Миша (ТОЛЬКО ЛОГИН)",
                        Next = new NextAction(Constants.AuthMenu, Constants.AuthMenuAuthAction)
                    }
                }
            });
        }
    }
}
