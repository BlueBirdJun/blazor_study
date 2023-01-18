using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace grpcclient
{
    public class Demo1
    {
        private static Random random;
        private static string url = "http://localhost:5000";
        private static string accesstoken = default;
        public Demo1(string act)
        {
            random = new Random();
            accesstoken = act;
        }

        private static Metadata headers = new Metadata();
        private static void GenerateHeader()
        {
            headers.Add("Authorization", $"Bearer {accesstoken}");
        }
        public async Task Teset1()
        {
            await ServerStreamingDemo();
            await ClientStreamingDemo();
            await BidirectionStreamingDemo();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        private static async Task BidirectionStreamingDemo()
        {
            var channel = GrpcChannel.ForAddress(url);
            var client = new StreamDemo.StreamDemoClient(channel);
            var stream = client.BidirectionalStreamingDemo();

            var requestTask = Task.Run(async () =>
            {
                for (int i = 1; i <= 10; i++)
                {
                    var randomNumber = random.Next(1, 10);
                    await Task.Delay(randomNumber * 1000);
                    await stream.RequestStream.WriteAsync(new Test { TestMessage = i.ToString() });
                    Console.WriteLine("Send Request: " + i);
                }

                await stream.RequestStream.CompleteAsync();
            });

            var responseTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext(CancellationToken.None))
                {
                    Console.WriteLine("Received Response: " + stream.ResponseStream.Current.TestMessage);
                }

                Console.WriteLine("Response Stream Completed");
            });

            await Task.WhenAll(requestTask, responseTask);
            await channel.ShutdownAsync();
        }

        private static async Task ClientStreamingDemo()
        {
            var channel = GrpcChannel.ForAddress(url);
            var client = new StreamDemo.StreamDemoClient(channel);
            var stream = client.ClientStreamingDemo();
            for (int i = 1; i <= 10; i++)
            {
                await stream.RequestStream.WriteAsync(new Test { TestMessage = $"Message {i}" });
                var randomNumber = random.Next(1, 10);
                await Task.Delay(randomNumber * 100);
            }

            await stream.RequestStream.CompleteAsync();
            var response = await stream.ResponseAsync;
            Console.WriteLine("Response: " + response.TestMessage);
            await channel.ShutdownAsync();
            Console.WriteLine("Completed Client Streaming");
        }

        private static async Task ServerStreamingDemo()
        {
            var channel = GrpcChannel.ForAddress(url);
            var client = new StreamDemo.StreamDemoClient(channel);
            var response = client.ServerStreamingDemo(new Test { TestMessage = "Test" });
            while (await response.ResponseStream.MoveNext(CancellationToken.None))
            {
                var value = response.ResponseStream.Current.TestMessage;
                Console.WriteLine(value);
            }

            Console.WriteLine("Server Streaming Completed");
            await channel.ShutdownAsync();
        }
    }
}
