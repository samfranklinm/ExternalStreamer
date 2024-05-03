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
        private readonly ConcurrentQueue<string> _messageQueue;
        private CancellationTokenSource _cts = new();
        private readonly Dictionary<int, Task> _runningTasks = new Dictionary<int, Task>();
        private int _taskIdCounter = 0;

        public SentenceHub()
        {
            _data = new List<string>();
            _messageQueue = new ConcurrentQueue<string>();
            for (int i = 1; i <= 5; i++)
            {
                _data.Add($"This is sentence number {i}");
            }
        }

        public Task Start(CancellationToken token)
        {
            var taskId = _taskIdCounter++;
            Console.WriteLine("Attempting to start the task.");
            var task = Task.Run(async () =>
            {
                Console.WriteLine("Task started.");
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        foreach (var sentence in _data)
                        {
                            if (token.IsCancellationRequested)
                            {
                                Console.WriteLine("Operation cancelled explicitly inside the loop.");
                                break;
                            }
                            _messageQueue.Enqueue(sentence);
                            Console.WriteLine($"Enqueued: {sentence}");
                            await Task.Delay(2000, token);
                        }
                        Console.WriteLine("Exiting task foreach loop normally.");
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation was cancelled due to token cancellation.");
                    _messageQueue.Clear();
                }
                Console.WriteLine("Task completing...");
            }, token);
            _runningTasks.Add(taskId, task);
            task.ContinueWith(t =>
            {
                Console.WriteLine($"Task {taskId} completed with status {t.Status}");
                _runningTasks.Remove(taskId);
            }, TaskContinuationOptions.ExecuteSynchronously);

            return task;
        }


        public async IAsyncEnumerable<string> GetSentences(bool cancellation, [EnumeratorCancellation] CancellationToken cancellationToken)
        {

            try
            {
                await HandleSetup(cancellation, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Setup was cancelled due to cancellation.");
                yield break; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during setup: {ex.Message}");
                yield break; 
            }

            // Now start yielding results if setup was successful
            while (!_messageQueue.IsEmpty)
            {
                if (_messageQueue.TryDequeue(out var sentence))
                {
                    yield return sentence;
                }
                else
                {
                    await Task.Delay(500, cancellationToken);
                }
            }

            Console.WriteLine("ExternalStreamer has ended the stream due to cancellation!");
        }


        private async Task HandleSetup(bool cancellation, CancellationToken cancellationToken)
        {
            Console.WriteLine("Cancellation requested? " + cancellation);
            if (cancellation)
            {
                _cts.Cancel();
                _cts.Token.ThrowIfCancellationRequested(); 
                Console.WriteLine("All tasks cancelled.");
                _runningTasks.Clear();
            }
            else
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Context.ConnectionAborted);
                if (_cts.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation token is already cancelled before starting the task.");
                    throw new OperationCanceledException(_cts.Token);
                }

                Console.WriteLine("About to start the task.");
                await Start(_cts.Token);
                Console.WriteLine("Task started.");
            }
        }
    }
}
