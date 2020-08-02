using EnterpriseBot.VK.Models.Enums;
using EnterpriseBot.VK.Models.Player;

namespace EnterpriseBot.VK.Abstractions
{
    public interface ILocalPlayerManager
    {
#nullable enable
        LocalPlayer? Get(long vkId);
#nullable restore
        LocalPlayer AddAuthorized(long vkId, long playerId);
        LocalPlayer AddNonAuthorized(long vkId);
        void Update(LocalPlayer localPlayer);
    }
}
