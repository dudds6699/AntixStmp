//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

using Antix.Mail.Smtp.Impostor.Properties;

namespace Antix.Mail.Smtp.Impostor
{
    /// <summary>
    ///   <para>An SMTP session</para>
    ///   <para>Receives a message using the SMTP protocol</para>
    ///   <para>http://www.faqs.org/rfcs/rfc821.html</para>
    /// </summary>
    public class Session : IDisposable
    {
        private static readonly object LockObject = new Object();
        private bool _disposed;

        /// <summary>
        ///   <para>Create object</para>
        /// </summary>
        /// <param name = "host">Host</param>
        /// <param name = "socket">Socket</param>
        internal Session(Host host, Socket socket)
        {
            Host = host;
            _socket = socket;

            ClientAddress = ((IPEndPoint)_socket.RemoteEndPoint).Address;

            _networkStream = new NetworkStream(_socket, FileAccess.ReadWrite, true);
            Write(
                ReplyCodes.Ready_220,
                string.Format(Resources.Ready_220, ClientAddress));

            Status = States.Connected;
            Host.RaiseEvent(HostEventTypes.SessionConnected, this);

            Read(Server.LINE_TERMINATOR);
        }

        #region IDisposable Members

        /// <summary>
        ///   <para>Dispose</para>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        ///   <para>Finalize object</para>
        /// </summary>
        ~Session()
        {
            Dispose(false);
        }

