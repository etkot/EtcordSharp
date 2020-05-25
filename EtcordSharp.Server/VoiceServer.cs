using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EtcordSharp.Server
{
    public class VoiceServer
    {
        private Server server;
        private UdpClient udpServer;
        private Thread receiveThread;

        public VoiceServer(Server server)
        {
            this.server = server;
            udpServer = new UdpClient();
        }

        public void Start(int port)
        {


            receiveThread = new Thread(() => Receive());
            receiveThread.Start();
        }

        private void Receive()
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                byte[] receiveBytes = udpServer.Receive(ref RemoteIpEndPoint);


            }
        }
    }
}
