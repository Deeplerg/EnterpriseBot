using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Models.MenuResults;

namespace EnterpriseBot.VK.Menus
{
    public class BotDoesNotSupportTextCommandsMenu : MenuBase
    {
        public IMenuResult ReturnBack(IMenuResult previousResult = null)
        {
            if (previousResult == null)
                previousResult = MenuContext.LocalPlayer.PreviousResult;

            string message = "На данный момент бот не поддерживает текстовые команды. Пожалуйста, используйте клавиатуру.";

            return new ReturnBackKeyboardResult(message, previousResult);
        }

        public override IMenuResult DefaultMenuLayout()
        {
            return ReturnBack();
        }
    }
}
