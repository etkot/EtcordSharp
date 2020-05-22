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
    public partial class ConnectFrom : Form
    {
        private MainForm mainForm;

        public ConnectFrom(MainForm mainForm)
        {
            this.mainForm = mainForm;

            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            mainForm.ConnectToServer(textBoxAddress.Text, textBoxUsername.Text);
            Close();
        }
    }
}
