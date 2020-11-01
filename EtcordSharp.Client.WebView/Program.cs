using System;
using SharpWebview;
using SharpWebview.Content;

namespace EtcordSharp.Client.WebView
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IWebviewContent content = new HostedContent();

#if DEBUG
            content = new UrlContent("http://localhost:3000");
#endif

            using (var webview = new Webview(true, true))
            {
                webview
                    .SetTitle("Etcord")
                    .SetSize(1024, 768, WebviewHint.None)
                    .SetSize(800, 600, WebviewHint.Min)
                    .Navigate(content)
                    .Run();
            }
        }
    }
}
