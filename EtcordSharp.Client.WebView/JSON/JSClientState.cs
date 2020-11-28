using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client.WebView.JSON
{
    public class JSClientState : JSObject
    {
        public override string name => "state";

        public Client.ClientState state { get; private set; }


        public static explicit operator JSClientState(Client.ClientState state)
        {
            JSClientState jSClientState = new JSClientState();
            jSClientState.state = state;

            return jSClientState;
        }

        public override string ToJSON()
        {
            return "\"" + state.ToString() +"\"";
        }
    }
}
