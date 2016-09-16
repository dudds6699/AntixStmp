//  by Anthony J. Johnston, antix.co.uk

using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Antix.Mail.Smtp.Impostor.Properties;

namespace Antix.Mail.Smtp.Impostor.Client
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IStatusParent
    {
        public MainWindow()
        {
            InitializeComponent();

            #region notify icon

            _notifyIcon = new NotifyIcon
                              {
                                  Icon = Properties.Resources.LogoIcon,
                                  Visible = true
                              };
            _notifyIcon.Click += (sender, e) =>
                                     {
                                         Show();
                                         WindowState = _windowState;
                                     };

            #endregion

            App.Status = new Status(this, StatusControl, _notifyIcon);

            try
            {
                #region server

                App.Server = new Server();
                if (!Settings.Default.Hosts.Any())
                {
                    // always add a default
                    Settings.Default.Hosts.Add(
                        new HostConfiguration
                            {
                                IPAddress = IPAddress.Any,
                                Port = 25,
                                MessageStorage = new FileMessageStorageConfiguration()
                            });

                    Settings.Default.Save();
                }

                Action showHideClose = () =>
                                           {
                                               // only show remove if more than one
                                               ((HostTabItem) HostsControl.Items[0]).HostCloseImage.Visibility =
                                                   HostsControl.Items.Count == 2
                                                       ? Visibility.Collapsed
                                                       : Visibility.Visible;
                                           };

                Func<HostConfiguration, HostTabItem> addHost
                    = hostConfig =>
                          {
                              var host = App.Server.CreateHost(hostConfig);
                              var hostTab = new HostTabItem(host, hostConfig);
                              HostsControl.Items.Insert(
                                  HostsControl.Items.Count - 1, hostTab);

                              #region watch informative events

                              host.Event += (sender, e)
                                            => Dispatcher.Invoke(
                                                DispatcherPriority.Normal,
                                                (Action) (() =>
                                                              {
                                                                  switch (e.Type)
                                                                  {
                                                                      case HostEventTypes.SessionMessageReceived:
                                                                          App.Status.Set(
                                                                              States.Info,
                                                                              Properties.Resources.
                                                                                  MessageReceived_Message,
                                                                              e.Session);

                                                                          break;
                                                                      case
                                                                          HostEventTypes.Started:
                                                                          App.Status.Set(
                                                                              States.Info,
                                                                              Properties.Resources.Host_Started_Message,
                                                                              host,
                                                                              host.IPAddress,
                                                                              host.Port);

                                                                          break;
                                                                      case
                                                                          HostEventTypes.Stopped:
                                                                          App.Status.Set(
                                                                              States.Info,
                                                                              Properties.Resources.Host_Stopped_Message,
                                                                              host,
                                                                              host.IPAddress,
                                                                              host.Port);

                                                                          break;
                                                                  }
                                                              }));

                              #endregion

                              #region ui events

                              hostTab.HostCloseImage.MouseUp
                                  += (sender, e) =>
                                         {
                                             Settings.Default.Hosts.Remove(hostConfig);
                                             Settings.Default.Save();

                                             App.Server.RemoveHost(host);
                                             host.Stop();

                                             HostsControl.Items.Remove(hostTab);
                                             showHideClose();

                                             if (
                                                 HostsControl.SelectedIndex
                                                 == HostsControl.Items.Count - 1)
                                             {
                                                 HostsControl.SelectedIndex--;
                                                 // move to previous
                                             }
                                         };

                              #endregion

                              return hostTab;
                          };

                //Action newHost 
                //    = () =>
                //                     {
                //                         var hostConfig =
                //                             new HostConfiguration
                //                                 {
                //                                     IPAddress = IPAddress.Any,
                //                                     Port = 0,
                //                                     MessageStorage = new FileMessageStorageConfiguration()
                //                                 };
                //                         Settings.Default.Hosts.Add(hostConfig);
                //                         addHost(hostConfig).IsSelected = true;
                //                     };

                // add the new host tab
                HostsControl.Items.Add(new HostTabItem(addHost));

                // create hosts for each Model
                foreach (var hostConfig in Settings.Default.Hosts)
                {
                    addHost(hostConfig);
                }

                showHideClose();

                #endregion

                if (App.Server.Hosts.Any(h => h.Status == HostStates.Error))
                {
                    App.Status.Set(States.Error, Properties.Resources.Start_Failure_Message);
                }
                else
                {
                    App.Status.Set(States.Info, Properties.Resources.Start_Success_Message);
                }

                // show the version
                Title += " " + (
                                   ApplicationDeployment.IsNetworkDeployed
                                       ? ApplicationDeployment.CurrentDeployment.CurrentVersion
                                       : GetType().Assembly.GetName().Version
                               ).ToVersionString();
            }
            catch (Exception ex)
            {
                Status.Log(ex);
            }
        }

        #region window

        NotifyIcon _notifyIcon;
        WindowState _windowState = WindowState.Normal;

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized) Hide();
            else _windowState = WindowState;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (App.Server != null) App.Server.Dispose();
            if (_notifyIcon != null) _notifyIcon.Dispose();

            App.Server = null;
            _notifyIcon = null;
        }

        #region glass

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // This can't be done any earlier than the SourceInitialized event:
            ExtendGlassFrame(this, new Thickness(-1));
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();

        public static bool ExtendGlassFrame(Window window, Thickness margin)
        {
            try
            {
                if (!DwmIsCompositionEnabled())
                    return false;

                var hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero)
                    throw new InvalidOperationException("The Window must be shown before extending glass.");

                // Set the background to transparent from both the WPF and Win32 perspectives
                window.Background = Brushes.Transparent;
                HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

                var margins = new MARGINS(margin);
                DwmExtendFrameIntoClientArea(hwnd, ref margins);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        struct MARGINS
        {
            public int Bottom;
            public int Left;
            public int Right;
            public int Top;

            public MARGINS(Thickness t)
            {
                Left = (int) t.Left;
                Right = (int) t.Right;
                Top = (int) t.Top;
                Bottom = (int) t.Bottom;
            }
        }

        #endregion

        #endregion
    }
}