using EnterpriseBot.VK.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace EnterpriseBot.VK.Services
{
    /// <summary>
    /// Small fire-and-forget message delivery service
    /// </summary>
    public class SeparateThreadVkMessageGateway : IVkMessageGateway, IDisposable
    {
        // To detect redundant calls
        private bool _disposed = false;

        private const int sleepTimeInMilliseconds = 1;

        private readonly ILogger<SeparateThreadVkMessageGateway> logger;
        private readonly Thread thread;
        private readonly IVkApi api;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;

        private readonly ConcurrentQueue<MessagesSendParams> toSend = new ConcurrentQueue<MessagesSendParams>();

        public SeparateThreadVkMessageGateway(ILogger<SeparateThreadVkMessageGateway> logger, IVkApi api)
        {
            this.logger = logger;
            this.api = api;


            cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = cancellationTokenSource.Token;

            thread = new Thread(() => Start(cancellationToken));
            thread.Start();
        }

        public void Send(long? peerId, string text)
        {
            toSend.Enqueue(new MessagesSendParams
            {
                Message = text,
                PeerId = peerId,
                RandomId = GetRandomId()
            });
        }

        public void Send(long? peerId, string text, MessageKeyboard keyboard)
        {
            toSend.Enqueue(new MessagesSendParams
            {
                Message = text,
                Keyboard = keyboard,
                PeerId = peerId,
                RandomId = GetRandomId()
            });
        }

        public void Send(MessagesSendParams pars)
        {
            toSend.Enqueue(pars);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        private void Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                while (!toSend.IsEmpty)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    MessagesSendParams message;
                    if (toSend.TryDequeue(out message))
                    {
                        try
                        {
                            api.Messages.Send(message);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Unable to send message");
                        }
                    }
                }

                Task.Delay(sleepTimeInMilliseconds).GetAwaiter().GetResult();
            }
        }

        private int? GetRandomId()
        {
            return 0; //0 means 'not to use random id'
        }


        ~SeparateThreadVkMessageGateway() => Dispose(false);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                cancellationTokenSource.Cancel();
            }

            //if (thread.IsAlive) //if the thread is still running
            //    thread.Abort();

            _disposed = true;
        }
    }
}
