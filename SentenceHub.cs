using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ExternalStreamer.Hubs
{
    public class SentenceHub : Hub
    {
        private List<string> _data;
        private CancellationTokenSource _cts = new();

        public SentenceHub()
        {
            _data = new List<string>();
            for (int i = 1; i <= 5; i++)
            {
                _data.Add($"This is sentence number {i}");
            }
        }

        public async IAsyncEnumerable<string> GetSentences(bool cancellation, [EnumeratorCancellation] CancellationToken cancellationToken)
        {

            Console.WriteLine("Starting to send sentences...");
            Console.WriteLine("Cancellation Requested: " + cancellation);
            while (true)
            {
                foreach (var item in _data)
                {
                    if (cancellation)
                    {
                        Console.WriteLine("Cancellation detected, stopping the stream...");
                        yield break;  // Stop streaming if cancellation is requested
                    }

                    yield return item;
                    await Task.Delay(1000);
                }
            }
        }
    }
}