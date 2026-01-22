using System;
using System.Threading.Tasks;
using VCore.Server.Core;

namespace VCore.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "V-CORE Messenger Server";
            
            int port = 8080;
            SocketServer server = new SocketServer(port);
            
            Console.WriteLine("========================================");
            Console.WriteLine("    V-CORE MESSENGER SERVER SYSTEM      ");
            Console.WriteLine("========================================");
            
            await server.StartAsync();
        }
    }
}