        /// <summary>
        ///   <para>String representation</para>
        /// </summary>
        /// <returns>ClientId if set or ClientAddress</returns>
        public override string ToString()
        {
            return ClientId ?? ClientAddress.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (LockObject)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        try
                        {
                            _socket.Shutdown(SocketShutdown.Both);
                            _socket.Close();

                            Host.RaiseEvent(HostEventTypes.SessionDisconnected, this);
                        }
                        finally
                        {
                            _networkStream = null;
                            _socket = null;
                        }
                    }
                }
                _disposed = true;
            }
        }

        #region constants/enums

        /// <summary>
        ///   <para>Status values</para>
        /// </summary>
        public enum States
        {
            /// <summary>
            ///   SessionConnected to client
            /// </summary>
            Connected,

            /// <summary>
            ///   Client has been identified
            /// </summary>
            Identified,

            /// <summary>
            ///   Receiving e-mail data
            /// </summary>
            Mail,

            /// <summary>
            ///   Receiving recipient data
            /// </summary>
            Recipient,

            /// <summary>
            ///   Receiving payload data
            /// </summary>
            Data,

            /// <summary>
            ///   SessionDisconnected from client
            /// </summary>
            Disconnected
        }

        #region Nested type: Commands

        /// <summary>
        ///   <para>Commands</para>
        /// </summary>
        internal static class Commands
        {
            public const string EHLO = "EHLO";
            public const string HELO = "HELO";
            public const string MAIL = "MAIL";
            public const string RCPT = "RCPT";
            public const string DATA = "DATA";
            public const string QUIT = "QUIT";
            public const string RSET = "RSET";
            public const string NOOP = "NOOP";
        }

        #endregion

        #region Nested type: ReplyCodes

        /// <summary>
        ///   <para>Reply Codes</para>
        ///   <para>The code in the name is a reminder only</para>
        /// </summary>
        internal enum ReplyCodes
        {
            Ready_220 = 220,
            Completed_250 = 250,
            StartInput_354 = 354,
            SyntaxError_500 = 500,
            ParameterError_501 = 501,
            CommandNotImplemented_502 = 502,
            CommandSequenceError_503 = 503,
            CommandParameterNotImplemented_504 = 504
        }

        #endregion

        #endregion

        #region properties/fields

        private StringBuilder _data;

        private MailAddress _from;
        private NetworkStream _networkStream;
        private byte[] _readBuffer;
        private Socket _socket;
        private string _terminator;
        private List<MailAddress> _to;

        public Host Host { get; private set; }
        public States Status { get; private set; }
        public IPAddress ClientAddress { get; private set; }
        public string ClientId { get; private set; }

        #endregion

        #region read

        /// <summary>
        ///   <para>Set up a read to a terminator</para>
        /// </summary>
        /// <param name = "terminator"></param>
        private void Read(string terminator)
        {
            _terminator = terminator;
            _data = new StringBuilder();

            Read();
        }

        /// <summary>
        ///   <para>Sets up an async read</para>
        /// </summary>
        private void Read()
        {
            if (_networkStream == null) return;

            try
            {
                _readBuffer = new byte[Settings.Default.SessionReadBufferSize];
                _networkStream.BeginRead(
                    _readBuffer, 0, _readBuffer.Length,
                    ReadCallback,
                    this);
            }
            catch (Exception)
            {
                Dispose();
            }
        }

        /// <summary>
        ///   <para>Read the data</para>
        /// </summary>
        /// <param name = "result"></param>
        //[DebuggerStepThrough]
        private void ReadCallback(IAsyncResult result)
        {
            if (_networkStream == null) return;

            try
            {
                var readData = Encoding.UTF8.GetString(
                    _readBuffer,
                    0, _networkStream.EndRead(result));
                    _data.Append(readData);  
                // check for terminator
                if (_data.Length > _terminator.Length
                    && _data.ToString(_data.Length - _terminator.Length, _terminator.Length).Equals(_terminator))
                {
                    // process data received not including the terminator
                    Process(
                        _data.ToString(
                            0, _data.Length - _terminator.Length));
                }
                else
                {
                    // read some more data
                    Read();
                }
            }
            catch (Exception)
            {
                Dispose();
            }
        }

        #endregion

        #region write

        private void Write(string data)
        {
            Log.Information("Session.Write: => {0}", data);

            data += Server.LINE_TERMINATOR;

            var bytes = Encoding.UTF8.GetBytes(data);
            _networkStream.Write(bytes, 0, bytes.Length);
        }

        private void Write(ReplyCodes code, string description)
        {
            Write(String.Format("{0:D} {1}", code, description));
        }

        private void Write(ReplyCodes code)
        {
            Write(code, Resources.ResourceManager.GetString(code.ToString()));
        }

        #endregion

        #region process

        /// <summary>
        ///   <para>Process the current data read</para>
        /// </summary>
        private void Process(string data)
        {
            var terminator = Server.LINE_TERMINATOR;
            try
            {
                if (Status == States.Data)
                {
                    var headers = GetHeaders(_data.ToString(0, _data.Length - _terminator.Length));
                    var message = new Message
                    {
                        Id = headers.ContainsKey(Server.HEADER_MESSAGE_ID)
                                               ? headers[Server.HEADER_MESSAGE_ID]
                                               : Guid.NewGuid().ToString(),
                        Subject = headers.ContainsKey(Server.HEADER_SUBJECT)
                                                    ? headers[Server.HEADER_SUBJECT]
                                                    : string.Empty,
                        From = headers.ContainsKey(Server.HEADER_FROM)
                                                 ? headers[Server.HEADER_FROM].ToMailAddress()
                                                 : _from,
                        To = _to,
                        ReceivedOn = DateTime.Now,
                        Headers = headers,
                        Data = _data.ToString(0, _data.Length - _terminator.Length)
                    };

                    // raise event
                    Host.Messages.Store(message);
                    Host.RaiseEvent(HostEventTypes.SessionMessageReceived, this);

                    Log.Information("Session.Process Data: => {0}", data);

                    Write(ReplyCodes.Completed_250);
                    Status = States.Identified;
                    _to = null;

                    return;
                }

                // command expected
                var command = data.Length < 4
                                  ? string.Empty
                                  : data.Substring(0, 4).ToUpper();
                Log.Information("Session.Process Command: => {0}", command);

                if (command.Equals(Commands.QUIT))
                {
                    Dispose();
                }
                else if (command.Equals(Commands.EHLO)
                         || command.Equals(Commands.HELO))
                {
                    ClientId = data.Substring(4).Trim();
                    Log.Information("Session.Process ClientId: => {0}", ClientId);

                    Write(ReplyCodes.Completed_250, String.Format(Resources.Hello_250, ClientId));

                    Status = States.Identified;
                    Host.RaiseEvent(HostEventTypes.SessionIdentified, this);
                }
                else if (command.Equals(Commands.NOOP))
                {
                    Write(ReplyCodes.Completed_250, String.Format(Resources.Hello_250, ClientId));
                }
                else if (command.Equals(Commands.RSET))
                {
                    Write(ReplyCodes.Completed_250, String.Format(Resources.Hello_250, ClientId));

                    _from = null;
                    _to = null;

                    Status = States.Identified;
                    Host.RaiseEvent(HostEventTypes.SessionIdentified, this);
                }
                else if (Status < States.Identified)
                {
                    Write(ReplyCodes.CommandSequenceError_503, Resources.ExpectedHELO_503);
                }
                else if (command.Equals(Commands.MAIL))
                {
                    if (Status != States.Identified)
                    {
                        Write(ReplyCodes.CommandSequenceError_503);
                    }
                    else
                    {
                        // store the from address, in case its not in the header
                        _from = data.Tail(":").ToMailAddress();
                        Log.Information("Session.Process From: => {0}", _from);

                        Status = States.Mail;
                        Write(ReplyCodes.Completed_250);
                    }
                }
                else if (command.Equals(Commands.RCPT))
                {
                    if (Status != States.Mail
                        && Status != States.Recipient)
                    {
                        Write(ReplyCodes.CommandSequenceError_503);
                    }
                    else
                    {
                        if (_to == null)
                            _to = new List<MailAddress>();
                        var to = data.Tail(":").ToMailAddress();
                        _to.Add(to);
                        Log.Information("Session.Process To: => {0}", to);

                        Status = States.Recipient;
                        Write(ReplyCodes.Completed_250);
                    }
                }
                else if (command.Equals(Commands.DATA))
                {
                    // request data
                    Status = States.Data;
                    Write(ReplyCodes.StartInput_354);

                    terminator = Server.DATA_TERMINATOR;
                }
                else
                {
                    Write(ReplyCodes.CommandNotImplemented_502);
                }
            }
            catch (InvalidMailAddressException ex)
            {
                Write(
                    ReplyCodes.ParameterError_501,
                    string.Format("Invalid Address '{0}'", ex.Address));
            }
            finally
            {
                if (!_disposed)
                {
                    // next read
                    Read(terminator);
                }
            }
        }

        /// <summary>
        ///   <para>Get the headers from the data passed in</para>
        /// </summary>
        /// <param name = "data">Data</param>
        /// <returns>Header collection</returns>
        internal static Dictionary<string, string> GetHeaders(string data)
        {
            var headers = new Dictionary<string, string>();
            if (!data.Contains(Server.HEADERS_TERMINATOR)) return headers;

            // get the headers part of the data an un-fold 
            var rawHeaders = data.Head(Server.HEADERS_TERMINATOR)
                .Replace(Server.LINE_TERMINATOR + "\t", string.Empty)
                .Replace(Server.LINE_TERMINATOR + " ", string.Empty);

            // parse into collection
            foreach (var header in rawHeaders.Split('\n'))
            {
                var value = header;
                var name = Extensions.Head(ref value, ":")
                    .Trim().ToLower();

                value = value.Trim();

                if (name.Length > 0)
                {
                    // add as is
                    headers.Add(
                        name,
                        Regex.Replace(value, Server.ENCODED_WORD_PATTERN,
                                      m =>
                                      {
                                          var encoding = Encoding.GetEncoding(m.Groups[1].Value);

                                          return m.Groups[2].Value.Equals("b",
                                                                          StringComparison.InvariantCultureIgnoreCase)
                                                     ? encoding.GetString(Convert.FromBase64String(m.Groups[3].Value))
                                                     : m.Groups[3].Value.FromEncodedWord(encoding);

                                          // assume quoted printable
                                      }, RegexOptions.IgnoreCase)
                        );
                }
            }

            return headers;
        }

        #endregion
    }
}