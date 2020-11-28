

namespace EtcordSharp.Client
{
    public interface IClientEventListener
    {
        void OnClientStateChanged(Client.ClientState state);
        void OnChannelAdded(ClientChannel channel);
        void OnChannelUpdated(ClientChannel channel);
        void OnMessageAdded(ClientMessage message);
        void OnUserAdded(ClientUser user);
        void OnUserUpdated(ClientUser user);
        void OnUserJoinVoice(ClientUser user, ClientChannel channel);
        void OnUserLeaveVoice(ClientUser user, ClientChannel channel);
    }
}