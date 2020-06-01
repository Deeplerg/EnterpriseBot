using EnterpriseBot.VK.Models.MenuRelated;
using System.Threading.Tasks;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IMenuMapper
    {
        NextAction MapAction(MenuContext context);
        Task<IMenuResult> InvokeAction(NextAction next, MenuContext context, IMenuRouter menuRouter, params object[] menuCreationParams);
    }
}
