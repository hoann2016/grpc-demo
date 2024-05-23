using Blog;
using Grpc.Core;
using MongoDB.Bson;
using MongoDB.Driver;

namespace server
{
    public class BlockServiceImpl : BlogService.BlogServiceBase
    {
        private static MongoClient mongoClient = new MongoClient("mongodb://localhost:27017");
        private static IMongoDatabase database = mongoClient.GetDatabase("mydb");
        private static IMongoCollection<BsonDocument> mongoCollection = database.GetCollection<BsonDocument>("blog");

        public override Task<CreateBlogResponse> CreateBlog(CreateBlogRequest request, ServerCallContext context)
        {
            var blog = request.Blog;
            BsonDocument doc = new BsonDocument("author_id", blog.AuthorId)
                .Add("title", blog.Title)
                .Add("content", blog.Content);
            mongoCollection.InsertOne(doc);
            string id = doc.GetValue("_id").ToString();
            blog.Id = id;
            return Task.FromResult(new CreateBlogResponse
            {
                Blog = blog
            });
        }

        public override async Task<ReadBlogResponse> ReadBlog(ReadBlogRequest request, ServerCallContext context)
        {
            var blogId = request.BlogId;
            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(blogId));
            var result = mongoCollection.Find(filter).FirstOrDefault();
            if (result == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Blog not found"));
            }

            Blog.Blog blog = new Blog.Blog
            {
                AuthorId = result.GetValue("author_id").ToString(),
                Title = result.GetValue("title").ToString(),
                Content = result.GetValue("content").ToString()
            };
            return new ReadBlogResponse
            {
                Blog = blog
            };
        }

        public override async Task<UpdateBlogResponse> UpdateBlog(UpdateBlogRequest request,
            ServerCallContext context)
        {
            var blog = request.Blog;
            var blogId = blog.Id;
            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(blogId));
            var result = mongoCollection.Find(filter).FirstOrDefault();
            if (result == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Blog not found"));
            }

            var update = new UpdateDefinitionBuilder<BsonDocument>()
                .Set("author_id", blog.AuthorId)
                .Set("title", blog.Title)
                .Set("content", blog.Content);
            mongoCollection.UpdateOne(filter, update);
            return new UpdateBlogResponse
            {
                Blog = blog
            };
        }

        public override async Task<DeleteBlogResponse> DeleteBlog(DeleteBlogRequest request, ServerCallContext context)
        {
            var blogId = request.BlogId;
            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(blogId));
            var result = mongoCollection.DeleteOne(filter);
            if (result.DeletedCount == 0)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Blog not found"));
            }

            return new DeleteBlogResponse
            {
                BlogId = blogId
            };
        }

        public override async Task ListBlog(ListBlogRequest request,
            IServerStreamWriter<ListBlogResponse> responseStream, ServerCallContext context)
        {
            try
            {
                var filter = new FilterDefinitionBuilder<BsonDocument>().Empty;
                var result = mongoCollection.Find(filter);
                foreach (var doc in result.ToList())
                {
                    Blog.Blog blog = new Blog.Blog
                    {
                        Id = doc.GetValue("_id").ToString(),
                        AuthorId = doc.GetValue("author_id").AsString,
                        Title = doc.GetValue("title").AsString,
                        Content = doc.GetValue("content").AsString
                    };
                    ListBlogResponse response = new ListBlogResponse
                    {
                        Blog = blog
                    };
                    await responseStream.WriteAsync(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}