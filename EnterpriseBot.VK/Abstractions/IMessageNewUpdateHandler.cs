using System.Threading.Tasks;
using EnterpriseBot.VK.Models.Other;
using VkNet.Model.GroupUpdate;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IMessageNewUpdateHandler
    {
        Task<bool> Handle(MessageNew messageNew);
    }
}