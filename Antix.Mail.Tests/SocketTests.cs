//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Antix.Mail.Smtp.Impostor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antix.Mail.Tests {
    /// <summary>
    ///   Summary description for SocketTests
    /// </summary>
    [TestClass]
    public class SocketTests {
        #region client

        private class Client : IDisposable {
            private readonly IPEndPoint _endPoint;
            private Socket _socket;

            public Client(IPAddress ip, int port) {
                _socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _endPoint = new IPEndPoint(ip, port);

                var permission = new SocketPermission(
                    NetworkAccess.Connect, TransportType.Tcp,
                    ip.ToString(), port);
                permission.Demand();
            }

            #region IDisposable Members

            public void Dispose() {
                if (_socket != null) {
                    if (_socket.Connected) {
                        _socket.Shutdown(SocketShutdown.Both);
                    }
                    _socket.Close();
                    _socket = null;
                }
            }

            #endregion

            public string Connect() {
                _socket.Connect(_endPoint);
                return Read();
            }

            public string Send(string command) {
                Write(command);
                return Read();
            }

            private string Read() {
                while (_socket.Available == 0) {
                    Thread.Sleep(100);
                }
                var data = new byte[_socket.Available];
                _socket.Receive(data);

                var text = Encoding.ASCII.GetString(data);
                Trace.TraceInformation("READ:" + text);
                return text;
            }

            private void Write(string text) {
                Trace.TraceInformation("WRITE:" + text);
                _socket.Send(Encoding.ASCII.GetBytes(text + "\r\n"));
            }
        }

        #endregion

        private const string TEST_IP = "127.0.0.1";
        private const int TEST_PORT = 25;

        private const string RESPONSE_CONNECTED = "220";
        private const string RESPONSE_OK = "250 ";
        private const string RESPONSE_NOT_SUPPORTED = "502 ";

        private const string COMMAND_HELO = "HELO MY LUVER";
        private const string COMMAND_NOOP = "NOOP";
        private const string COMMAND_RSET = "RSET";
        private const string COMMAND_FAKE = "FAKE";
        private const string COMMAND_MAIL = "MAIL FROM:<from@localhost>";

        [TestMethod]
        public void Connect() {
            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration {IPAddressString = TEST_IP, Port = TEST_PORT});
                host.Start();

                using (var client = new Client(host.IPAddress, host.Port)) {
                    Assert.IsTrue(client.Connect().StartsWith(RESPONSE_CONNECTED));
                }
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        public void ConnectHELO() {
            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration {IPAddressString = TEST_IP, Port = TEST_PORT});
                var lastEventType = default(HostEventTypes);
                host.Event += (s, e) => { lastEventType = e.Type; };

                host.Start();

                using (var client = new Client(host.IPAddress, host.Port)) {
                    Assert.IsTrue(client.Connect().StartsWith(RESPONSE_CONNECTED));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionConnected, lastEventType);

                    Assert.IsTrue(client.Send(COMMAND_HELO).StartsWith(RESPONSE_OK));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionIdentified, lastEventType);
                }
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        public void ConnectRSET() {
            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration {IPAddressString = TEST_IP, Port = TEST_PORT});
                var lastEventType = default(HostEventTypes);
                host.Event += (s, e) => { lastEventType = e.Type; };

                host.Start();

                using (var client = new Client(host.IPAddress, host.Port)) {
                    Assert.IsTrue(client.Connect().StartsWith(RESPONSE_CONNECTED));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionConnected, lastEventType);

                    Assert.IsTrue(client.Send(COMMAND_HELO).StartsWith(RESPONSE_OK));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionIdentified, lastEventType);

                    Assert.IsTrue(client.Send(COMMAND_RSET).StartsWith(RESPONSE_OK));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionIdentified, lastEventType);

                    Assert.IsTrue(client.Send(COMMAND_MAIL).StartsWith(RESPONSE_OK));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionIdentified, lastEventType);
                }
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        public void ConnectNOOP() {
            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration {IPAddressString = TEST_IP, Port = TEST_PORT});
                var lastEventType = default(HostEventTypes);
                host.Event += (s, e) => { lastEventType = e.Type; };

                host.Start();

                using (var client = new Client(host.IPAddress, host.Port)) {
                    Assert.IsTrue(client.Connect().StartsWith(RESPONSE_CONNECTED));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionConnected, lastEventType);

                    Assert.IsTrue(client.Send(COMMAND_HELO).StartsWith(RESPONSE_OK));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionIdentified, lastEventType);

                    Assert.IsTrue(client.Send(COMMAND_NOOP).StartsWith(RESPONSE_OK));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionIdentified, lastEventType);
                }
            }
            finally {
                server.Dispose();
            }
        }

        [TestMethod]
        public void ConnectFAKE() {
            var server = new Server();

            try {
                var host = server.CreateHost(new HostConfiguration {IPAddressString = TEST_IP, Port = TEST_PORT});
                var lastEventType = default(HostEventTypes);
                host.Event += (s, e) => { lastEventType = e.Type; };

                host.Start();

                using (var client = new Client(host.IPAddress, host.Port)) {
                    Assert.IsTrue(client.Connect().StartsWith(RESPONSE_CONNECTED));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionConnected, lastEventType);

                    Assert.IsTrue(client.Send(COMMAND_HELO).StartsWith(RESPONSE_OK));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionIdentified, lastEventType);

                    Assert.IsTrue(client.Send(COMMAND_FAKE).StartsWith(RESPONSE_NOT_SUPPORTED));
                    Thread.Sleep(100);
                    Assert.AreEqual(HostEventTypes.SessionIdentified, lastEventType);
                }
            }
            finally {
                server.Dispose();
            }
        }
    }
}