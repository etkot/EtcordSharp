using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using EtcordSharp.Client.WebView.JSON;
using SharpWebview;
using SharpWebview.Content;

namespace EtcordSharp.Client.WebView
{
    public class WebviewClient : IClientEventListener
    {
        private Thread webviewThread;
        private Webview webview;

        private struct WebViewEvent
        {
            public string name;
            public string id;
            public string data;
        }
        private ConcurrentQueue<WebViewEvent> webViewEventQueue;

        private Client client;

        public void Start()
        {
            webViewEventQueue = new ConcurrentQueue<WebViewEvent>();

            webviewThread = new Thread(() => StartWebview());
            webviewThread.SetApartmentState(ApartmentState.STA);
            webviewThread.Start();

            client = new Client(this);

            WebViewEvent webViewEvent;
            while (true)
            {
                client.Receive();

                while (webViewEventQueue.TryDequeue(out webViewEvent))
                {
                    ProcessEvent(webViewEvent.name, webViewEvent.id, webViewEvent.data);
                }

                Thread.Sleep(10);
            }
        }

        private void ProcessEvent(string name, string id, string data)
        {
            string methodName = name.Split("_")[1];
            MethodInfo method = typeof(WebviewClient).GetMethod(methodName);

            string[] strParams = data.Substring(1, data.Length - 2).Split(',');
            object[] parameters = new object[strParams.Length];

            if (strParams.Length != method.GetParameters().Length)
            {
                LogError("Tried to call \"" + name + "\" with invalid amount of parameters (" + strParams.Length + " instead of " + method.GetParameters().Length + ")");
                return;
            }

            for (int i = 0; i < strParams.Length; i++)
            {
                if (strParams[i][0] == '"' && strParams[i][strParams[i].Length - 1] == '"')
                {
                    parameters[i] = strParams[i].Substring(1, strParams[i].Length - 2);
                }
                else
                {
                    int num;
                    if (int.TryParse(strParams[i], out num))
                    {
                        parameters[i] = num;
                        continue;
                    }

                    float f;
                    if (float.TryParse(strParams[i], out f))
                    {
                        parameters[i] = f;
                        continue;
                    }
                }
            }

            bool result = (bool)method.Invoke(this, parameters);
            SendReturn(id, result);
        }

        public bool Connect(string address, string username)
        {
            int port = 3879;
            string[] addressSplit = address.Split(':');
            if (addressSplit.Length > 1)
            {
                if (!int.TryParse(addressSplit[1], out port))
                {
                    LogError("etcord_Connect: Invalid port (not an integer)");
                    return false;
                }
            }

            if (port > 65536)
            {
                LogError("etcord_Connect: Invalid port (over 65536)");
                return false;
            }

            if (username == "")
            {
                LogError("etcord_Connect: No username given");
                return false;
            }

            client.Connect(address, port, username);
            return true;
        }



        private void StartWebview()
        {
            IWebviewContent content;

#if DEBUG
            content = new UrlContent("http://localhost:3000");
#else
            content = new HostedContent();
#endif

            using (Webview wv = new Webview(true, true))
            {
                webview = wv;
                webview
                    .SetTitle("Etcord")
                    .SetSize(1024, 768, WebviewHint.None)
                    .SetSize(800, 600, WebviewHint.Min)
                    .Bind("etcord_Connect", (id, req) =>
                    {
                        webViewEventQueue.Enqueue(new WebViewEvent{ name = "etcord_Connect", id = id, data = req });

                        // Req contains the parameters of the javascript call
                        //Console.WriteLine(req);
                        // And returns a successful promise result to the javascript function, which executed the 'evalTest'
                        //webview.Return(id, RPCResult.Success, "undefined");
                    })
                    .Navigate(content)
                    .Run();

                webview = null;
            }
        }

        #region Events

        private string ToJSON(params JSObject[] parameters)
        {
            string json = "{";

            for (int i = 0; i < parameters.Length; i++)
            {
                json += parameters[i].name + ":" + parameters[i].ToJSON();
                if (i < parameters.Length - 1)
                    json += ",";
            }

            json += "}";
            return json;
        }

        private void SendEvent(string name, params JSObject[] parameters)
        {
            string json = ToJSON(parameters);
            string command = "events.emit(\"" + name + "\", " + json + ")";
            Send(command);
        }



        public void OnClientStateChanged(Client.ClientState state)
        {
            SendEvent("ClientStateChanged", (JSClientState)state);
        }

        public void OnChannelAdded(ClientChannel channel)
        {
            SendEvent("ChannelAdded", (JSChannel)channel);
        }

        public void OnChannelUpdated(ClientChannel channel)
        {
            SendEvent("ChannelAdded", (JSChannel)channel);
        }

        public void OnMessageAdded(ClientMessage message)
        {
            SendEvent("ChannelAdded", (JSMessage)message);
        }

        public void OnUserAdded(ClientUser user)
        {
            SendEvent("ChannelAdded", (JSUser)user);
        }

        public void OnUserJoinVoice(ClientUser user, ClientChannel channel)
        {
            SendEvent("ChannelAdded", (JSUser)user, (JSChannel)channel);
        }

        public void OnUserLeaveVoice(ClientUser user, ClientChannel channel)
        {
            SendEvent("ChannelAdded", (JSUser)user, (JSChannel)channel);
        }

        public void OnUserUpdated(ClientUser user)
        {
            SendEvent("ChannelAdded", (JSUser)user);
        }

        #endregion Events

        #region Webview Helpers

        private void Send(string command)
        {
            webview?.Dispatch(() => webview.Evaluate(command));
        }

        private void SendReturn(string id, bool success, string resultJson = "undefined")
        {
            webview?.Dispatch(() => webview.Return(id, success ? RPCResult.Success : RPCResult.Error, resultJson));
        }

        private void LogError(string text)
        {
            Send("console.error('" + text + "')");
        }

        #endregion Webview Helpers
    }
}
