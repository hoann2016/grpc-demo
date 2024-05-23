

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
var client=new GreetService.GreetServiceClient (channel);

GreetManyTimesRequest greetManyTimesRequest = new GreetManyTimesRequest()
{
    Greeting = new Greeting()
    {
        FirstName = "John",
        LastName = "Doe"
    }
};
var response =client.GreetmanyTimes(greetManyTimesRequest);
while (await response.ResponseStream.MoveNext())
{
    Console.WriteLine(response.ResponseStream.Current.Result);
    await Task.Delay(200);
}
channel.ShutdownAsync().Wait();
Console.Read();