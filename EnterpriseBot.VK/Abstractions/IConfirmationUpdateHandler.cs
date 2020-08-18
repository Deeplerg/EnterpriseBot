using System.Threading.Tasks;
using EnterpriseBot.VK.Models.Other;
using VkNet.Model.GroupUpdate;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IConfirmationUpdateHandler
    {
        Task<string> Handle(GroupUpdate update);
    }
}