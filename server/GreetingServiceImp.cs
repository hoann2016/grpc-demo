using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;

namespace server
{
    public class GreetingServiceImp: GreetService.GreetServiceBase
    {
        public override async Task GreetmanyTimes(GreetManyTimesRequest request, IServerStreamWriter<GreetingManyTimesResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine($"The server received the request: {request}");
            Console.WriteLine(request.ToString());
            string result=String.Format("Hello {0} {1}",request.Greeting.FirstName,request.Greeting.LastName);
            for (int i = 0; i < 10; i++)
            {
                GreetingManyTimesResponse response = new GreetingManyTimesResponse()
                {
                    Result = result
                };
                await responseStream.WriteAsync(response);
            }
            
        }
    }
}