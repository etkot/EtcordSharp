﻿namespace EtcordSharp.Client.Windows
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonConnect = new System.Windows.Forms.Button();
            this.chatBox = new System.Windows.Forms.TextBox();
            this.panelConnectionControls = new System.Windows.Forms.Panel();
            this.listBoxChannels = new System.Windows.Forms.ListBox();
            this.chatInputBox = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.panelChat = new System.Windows.Forms.Panel();
            this.panelConnectionControls.SuspendLayout();
            this.panelChat.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(3, 3);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // chatBox
            // 
            this.chatBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatBox.Location = new System.Drawing.Point(0, 0);
            this.chatBox.Multiline = true;
            this.chatBox.Name = "chatBox";
            this.chatBox.ReadOnly = true;
            this.chatBox.Size = new System.Drawing.Size(631, 406);
            this.chatBox.TabIndex = 1;
            // 
            // panelConnectionControls
            // 
            this.panelConnectionControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelConnectionControls.Controls.Add(this.buttonConnect);
            this.panelConnectionControls.Location = new System.Drawing.Point(10, 10);
            this.panelConnectionControls.Name = "panelConnectionControls";
            this.panelConnectionControls.Size = new System.Drawing.Size(800, 29);
            this.panelConnectionControls.TabIndex = 2;
            // 
            // listBoxChannels
            // 
            this.listBoxChannels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxChannels.FormattingEnabled = true;
            this.listBoxChannels.Location = new System.Drawing.Point(10, 45);
            this.listBoxChannels.Name = "listBoxChannels";
            this.listBoxChannels.Size = new System.Drawing.Size(163, 433);
            this.listBoxChannels.TabIndex = 3;
            this.listBoxChannels.SelectedIndexChanged += new System.EventHandler(this.listBoxChannels_SelectedIndexChanged);
            // 
            // chatInputBox
            // 
            this.chatInputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatInputBox.Location = new System.Drawing.Point(0, 412);
            this.chatInputBox.Name = "chatInputBox";
            this.chatInputBox.ReadOnly = true;
            this.chatInputBox.Size = new System.Drawing.Size(570, 20);
            this.chatInputBox.TabIndex = 4;
            this.chatInputBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.chatInputBox_KeyUp);
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Location = new System.Drawing.Point(573, 410);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(55, 23);
            this.buttonSend.TabIndex = 5;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // panelChat
            // 
            this.panelChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelChat.Controls.Add(this.chatBox);
            this.panelChat.Controls.Add(this.buttonSend);
            this.panelChat.Controls.Add(this.chatInputBox);
            this.panelChat.Location = new System.Drawing.Point(179, 45);
            this.panelChat.Margin = new System.Windows.Forms.Padding(0);
            this.panelChat.Name = "panelChat";
            this.panelChat.Size = new System.Drawing.Size(631, 435);
            this.panelChat.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 489);
            this.Controls.Add(this.panelChat);
            this.Controls.Add(this.listBoxChannels);
            this.Controls.Add(this.panelConnectionControls);
            this.Name = "MainForm";
            this.Text = "EtcordSharp";
            this.panelConnectionControls.ResumeLayout(false);
            this.panelChat.ResumeLayout(false);
            this.panelChat.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox chatBox;
        private System.Windows.Forms.Panel panelConnectionControls;
        private System.Windows.Forms.ListBox listBoxChannels;
        private System.Windows.Forms.TextBox chatInputBox;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Panel panelChat;
    }
}

