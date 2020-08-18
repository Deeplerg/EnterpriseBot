using System.Threading.Tasks;
using EnterpriseBot.VK.Models.MenuRelated;
using VkNet.Model;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IMenuContextConfigurator
    {
        Task<MenuContext> Configure(Message message);
    }
}