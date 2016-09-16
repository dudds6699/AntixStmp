//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Threading;

using Antix.Mail.Smtp.Impostor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antix.Mail.Tests {
    /// <summary>
    ///   <para>Tests for Impostor</para>
    /// </summary>
    [TestClass]
    public class ImpostorTests {

        private const string IPADDRESS = "0.0.0.0";  // Any
        private const int PORT = 25;
        private const int MESSAGE_COUNT = 10;
        private const string SUBJECT = "Test E-mail Æ Ø Å";
        private const string BODY = "This is a test";
        private const string FROM_EMAIL = "from@localhost";
        private const string FROM_NAME = "From Mail Åddress";
        private const string TO_EMAIL = "to@localhost";
        private const string TO_NAME = "To Mail Address";
        private const string TO2_EMAIL = "to2@localhost";
        private const string TO2_NAME = "To 2 Mail Address";

        [ClassInitialize]
        public static void TestInit(TestContext context) {
            // open the temp path
            try {
                Process.Start(Path.GetTempPath());
            }
// ReSharper disable EmptyGeneralCatchClause
                // ignore errors here, not testing this
            catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
            }
        }

        [TestMethod]
        [Description("Start, stop and dispose of a host")]
        public void StartStopDisposeHost() {
            var ipAddress = IPAddress.Parse(IPADDRESS);

            var server = new Server();
            try {
                var host = server.CreateHost(new HostConfiguration
                                             {
                                                 IPAddress = ipAddress,
                                                 Port = PORT
                                             });
                host.Messages.DeleteAll();

                Assert.AreEqual(1, server.Hosts.Count);
                Assert.IsNotNull(server.Hosts.First());
                Assert.AreEqual(ipAddress, server.Hosts.First().IPAddress);
                Assert.AreEqual(PORT, server.Hosts.First().Port);

                host.Start();
                Assert.AreEqual(HostStates.Started, host.Status);

                SendEmails(server, 1);

                host.Stop();
                Assert.AreEqual(HostStates.Stopped, host.Status);

                host.Start();
                Assert.AreEqual(HostStates.Started, host.Status);

                SendEmails(server, 1);

                host.Dispose();
                Assert.AreEqual(HostStates.Stopped, host.Status);
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        [Description("Create duplicate host")]
        public void CreateDuplicateHost() {
            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration {Port = PORT});
                host.Start();

                host = server.CreateHost(new HostConfiguration {Port = PORT});
                host.Start();

                Assert.Fail("Created duplicate host");
            }
            catch (Exception ex) {
                Assert.IsInstanceOfType(ex, typeof(SocketException));
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        [Description("Create a host and send e-mail to it from a number of threads")]
        public void CreateHostSendEmail_Threads() {
            var server = new Server();
            var threads = new List<Thread>();
            try {
                var host = server.CreateHost(new HostConfiguration {Port = PORT});
                host.Messages.DeleteAll();
                host.Start();

                var startedOn = DateTime.Now;

                for (var i = 0; i < 50; i++) {
                    Trace.TraceInformation("sending on thread {0}", i);

                    var thread = new Thread(SendEmails);
                    thread.Start(server);

                    threads.Add(thread);
                }

                foreach (var thread in threads) {
                    thread.Join();
                }

                Trace.TraceInformation(string.Format("Done in {0}", DateTime.Now - startedOn));
            }
            catch (Exception ex) {
                EventLog.WriteEntry("Antix.Mail.Tests", ex.ToString(), EventLogEntryType.Error);
            }
            finally {
                server.Dispose();

                Trace.TraceInformation("Finally");
            }
        }

        [TestMethod]
        [Description("Create a host and send e-mail to it")]
        public void CreateHostSendEmail() {
            var server = new Server();
            try {
                var host = server.CreateHost(new HostConfiguration {
                    IPAddress = IPAddress.Parse(IPADDRESS),
                    Port = PORT
                });
                host.Messages.DeleteAll();
                host.Start();

                SendEmails(server);

                Assert.IsNotNull(host.Messages.FirstOrDefault());
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        [Description("Create a host start and stop and send e-mail to it")]
        public void CreateHostSendEmail_Stopped() {
            var server = new Server();
            try {
                var host = server.CreateHost(new HostConfiguration {Port = PORT});
                host.Messages.DeleteAll();
                host.Start();
                host.Stop();

                // send a _message
                var message = new MailMessage
                              {
                                  From = new MailAddress(FROM_EMAIL, FROM_NAME),
                                  Subject = SUBJECT,
                                  Body = BODY
                              };
                message.To.Add(new MailAddress(TO_EMAIL, TO_NAME));

                var smtpClient = new SmtpClient(host.DNSHostEntry.HostName, host.Port);
                smtpClient.Send(message);

                Assert.Fail("shouldn't get here");
            }
            catch (SmtpException ex) {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(WebException));
                Assert.IsNotNull(ex.InnerException.InnerException);
                Assert.IsInstanceOfType(ex.InnerException.InnerException, typeof(SocketException));

                var socketException = (SocketException) ex.InnerException.InnerException;
                Assert.AreEqual(10061 /* Connection Refused */, socketException.ErrorCode);
            }
            finally {
                server.Dispose();
            }
        }

        #region send e-mails

        public void SendEmails(object serverObject) {
            try {
                SendEmails((Server) serverObject, MESSAGE_COUNT);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public static void SendEmails(Server server, int sendCount) {
            var host = server.Hosts.First();
            Assert.AreEqual(HostStates.Started, host.Status);

            // send a _message
            var message = new MailMessage
                          {
                              From = new MailAddress(FROM_EMAIL, FROM_NAME),
                              Subject = SUBJECT,
                              Body = BODY
                          };
            message.To.Add(new MailAddress(TO_EMAIL, TO_NAME));
            message.To.Add(new MailAddress(TO2_EMAIL, TO2_NAME));

            // add an attachment
            var resource = typeof(ImpostorTests).Assembly
                .GetManifestResourceStream("Antix.Mail.Tests.AnthonyJohnston.Rtf");
            if (resource == null)
                Assert.Fail("Could not load attachment resource 'Antix.Mail.Tests.AnthonyJohnston.Rtf'");
            message.Attachments.Add(new Attachment(resource, "AnthonyJohnston.Rtf"));

            var smtpClient = new SmtpClient(
                host.DNSHostEntry.HostName, // works, but, handled exception thrown (on my network anyway), ipv4 address works fine
                host.Port); 

            for (var i = 0; i < sendCount; i++) {
                message.Subject = SUBJECT;

                smtpClient.Send(message);
            }

            smtpClient.Dispose();

            // check messages
            foreach (var receivedMessage in host.Messages) {
                var from = receivedMessage.From.ToMailAddress();
                var to = receivedMessage.To.ToMailAddresses().ToArray();

                Assert.IsNotNull(receivedMessage);
                Assert.AreEqual(new MailAddress(FROM_EMAIL, FROM_NAME), from);
                Assert.AreEqual(SUBJECT, receivedMessage.Subject);
                Assert.AreEqual(2, to.Count());
                Assert.AreEqual(new MailAddress(TO_EMAIL, TO_NAME), to.ElementAt(0));
                Assert.AreEqual(new MailAddress(TO2_EMAIL, TO2_NAME), to.ElementAt(1));
                //Assert.IsNotNull(receivedMessage.Data);
            }
        }

        #endregion
    }
}