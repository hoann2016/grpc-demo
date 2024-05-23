using Blog;
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
var client = new BlogService.BlogServiceClient(channel);
//create blog
var response = client.CreateBlog(new CreateBlogRequest
{
    Blog = new Blog.Blog
    {
        AuthorId = "clement",
        Title = "My First Blog",
        Content = "Content of the first blog"
    }
});
Console.WriteLine("Blog created: " + response.Blog.ToString());
//list blogs
var response2 = client.ListBlog(new ListBlogRequest() { });
while (await response2.ResponseStream.MoveNext())
{
    Console.WriteLine(response2.ResponseStream.Current.Blog.ToString());
}

//delete blog
//try
//{
//    var response2 = client.DeleteBlog(new DeleteBlogRequest
//    {
//        BlogId = response.Blog.Id
//    });
//    Console.WriteLine("Blog deleted: " + response2.BlogId);
//}
//catch (Exception e)
//{
//    Console.WriteLine(e);
//    throw;
//}

// update blog
//try
//{
//    response.Blog.AuthorId = "ricky";
//    response.Blog.Title = "update title";
//    response.Blog.Content = "update content";
//    var response2 = client.UpdateBlog(new UpdateBlogRequest
//    {
//        Blog = response.Blog
//    });
//    Console.WriteLine(response2.Blog.ToString());
//}
//catch (Exception e)
//{
//    Console.WriteLine(e);
//    throw;
//}


// get blog
//try
//{
//    var response = client.ReadBlog(new ReadBlogRequest
//    {
//        BlogId = "664f2e04982682697480b4f7"
//    });
//    Console.WriteLine("Blog read: " + response.Blog.ToString());
//}
//catch (RpcException e)
//{
//    Console.WriteLine(e.Status.Detail);
//}

channel.ShutdownAsync().Wait();
Console.Read();