//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;

namespace Antix.Mail.Smtp.Impostor {
    /// <summary>
    ///   <para>Used to create and manage sessions</para>
    /// </summary>
    public class Server : IDisposable {
        private bool disposed;

        /// <summary>
        ///   <para>Create Instance</para>
        /// </summary>
        public Server() {
            Hosts = new HostCollection();
        }

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
        ~Server() {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    foreach (var host in Hosts) host.Dispose();
                    Hosts.Clear();
                }
            }
            disposed = true;
        }

        #region Constants

        internal const string LINE_TERMINATOR = "\r\n";
        internal const string HEADERS_TERMINATOR = "\r\n\r\n";
        internal const string DATA_TERMINATOR = "\r\n.\r\n";

        internal const string QUOTE_UTF8 = "=?utf-8?q?";
        internal const string BASE64_UTF8 = "=?utf-8?b?";
        internal const string ENCODED_WORD_PATTERN = @"=\?([a-z0-9\-]+)\?([qb])\?(.+?)\?=";

        public const string HEADER_MESSAGE_ID = "_message-id";
        public const string HEADER_FROM = "from";
        public const string HEADER_TO = "to";
        public const string HEADER_SUBJECT = "subject";

        #endregion

        #region hosts

        /// <summary>
        ///   <para>All Hosts created by this server</para>
        /// </summary>
        public HostCollection Hosts { get; private set; }

        /// <summary>
        ///   <para>Create a new host</para>
        ///   <para>Hosts must be unique by ip and port</para>
        /// </summary>
        /// <param name = "config">Configuration</param>
        /// <returns>New host</returns>
        public Host CreateHost(HostConfiguration config) {
            var host = new Host(this);
            host.Configure(config);

            Hosts.Add(host);

            return host;
        }

        /// <summary>
        ///   <para>Create a default new host</para>
        /// </summary>
        /// <returns>New host</returns>
        public Host CreateHost() {
            return CreateHost(new HostConfiguration());
        }

        /// <summary>
        ///   <para>Remove a host</para>
        /// </summary>
        /// <param name = "host">Host to remove</param>
        public void RemoveHost(Host host) {
            Hosts.Remove(host);
        }

        #endregion
    }
}