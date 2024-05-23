using FileTransfer;
using Google.Protobuf;
using Grpc.Core;

const string target = "127.0.0.1:50051";
Channel channel = new Channel(target, ChannelCredentials.Insecure);
await channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
    {
        Console.WriteLine("The client connected successfully");
    }
});

var client = new FileTransferService.FileTransferServiceClient(channel);
var stream = client.SendFile();

//receive the response
Task.Run(async () =>
{
    while (await stream.ResponseStream.MoveNext())
    {
        var res = stream.ResponseStream.Current;
        Console.WriteLine("Received: " + res);
    }
});


//Upload the file
var bytes = File.ReadAllBytes("./upload/image.gif");
var byteString = ByteString.CopyFrom(bytes);
var fileName = "image.gif";
var chunkSize = 64 * 1024;
var bytesLeft = byteString.Length;
var offset = 0;
while (bytesLeft > 0)
{
    var thisChunkSize = Math.Min(chunkSize, bytesLeft);
    var data = byteString.Take(thisChunkSize).Skip(offset).ToArray();

    var chunk = new FileTransferRequest
    {
        FileName = fileName,
        Data = ByteString.CopyFrom(data)
    };
    await stream.RequestStream.WriteAsync(chunk);
    offset += thisChunkSize;
    bytesLeft -= thisChunkSize;
}
await stream.RequestStream.CompleteAsync();


channel.ShutdownAsync().Wait();
Console.Read();