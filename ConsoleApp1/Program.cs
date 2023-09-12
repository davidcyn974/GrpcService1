// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using GrpcService1;
using GrpcService1.Services;
using System;



/*var channel = GrpcChannel.ForAddress("https://localhost:7081");
var client = new Greeter.GreeterClient(channel);

var response = await client.SayHelloAsync(new DonneMoiRequest { Name = "Val", Age = 77 , Sex = "Unknown" });

Console.WriteLine("Response from server  : " + response.Message);*/


class Program
{
    
    public static async Task Main(string[] args)
    {
        //await ServerStreamingDemo();
        //await ClientStreamingDemo();
        //await BidirectionnalStreamingDemo();
        /*var channel = GrpcChannel.ForAddress("http://localhost:5219");
        var client = new Greeter.GreeterClient(channel);

        var response = await client.SayHelloAsync(new DonneMoiRequest
        {
            Name = "Bob",
        });*/

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static async Task ServerStreamingDemo()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5219");
        var client = new Greeter.GreeterClient(channel);
        var response = client.ServerStreaming(new Test { TestMessage = "Hello" });
        while( await response.ResponseStream.MoveNext(CancellationToken.None))
        {
            Console.WriteLine(response.ResponseStream.Current.TestMessage);
        }


        Console.WriteLine("Fin du stream");
        await channel.ShutdownAsync();
    }

    static async Task ClientStreamingDemo()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5219");
        var client = new Greeter.GreeterClient(channel);
        var response = client.ClientStreaming();
        for (int i = 0; i < 10; i++)
        {
            await response.RequestStream.WriteAsync(new Test { TestMessage = "Message du client numéro : " + i });
        }
        await response.RequestStream.CompleteAsync(); // signal the server stream's end
        Console.WriteLine(await response.ResponseAsync);
        await channel.ShutdownAsync();
    }

    static async Task BidirectionnalStreamingDemo()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5219");
        var client = new Greeter.GreeterClient(channel);

        var stream = client.BidirectionalStreaming();

        var requestTask = Task.Run(async () =>
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(new Random().Next(1,10) * 1000 );
                await stream.RequestStream.WriteAsync(new Test { TestMessage = i.ToString() });
                Console.WriteLine("Nb request sent = " + i);
            }
            await stream.RequestStream.CompleteAsync();
        });

        var responseTask = Task.Run(async () =>
        {
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                Console.WriteLine("Response from server is : " + stream.ResponseStream.Current.TestMessage);
            }
        });


    }



}