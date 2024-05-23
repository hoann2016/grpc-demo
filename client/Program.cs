

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
var client=new GreetService.GreetServiceClient(channel);
var stream=client.GreetEveryOne();
var responseReader=Task.Run(async ()=>
{
    while (await stream.ResponseStream.MoveNext())
    {
        Console.WriteLine("Received: " + stream.ResponseStream.Current.Result);
    }
});
List<GreetEveryOneRequest> requests = new List<GreetEveryOneRequest>()
{
    new GreetEveryOneRequest()
    {
        Greeting = new Greeting()
        {
            FirstName = "Mark",
            LastName = "Zuckerberg"
        }
    },
    new GreetEveryOneRequest()
    {
        Greeting = new Greeting()
        {
            FirstName = "Elon",
            LastName = "Musk"
        }
    },
    new GreetEveryOneRequest()
    {
        Greeting = new Greeting()
        {
            FirstName = "Jeff",
            LastName = "Bezos"
        }
    }
};
foreach (var request in requests)
{
    Console.WriteLine("Sending: " + request.Greeting.FirstName + " " + request.Greeting.LastName);
    await stream.RequestStream.WriteAsync(request);
    await Task.Delay(1000);
}
channel.ShutdownAsync().Wait();
Console.Read();