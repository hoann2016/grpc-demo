﻿using Greet;
using Grpc.Core;
using server;

const int port =50051;
Server server=null;
try
{
    server = new Server
    {
        Services = { GreetingService.BindService(new GreetingServiceIpl()) },
        Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
    };
    server.Start();
    Console.WriteLine("Server listening on port " + port);
    Console.WriteLine("Press any key to stop the server...");
    Console.Read();
}
catch (Exception e)
{
    Console.WriteLine("An error occurred: " + e.Message);
}
finally
{
    
    if (server != null)
    {
        server.ShutdownAsync().Wait();
    }
}