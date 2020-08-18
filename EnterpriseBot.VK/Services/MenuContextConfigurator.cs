using System.Threading.Tasks;
using EnterpriseBot.ApiWrapper;
using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Exceptions;
using EnterpriseBot.VK.Models.Keyboard;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Models.Messages;
using EnterpriseBot.VK.Models.Player;
using EnterpriseBot.VK.Utils;
using Newtonsoft.Json;
using VkNet.Model;

namespace EnterpriseBot.VK.Services
{
    public class MenuContextConfigurator : IMenuContextConfigurator
    {
        private readonly ILocalPlayerManager localPlayerManager;
        private readonly EntbotApi api;

        public MenuContextConfigurator(ILocalPlayerManager localPlayerManager,
                                       EntbotApi botApi)
        {
            this.localPlayerManager = localPlayerManager;
            this.api = botApi;
        }
        
        public async Task<MenuContext> Configure(Message message)
        {
            MenuContext context = new MenuContext();
            
            long peerId = message.PeerId.Value;
            context.Message = new MessageInfo
            {
                Text = message.Text,
                VkId = peerId
            };
            
            if (!string.IsNullOrEmpty(message.Payload))
            {
                string buttonPayload = JsonConvert.DeserializeObject<VkButtonPayload>(message.Payload).Payload;
                
                switch (buttonPayload)
                {
                    case Constants.VkStartButtonPayloadValue:
                        context.Message.PressedButton = null;
                        break;
                    
                    // In any other case
                    default:
                        if (!KeyboardUtils.TryDecipherPayload(buttonPayload, out int pressedButton))
                            throw new InvalidMessagePayloadException($"{peerId} is trying to change the payload: {message.Payload}");

                        context.Message.PressedButton = pressedButton;
                        break;
                }
            }
            var localPlayer = await AuthByVkId(peerId);
            context.LocalPlayer = localPlayer;

            return context;
        }
        
        private async Task<LocalPlayer> AuthByVkId(long peerId)
        {
            var localPlayer = localPlayerManager.Get(peerId);
            if (localPlayer == null)
            {
                var player = await api.Essences.Player.GetByVK(peerId);
                if (player == null)
                {
                    localPlayer = localPlayerManager.AddNonAuthorized(peerId);
                }
                else
                {
                    localPlayer = localPlayerManager.AddAuthorized(peerId, player.Id);
                }
            }
            return localPlayer;
        }
    }
}