using EtcordSharp.Packets;
using System;

namespace EtcordSharp.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            PacketSerializer.Initialize();

            Server server = new Server();
            server.Start(3879);
        }
    }
}
