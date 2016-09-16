//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

using Antix.Mail.Smtp.Impostor.Client.Properties;

using Application = System.Windows.Application;

namespace Antix.Mail.Smtp.Impostor.Client {
    /// <summary>
    ///   <para>Interface for a window or control using status</para>
    /// </summary>
    public interface IStatusParent {
        bool IsVisible { get; }
    }

    /// <summary>
    ///   <para>Possible status</para>
    /// </summary>
    public enum States {
        Info,
        Warning,
        Error
    }

    /// <summary>
    ///   <para>A class for status</para>
    /// </summary>
    public class Status {
        private readonly NotifyIcon _notifyIcon;
        private readonly IStatusParent _parent;
        private readonly TextBlock _textControl;

        public Status(IStatusParent parent, TextBlock textControl, NotifyIcon notifyIcon) {
            _parent = parent;
            _textControl = textControl;
            _notifyIcon = notifyIcon;
        }

        public Status(IStatusParent parent, TextBlock textControl)
            : this(parent, textControl, null) {}

        public void Set(Exception ex) {
            try {
                Set(States.Error, Resources.Unexpected_Exception);
                Log(ex);
            }
            catch (Exception) {
                // cannot write to the event log
            }
        }

        /// <summary>
        ///   <para>Set the status on the window and if not visible popup an info bubble</para>
        /// </summary>
        public void Set(States state, string message, params object[] args) {
            try {
                _textControl.Dispatcher.Invoke(
                    DispatcherPriority.Render,
                    (Action) delegate {
                                 if (args != null &&
                                     args.Length > 0) {
                                     message = string.Format(message, args);
                                 }

                                 _textControl.Text = message;
                                 var icon = ToolTipIcon.Info;
                                 switch (state) {
                                     case States.Info:
                                         _textControl.Foreground = Brushes.Black;
                                         break;
                                     case States.Warning:
                                         _textControl.Foreground = Brushes.DarkGoldenrod;
                                         icon = ToolTipIcon.Warning;
                                         break;
                                     case States.Error:
                                         _textControl.Foreground = Brushes.DarkRed;
                                         icon = ToolTipIcon.Error;
                                         break;
                                 }

                                 if (_notifyIcon != null &&
                                     !_parent.IsVisible) {
                                     _notifyIcon.ShowBalloonTip(
                                         2500, Resources.Title,
                                         message,
                                         icon);
                                 }
                             });
            }
            catch (Exception ex) {
                Log(ex);
            }
        }

        internal static void Log(Exception ex) {
            try {
                EventLog.WriteEntry(
                    Application.ResourceAssembly.GetName().Name, ex.ToString(),
                    EventLogEntryType.Error);
            }
            catch (Exception eventLogEx) {
                throw new Exception(
                    Resources.Log_Failed_Exception, eventLogEx);
            }
        }
    }
}