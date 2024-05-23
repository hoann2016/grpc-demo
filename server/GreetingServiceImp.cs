using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;

namespace server
{
    public class GreetingServiceImp:GreetService.GreetServiceBase
    {

        public override async Task GreetEveryOne(IAsyncStreamReader<GreetEveryOneRequest> requestStream, IServerStreamWriter<GreetEveryOneResponse> responseStream, ServerCallContext context)
        {
            string result = "Hello ";
            while (await requestStream.MoveNext())
            {
                result += String.Format("hello {0} {1} ", requestStream.Current.Greeting.FirstName, requestStream.Current.Greeting.LastName);
                Console.WriteLine(" reveived request from client"+ result);
                await responseStream.WriteAsync(new GreetEveryOneResponse() { Result = result });
            }

        }
    }
}