//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Windows.Forms;

namespace Antix.Mail.Tester {
    public partial class MessageForm : Form {
        public MessageForm() {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, EventArgs e) {
            var client = default(SmtpClient);
            try {
                client = new SmtpClient(ServerControl.Text, int.Parse(PortControl.Text));
                var message = new MailMessage();

                message.From = new MailAddress(fromTextBox.Text);
                message.To.Add(toTextBox.Text);
                message.Subject = subjectTextbox.Text;
                message.Body = bodyTextbox.Text;

                if (File.Exists(attachmentTextBox.Text)) {
                    message.Attachments.Add(
                        new Attachment(attachmentTextBox.Text)
                        );
                }

                client.Send(message);

                statusLabel.Text = string.Format("'{0}' send on {1}", subjectTextbox.Text, DateTime.Now);
            }
            catch (Exception ex) {
                Debug.Write(ex.ToString());
                statusLabel.Text = ex.Message;
            }
            finally {
                if (client != null) client.Dispose();
            }
        }

        private void attachmentButton_Click(object sender, EventArgs e) {
            try {
                if (attachmentFileDialog.ShowDialog() == DialogResult.OK) {
                    attachmentTextBox.Text = attachmentFileDialog.FileName;
                }
            }
            catch (Exception ex) {
                Debug.Write(ex.ToString());
                statusLabel.Text = ex.Message;
            }
        }
    }
}