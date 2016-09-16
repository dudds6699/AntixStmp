//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Host object</para>
    /// </summary>
    public class Host : IDisposable {
        private bool _disposed;

        /// <summary>
        ///   <para>Create object</para>
        /// </summary>
        internal Host(Server server) {
            if (server == null) throw new ArgumentNullException("server");

            Server = server;

            Log.Information("Host.Constructor: => {0}", server);
        }

        #region config properties

        /// <summary>
        ///   <para>Gets the host configuration</para>
        /// </summary>
        private HostConfiguration Configuration { get; set; }

        /// <summary>
        ///   <para>Gets the name of the host</para>
        ///   <para>Change via Configuration.Name</para>
        /// </summary>
        public string Name {
            get { return Configuration.Name; }
        }

        /// <summary>
        ///   <para>Gets the ip address of the host</para>
        ///   <para>Change via Configuration.IPAddress</para>
        /// </summary>
        public IPAddress IPAddress {
            get { return Configuration.IPAddress; }
        }

        /// <summary>
        ///   <para>Gets the port of the host</para>
        ///   <para>Change via Configuration.Port</para>
        /// </summary>
        public int Port {
            get { return Configuration.Port; }
        }

        #region host entry

        private IPHostEntry _dnsHostEntry;

        /// <summary>
        /// <para>DNS Host entry for this host</para>
        /// </summary>
        public IPHostEntry DNSHostEntry {
            get {
                return _dnsHostEntry
                       ?? (_dnsHostEntry = IPAddress.Equals(IPAddress.Any)
                                              ? Dns.GetHostEntry(Dns.GetHostName())
                                              : Dns.GetHostEntry(IPAddress));
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        ///   <para>Dispose</para>
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        ///   <para>Finalize object</para>
        /// </summary>
        ~Host() {
            Dispose(false);
        }

        /// <summary>
        ///   <para>String representation</para>
        ///   <para>If name is null and ip is Any then uses the last ip address in the localhost address list</para>
        /// </summary>
        /// <returns>IP:Port, eg 0.0.0.0:25</returns>
        public override string ToString() {
            return string.IsNullOrWhiteSpace(Name)
                       ? string.Format("{0}:{1}", DNSHostEntry.AddressList.LastOrDefault(), Configuration.Port)
                       : Name;
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                Log.Information("Host.Dispose Begin: {0}", this);

                if (disposing) {
                    if (Messages != null) {
                        Messages.Dispose();
                        Messages = null;
                    }

                    Stop();
                }
                Log.Information("Host.Dispose End: {0}", this);
            }
            _disposed = true;
        }

        #region properties/constants

        private TcpListener _listener;
        private List<Session> _sessions;
        private Thread _thread;

        /// <summary>
        ///   <para>Gets reference to the server</para>
        /// </summary>
        public Server Server { get; private set; }

        /// <summary>
        ///   <para>Message Store</para>
        /// </summary>
        public IMessageStorage Messages { get; private set; }

        #endregion

        #region events

        public event EventHandler<HostEventArgs> Event;

        /// <summary>
        ///   <para>Raise event</para>
        /// </summary>
        /// <param name = "type">Event Type</param>
        /// <param name = "session">Session</param>
        internal void RaiseEvent(HostEventTypes type, Session session) {
            if (Event == null) return;

            Log.Information("Host.RaiseEvent Begin: {0} => type: {1}, session: {2}", this, type, session);

            Event(this, new HostEventArgs(type, session));

            Log.Information("Host.RaiseEvent End: {0} => type: {1}, session: {2}", this, type, session);
        }

        #endregion

        #region configure/start/stop

        /// <summary>
        ///   <para>Current state</para>
        /// </summary>
        public HostStates Status { get; private set; }

        /// <summary>
        ///   <para>Configure this host</para>
        ///   <para>Applys the configuration settings to the host, creates the message storage</para>
        ///   <para>This will stop the host</para>
        /// </summary>
        /// <param name = "config">Configuration</param>
        public void Configure(HostConfiguration config) {
            Log.Information("Host.Configure Begin: => {0}", config);

            Stop();

            if (config.IPAddress == null) throw new NullReferenceException("config.IPAddress");
            Configuration = config;
            Messages = config.MessageStorage.Create(config);

            _dnsHostEntry = null;
            _sessions = new List<Session>();

            Log.Information("Host.Configure End: {0}", this);
        }

        /// <summary>
        ///   <para>Start on a new thread</para>
        /// </summary>
        /// <exception cref = "SocketException" />
        public void Start() {
            if (Status == HostStates.Started) return;

            Log.Information("Host.Start Begin: {0}", this);
            try {
                var permission = new SocketPermission(
                    NetworkAccess.Connect, TransportType.Tcp,
                    Configuration.IPAddressString, Configuration.Port);
                Log.Information("Host.Start Permission Demand");
                permission.Demand();
                Log.Information("Host.Start Permission Granted");

                Log.Information("Host.Start Start Listener on {0}", Configuration);
                _listener = new TcpListener(Configuration.IPAddress, Configuration.Port);

                _listener.Start((int) SocketOptionName.MaxConnections);
                Status = HostStates.Started;

                Log.Information("Host.Start Listening on {0}", Configuration);

                var acceptCallback = default(Action<IAsyncResult>);
                acceptCallback = r => {
                                     if (Status != HostStates.Started) return;

                                     var listener = (TcpListener) r.AsyncState;
                                     lock (_sessions) {
                                         // add new session
                                         _sessions.Add(new Session(this, listener.EndAcceptSocket(r)));

                                         // remove disconnected
                                         _sessions.RemoveAll(s => s.Status == Session.States.Disconnected);
                                     }

                                     // listen again
                                     _listener.BeginAcceptSocket(new AsyncCallback(acceptCallback), r.AsyncState);
                                 };

                for (var i = 0; i < 5; i++)
                    _listener.BeginAcceptSocket(new AsyncCallback(acceptCallback), _listener);

                RaiseEvent(HostEventTypes.Started, null);
            }
            catch (Exception ex) {
                Log.Error(ex);
                Stop();
                Status = HostStates.Error;
                throw;
            }
            finally {
                Log.Information("Host.Start End: {0}", this);
            }
        }

        /// <summary>
        ///   <para>Stop the Host</para>
        /// </summary>
        public void Stop() {
            if (Status != HostStates.Started) return;

            Log.Information("Host.Stop Begin: {0}", this);
            try {
                Status = HostStates.Stopped;
                RaiseEvent(HostEventTypes.Stopped, null);

                foreach (var session in _sessions) session.Dispose();
                _sessions.Clear();

                if (_listener != null) {
                    _listener.Stop();

                    Log.Information("Host.Stop Stopped: {0}", this);
                }

                if (_thread != null) {
                    _thread.Join();
                }
            }
            finally {
                _listener = null;
                _thread = null;
                Log.Information("Host.Stop End: {0}", this);
            }
        }

        #endregion

        #region Test

        /// <summary>
        ///   <para>Test combo can be started</para>
        /// </summary>
        /// <param name = "ipAddress">IP Address</param>
        /// <param name = "port">Port</param>
        /// <returns>True if successfully started</returns>
        public static bool Test(IPAddress ipAddress, int port) {
            Log.Information("Host.Test Begin");

            var listener = new TcpListener(ipAddress, port);
            Log.Information("Host.Test Listener created");
            try {
                var permission = new SocketPermission(
                    NetworkAccess.Connect, TransportType.Tcp,
                    ipAddress.ToString(), port);
                Log.Information("Host.Test perission created");
                permission.Demand();
                Log.Information("Host.Test perission demanded");

                listener.Start();
                Log.Information("Host.Test listener started");

                return true;
            }
            catch (Exception) {
                return false;
            }
            finally {
                try {
                    // stop ignoring errors
                    listener.Stop();
                }
                catch (Exception ex) {
                    Log.Error(ex);
                }
                Log.Information("Host.Test End");
            }
        }

        #endregion
    }
}