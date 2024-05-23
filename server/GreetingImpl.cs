using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;

namespace server
{
    public class GreetingServiceImpl:Greet.GreetingService.GreetingServiceBase
    {
        public override async Task<GreetingResponse> LongGreet(IAsyncStreamReader<LongGreetRequest> request, ServerCallContext context)
        {
            string result = "Hello ";
            while (await request.MoveNext())
            {
                result += String.Format("Hello {0}  {1}", request.Current.Greeting.FirstName,request.Current.Greeting.LastName);
            }
          return new GreetingResponse { Result = result };
        }
    }
}