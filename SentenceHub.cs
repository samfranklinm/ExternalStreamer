using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ExternalStreamer.Hubs
{
    public class SentenceHub : Hub
    {
        private List<string> _data;

        public SentenceHub()
        {
            _data = new List<string> { "First sentence", "This is my second sentence.", "This is my third sentence.", "This is my fourth sentence.", "This is my last sentence." };
        }
        public async IAsyncEnumerable<string> GetSentence(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting to send sentences...");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var item in _data)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return item;
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }
    }
}
