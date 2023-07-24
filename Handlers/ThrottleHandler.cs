using TechTask.Configurations;

namespace TechTask.Handlers
{
    public class ThrottleHandler : DelegatingHandler
    {
        private static int _freeRequestCount = 100;

        private static readonly object s = new();

        private readonly HackerNewsConfiguration _newsConfiguration;

        public ThrottleHandler(HttpMessageHandler innerHandler, HackerNewsConfiguration newsConfiguration)
            : base(innerHandler)
        {
            _newsConfiguration = newsConfiguration;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage>? response;

            lock (s)
            {
            resendMessage:
                if (_freeRequestCount-- > 0)
                {
                    response = base.SendAsync(request, cancellationToken);
                }
                else
                {
                    Task.Delay(1000, cancellationToken).Wait(cancellationToken);
                    _freeRequestCount = _newsConfiguration.MaxRequestPerSecond;

                    goto resendMessage;
                }
            }

            return response;
        }
    }
}