using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Player;

namespace EnterpriseBot.VK.Abstractions
{
    public interface ILocalPlayerManager
    {
        LocalPlayer Get(object id, PlayerManagerFilter filter);
        LocalPlayer AddAuthorized(long vkId, long playerId);
        LocalPlayer AddNonAuthorized(long vkId);
    }
}
