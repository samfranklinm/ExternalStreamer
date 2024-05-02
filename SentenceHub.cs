using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace ExternalStreamer.Hubs
{
    public class SentenceHub : Hub
    {
        private List<string> _data;

        public SentenceHub()
        {
            _data = new List<string> { "First sentence", "This is my second sentence.", "This is my third sentence.", "This is my fourth sentence.", "This is my last sentence." };
        }
        public async IAsyncEnumerable<string> GetSentence([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting to send sentences...");
            while(true)
            {
                foreach (var item in _data)
                {
                    yield return item;
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }
    }
}
