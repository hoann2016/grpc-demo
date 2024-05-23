

using Dumy;
using Greet;
using Grpc.Core;

const string target="127.0.0.1:50051";
Channel channel = new Channel(target, ChannelCredentials.Insecure);
await channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
    {
        Console.WriteLine("The client connected successfully");
    }
});
var client=new GreetingService.GreetingServiceClient(channel);
var stream =client.LongGreet();
var request = new LongGreetRequest
{
    Greeting = new Greeting
    {
        FirstName = "John",
        LastName = "Doe"
    }
};

foreach(int i in Enumerable.Range(1,10))
{
    await stream.RequestStream.WriteAsync(request);
}
await stream.RequestStream.CompleteAsync();

var response = await stream.ResponseAsync;
Console.WriteLine(response.Result);
channel.ShutdownAsync().Wait();
Console.Read();