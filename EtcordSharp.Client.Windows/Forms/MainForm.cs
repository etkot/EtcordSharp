using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EtcordSharp.Client.Windows
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private Client client;
        private Timer receiveTimer;

        private ClientChannel selectedChannel = null;
        private Dictionary<ClientChannel, TreeNode> channelNodes;
        private Dictionary<ClientUser, TreeNode> userNodes;


        public MainForm()
        {
            InitializeComponent();

            Client.CreateAudioPlayer = () => new WindowsAudioPlayer();
            client = new Client();

            receiveTimer = new Timer();
            receiveTimer.Interval = 15;
            receiveTimer.Tick += (s, e) => client.Receive();
            receiveTimer.Start();

            channelNodes = new Dictionary<ClientChannel, TreeNode>();
            userNodes = new Dictionary<ClientUser, TreeNode>();

            InitializeClientEvents();
        }

        public void ConnectToServer(string address, string username)
        {
            if (username == "")
            {
                MessageBox.Show(this, "No username given", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int port = 3879;

            // Separate port if one was given
            string[] addressSplit = address.Split(':');
            if (addressSplit.Length > 1)
            {
                address = addressSplit[0];
                if (!int.TryParse(addressSplit[1], out port))
                {
                    MessageBox.Show(this, "Invalid port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            client.Connect(address, port, username);
        }

        void SelectChannel(ClientChannel channel)
        {
            if (channel == selectedChannel)
                return;
            
            selectedChannel = channel;

            channel.GetChatHistory();
            chatBox.ResetText();
            foreach (KeyValuePair<int, ClientMessage> message in channel.Messages)
            {
                chatBox.AppendText("\r\n<" + message.Value.SenderName + "> " + message.Value.Content);
            }
        }


        #region Client events

        private void InitializeClientEvents()
        {
            client.OnClientStateChanged = OnClientStateChanged;

            client.OnChannelAdded = OnChannelAdded;
            client.OnChannelUpdated = OnChannelUpdated;

            client.OnMessageAdded = OnMessageAdded;

            client.OnUserAdded = OnUserAdded;
            client.OnUserUpdated = OnUserUpdated;

            client.OnUserJoinVoice = OnUserJoinVoice;
            client.OnUserLeaveVoice = OnUserLeaveVoice;
        }

        private void OnClientStateChanged(Client.ClientState newState)
        {
            if (newState == Client.ClientState.Connected)
            {
                buttonConnect.Text = "Disconnect";
                chatInputBox.ReadOnly = false;
            }
            else if (newState == Client.ClientState.Unconnected)
            {
                buttonConnect.Text = "Connect";
                chatInputBox.ReadOnly = true;
            }
        }

        private void OnChannelAdded(ClientChannel channel)
        {
            TreeNode node;
            string key = "c" + channel.ChannelID.ToString();
            if (channel.Parent == null)
            {
                node = treeViewChannels.Nodes.Add(key, channel.Name);
            }
            else
            {
                TreeNode parent;
                if (channelNodes.TryGetValue(channel.Parent, out parent))
                {
                    node = parent.Nodes.Add(key, channel.Name);
                }
                else
                {
                    Console.WriteLine("Warning: Channel parent node not found");
                    return;
                }
            }

            treeViewChannels.ExpandAll();
            channelNodes.Add(channel, node);
        }
        private void OnChannelUpdated(ClientChannel channel)
        {
            treeViewChannels.Update();
        }

        private void OnMessageAdded(ClientMessage message)
        {
            if (selectedChannel == message.Channel)
            {
                chatBox.AppendText("\r\n<" + message.SenderName + "> " + message.Content);
            }
        }


        private void OnUserAdded(ClientUser user)
        {

        }
        private void OnUserUpdated(ClientUser user)
        {
            TreeNode node;
            if (userNodes.TryGetValue(user, out node))
            {
                node.Text = user.Name;
            }
        }

        private void OnUserJoinVoice(ClientUser user, ClientChannel channel)
        {
            TreeNode channelNode;
            if (channelNodes.TryGetValue(channel, out channelNode))
            {
                string key = "u" + user.UserID.ToString();
                TreeNode node = channelNode.Nodes.Add(key, user.Name);
                userNodes.Add(user, node);

                if (user.IsLocal)
                    channelNode.Expand();
            }
        }
        private void OnUserLeaveVoice(ClientUser user, ClientChannel channel)
        {
            TreeNode node;
            if (userNodes.TryGetValue(user, out node))
            {
                node.Remove();
                userNodes.Remove(user);
            }
        }

        #endregion Client events


        #region Form events

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (client.State == Client.ClientState.Unconnected)
            {
                ConnectFrom connectFrom = new ConnectFrom(this);
                connectFrom.ShowDialog(this);
            }
            else if (client.State == Client.ClientState.Connected)
            {
                client.Disconnect();
            }
        }

        private void buttonLeave_Click(object sender, EventArgs e)
        {
            client.LeaveVoiceChannel();
        }

        private void treeViewChannels_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewChannels.SelectedNode.Name[0] == 'c')
            {
                TreeNode node = treeViewChannels.SelectedNode;
                string channelIDStr = node.Name.Substring(1, node.Name.Length - 1);
                int channelID = int.Parse(channelIDStr);

                SelectChannel(client.Channels[channelID]);
            }
        }

        private void treeViewChannels_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeViewChannels.SelectedNode.Bounds.Contains(e.Location))
            {
                client.JoinVoiceChannel(selectedChannel);
            }
        }

        private void chatInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                buttonSend.PerformClick();
            }
            
            e.Handled = true;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (chatInputBox.Text.Length > 0)
            {
                client.SendChatMessage(selectedChannel, chatInputBox.Text);

                chatInputBox.Text = "";
            }
        }

        #endregion Form events
    }
}
