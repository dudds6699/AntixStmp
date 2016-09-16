namespace Antix.Mail.Tester {
    partial class MessageForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.fromLabel = new System.Windows.Forms.Label();
            this.fromTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toTextBox = new System.Windows.Forms.TextBox();
            this.toLabel = new System.Windows.Forms.Label();
            this.subjectTextbox = new System.Windows.Forms.TextBox();
            this.subjectLabel = new System.Windows.Forms.Label();
            this.bodyTextbox = new System.Windows.Forms.TextBox();
            this.bodyLabel = new System.Windows.Forms.Label();
            this.ServerControl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PortControl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.attachmentFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.attachmentTextBox = new System.Windows.Forms.TextBox();
            this.attachmentLabel = new System.Windows.Forms.Label();
            this.attachmentButton = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // fromLabel
            // 
            this.fromLabel.Location = new System.Drawing.Point(12, 15);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(69, 17);
            this.fromLabel.TabIndex = 0;
            this.fromLabel.Text = "From";
            // 
            // fromTextBox
            // 
            this.fromTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fromTextBox.Location = new System.Drawing.Point(87, 12);
            this.fromTextBox.Name = "fromTextBox";
            this.fromTextBox.Size = new System.Drawing.Size(351, 20);
            this.fromTextBox.TabIndex = 1;
            this.fromTextBox.Text = "Admin Chap <administrator@localhost>";
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.Location = new System.Drawing.Point(363, 214);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 2;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 240);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(450, 22);
            this.statusStrip.TabIndex = 3;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(435, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toTextBox
            // 
            this.toTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toTextBox.Location = new System.Drawing.Point(87, 38);
            this.toTextBox.Name = "toTextBox";
            this.toTextBox.Size = new System.Drawing.Size(351, 20);
            this.toTextBox.TabIndex = 5;
            this.toTextBox.Text = "Admin Chap <administrator@localhost>";
            // 
            // toLabel
            // 
            this.toLabel.Location = new System.Drawing.Point(12, 41);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(69, 17);
            this.toLabel.TabIndex = 4;
            this.toLabel.Text = "To";
            // 
            // subjectTextbox
            // 
            this.subjectTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.subjectTextbox.Location = new System.Drawing.Point(87, 64);
            this.subjectTextbox.Name = "subjectTextbox";
            this.subjectTextbox.Size = new System.Drawing.Size(351, 20);
            this.subjectTextbox.TabIndex = 7;
            this.subjectTextbox.Text = "Test Subject Æ Ø Å";
            // 
            // subjectLabel
            // 
            this.subjectLabel.Location = new System.Drawing.Point(12, 67);
            this.subjectLabel.Name = "subjectLabel";
            this.subjectLabel.Size = new System.Drawing.Size(69, 17);
            this.subjectLabel.TabIndex = 6;
            this.subjectLabel.Text = "Subject";
            // 
            // bodyTextbox
            // 
            this.bodyTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bodyTextbox.Location = new System.Drawing.Point(87, 90);
            this.bodyTextbox.Multiline = true;
            this.bodyTextbox.Name = "bodyTextbox";
            this.bodyTextbox.Size = new System.Drawing.Size(351, 92);
            this.bodyTextbox.TabIndex = 9;
            this.bodyTextbox.Text = "Test Message";
            // 
            // bodyLabel
            // 
            this.bodyLabel.Location = new System.Drawing.Point(12, 93);
            this.bodyLabel.Name = "bodyLabel";
            this.bodyLabel.Size = new System.Drawing.Size(69, 17);
            this.bodyLabel.TabIndex = 8;
            this.bodyLabel.Text = "Body";
            // 
            // ServerControl
            // 
            this.ServerControl.Location = new System.Drawing.Point(87, 214);
            this.ServerControl.Name = "ServerControl";
            this.ServerControl.Size = new System.Drawing.Size(132, 20);
            this.ServerControl.TabIndex = 11;
            this.ServerControl.Text = "localhost";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "Server";
            // 
            // PortControl
            // 
            this.PortControl.Location = new System.Drawing.Point(225, 214);
            this.PortControl.Name = "PortControl";
            this.PortControl.Size = new System.Drawing.Size(39, 20);
            this.PortControl.TabIndex = 12;
            this.PortControl.Text = "25";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(216, 214);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = ":";
            // 
            // attachmentTextBox
            // 
            this.attachmentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.attachmentTextBox.Location = new System.Drawing.Point(87, 188);
            this.attachmentTextBox.Name = "attachmentTextBox";
            this.attachmentTextBox.Size = new System.Drawing.Size(312, 20);
            this.attachmentTextBox.TabIndex = 15;
            // 
            // attachmentLabel
            // 
            this.attachmentLabel.Location = new System.Drawing.Point(12, 191);
            this.attachmentLabel.Name = "attachmentLabel";
            this.attachmentLabel.Size = new System.Drawing.Size(69, 17);
            this.attachmentLabel.TabIndex = 14;
            this.attachmentLabel.Text = "Attachment";
            // 
            // attachmentButton
            // 
            this.attachmentButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.attachmentButton.Location = new System.Drawing.Point(405, 188);
            this.attachmentButton.Name = "attachmentButton";
            this.attachmentButton.Size = new System.Drawing.Size(33, 20);
            this.attachmentButton.TabIndex = 16;
            this.attachmentButton.Text = "...";
            this.attachmentButton.UseVisualStyleBackColor = true;
            this.attachmentButton.Click += new System.EventHandler(this.attachmentButton_Click);
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 262);
            this.Controls.Add(this.attachmentButton);
            this.Controls.Add(this.attachmentTextBox);
            this.Controls.Add(this.attachmentLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PortControl);
            this.Controls.Add(this.ServerControl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bodyTextbox);
            this.Controls.Add(this.bodyLabel);
            this.Controls.Add(this.subjectTextbox);
            this.Controls.Add(this.subjectLabel);
            this.Controls.Add(this.toTextBox);
            this.Controls.Add(this.toLabel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.fromTextBox);
            this.Controls.Add(this.fromLabel);
            this.Name = "MessageForm";
            this.Text = "Test Message";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.TextBox fromTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.TextBox toTextBox;
        private System.Windows.Forms.Label toLabel;
        private System.Windows.Forms.TextBox subjectTextbox;
        private System.Windows.Forms.Label subjectLabel;
        private System.Windows.Forms.TextBox bodyTextbox;
        private System.Windows.Forms.Label bodyLabel;
        private System.Windows.Forms.TextBox ServerControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PortControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog attachmentFileDialog;
        private System.Windows.Forms.TextBox attachmentTextBox;
        private System.Windows.Forms.Label attachmentLabel;
        private System.Windows.Forms.Button attachmentButton;
    }
}

