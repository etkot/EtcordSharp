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
            using (var webview = new Webview())
            {
                webview
                    .SetTitle("The Hitchhicker")
                    .SetSize(1024, 768, WebviewHint.None)
                    .SetSize(800, 600, WebviewHint.Min)
                    .Navigate(new UrlContent("https://en.wikipedia.org/wiki/The_Hitchhiker%27s_Guide_to_the_Galaxy_(novel)"))
                    .Run();
            }
        }
    }
}
