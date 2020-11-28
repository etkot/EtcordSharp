using System;

namespace EtcordSharp.Client.WebView
{
    class Program
    {
        private static void Main(string[] args)
        {
            WebviewClient client = new WebviewClient();
            client.Start();
        }
    }
}
