using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;

namespace EnterpriseBot.VK.Menus
{
    public class MainMenu : MenuBase
    {
        private readonly VkLinksSetting links;

        public MainMenu(IOptions<VkSettings> vkOptions)
        {
            this.links = vkOptions.Value.Links;
        }

        public async Task<IMenuResult> AfterRestart()
        {
            var player = await BotApi.Essences.Player.Get(MenuContext.LocalPlayer.PlayerId);

            string message = $"Привет, {player.Name}! Похоже, произошла перезагрузка бота. " +
                              "Информация о меню недоступна. 🙁\n" +
                              "\n" +
                              "Если из-за этого произошло что-то ужасное, обратись в" +
                             $"{MessageUtils.HideVkNameIntoText(links.EntbotSupportVkName, " техподдержку, ")}" +
                              "поможем 😉";

            return Keyboard(message, new LocalKeyboardButton
            {
                Text = "В главное меню",
                Color = KeyboardButtonColor.Default,
                Next = new NextAction
                {
                    Menu = Constants.MainMenu
                }
            });
        }

        public override IMenuResult DefaultMenuLayout()
        {
            return Text("Главное меню бота. В разработке...", this.GetType(), nameof(DefaultMenuLayout));
        }
    }
}
