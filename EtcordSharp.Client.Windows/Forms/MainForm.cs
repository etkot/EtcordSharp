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


        public MainForm()
        {
            InitializeComponent();

            client = new Client();

            receiveTimer = new Timer();
            receiveTimer.Interval = 15;
            receiveTimer.Tick += (s, e) => client.Receive();
            receiveTimer.Start();

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
            listBoxChannels.Items.Add(channel);
        }

        private void OnChannelUpdated(ClientChannel channel)
        {
            listBoxChannels.Update();
        }

        private void OnMessageAdded(ClientMessage message)
        {
            if (selectedChannel == message.Channel)
            {
                chatBox.AppendText("\r\n<" + message.SenderName + "> " + message.Content);
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

        private void listBoxChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectChannel((ClientChannel)listBoxChannels.SelectedItem);
        }

        private void chatInputBox_KeyUp(object sender, KeyEventArgs e)
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
                client.SendMessage(selectedChannel, chatInputBox.Text);

                chatInputBox.Text = "";
            }
        }

        #endregion Form events
    }
}
