using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace EnterpriseBot.VK.Abstractions
{
    public interface IVkMessageGateway
    {
        void Send(long? peerId, string text);
        void Send(long? peerId, string text, MessageKeyboard keyboard);
        void Send(MessagesSendParams pars);
        void Stop();
    }
}
