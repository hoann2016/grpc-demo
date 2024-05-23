

using Dumy;
using Greet;
using Grpc.Core;

const string target="127.0.0.1:50051";
Channel channel = new Channel(target, ChannelCredentials.Insecure);
channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
    {
        Console.WriteLine("The client connected successfully");
    }
});
// var client=new DummyService.DummyServiceClient(channel);
var client=new GreetingService.GreetingServiceClient(channel);
var response=client.Greet(new GreetingRequest
{
    Greeting = new Greeting
    {
        FirstName = "John",
        LastName = "Doe"
    }
});
Console.WriteLine(response.Result);
channel.ShutdownAsync().Wait();
Console.Read();