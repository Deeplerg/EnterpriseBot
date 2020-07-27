using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.MenuResults;

namespace EnterpriseBot.VK.Menus
{
    public class BotDoesNotSupportTextCommandsMenu : MenuBase
    {
        public IMenuResult ReturnBack()
        {
            string message = "На данный момент бот не поддерживает текстовые команды. Пожалуйста, используйте клавиатуру.";

            return new ReturnBackKeyboardResult(message, MenuContext);
        }

        public override IMenuResult DefaultMenuLayout()
        {
            return ReturnBack();
        }
    }
}
