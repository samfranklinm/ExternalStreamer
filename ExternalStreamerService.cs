//using Microsoft.AspNetCore.SignalR.Client;
//using System;
//using System.Threading.Tasks;

//public class ExternalStreamerService
//{
//    private readonly HubConnection _connection;

//    public ExternalStreamerService()
//    {
//        _connection = new HubConnectionBuilder()
//            .WithUrl("http://localhost:7059/sentenceHub")
//            .WithAutomaticReconnect()
//            .Build();

//        _connection.On<string>("ReceiveSentence", (sentence) =>
//        {
//            Console.WriteLine($"Received sentence: {sentence}");
//        });

//        var connectionState = _connection.State;

//        _connection.StartAsync();
//    }
//}
