using EnterpriseBot.VK.Models.Other;
using System.Threading.Tasks;
using VkNet.Model.GroupUpdate;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IVkUpdateHandler
    {
        Task<HandleUpdateResult> HandleUpdateAsync(GroupUpdate update);
    }
}
