//  
//  by Anthony Johnston      
//     Antix Software Limited 
//     http://antix.co.uk
// ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Antix.Mail.Smtp.Impostor.Properties;

namespace Antix.Mail.Smtp.Impostor.Client {
    /// <summary>
    ///   Interaction logic for HostTabItem.xaml
    /// </summary>
    public partial class HostTabItem : TabItem, IStatusParent {
        public HostTabItem(Func<HostConfiguration, HostTabItem> addHost) {
            InitializeComponent();

            Header = "+";
            MessagesControl.Visibility = Visibility.Collapsed;
            SettingsControl.Visibility = Visibility.Visible;

            // defaults
            Model = new HostConfiguration
                    {
                        IPAddress = IPAddress.Any,
                        Port = 0,
                        Name = string.Empty
                    };

            HostSettingsApplyButton.Content = Properties.Resources.Host_Create;
            HostSettingsApplyButton.Click += (sender, e) => {
                                                 try {
                                                     if (Validate()) {
                                                         var hostConfig = new HostConfiguration
                                                                          {
                                                                              Name = Model.Name,
                                                                              IPAddress = Model.IPAddress,
                                                                              Port = Model.Port,
                                                                              MessageStorage =
                                                                                  new FileMessageStorageConfiguration()
                                                                          };

                                                         addHost(hostConfig).IsSelected = true;

                                                         Settings.Default.Hosts.Add(hostConfig);
                                                         Settings.Default.Save();
                                                     }
                                                 }
                                                 catch (Exception ex) {
                                                     Status.Log(ex);
                                                 }
                                             };
        }

        /// <summary>
        ///   <para>Create object</para>
        /// </summary>
        /// <param name = "host">Host</param>
        public HostTabItem(Host host, HostConfiguration config) {
            InitializeComponent();

            Status = new Status(this, StatusControl);
            Host = host;
            Model = config;

            #region watch informative events

            host.Event += (sender, e) => {
                              Dispatcher.Invoke(DispatcherPriority.Normal, (Action) (() => {
                                                                                         switch (e.Type) {
                                                                                             case
                                                                                                 HostEventTypes.
                                                                                                     SessionConnected:
                                                                                             case
                                                                                                 HostEventTypes.
                                                                                                     SessionIdentified:
                                                                                                 HostStatusImage.Source
                                                                                                     =
                                                                                                     (ImageSource)
                                                                                                     FindResource(
                                                                                                         "ReceivingImage");
                                                                                                 Status.Set(
                                                                                                     States.Info,
                                                                                                     Properties.
                                                                                                         Resources.
                                                                                                         Connected_Message,
                                                                                                     e.Session);

                                                                                                 break;
                                                                                             case
                                                                                                 HostEventTypes.
                                                                                                     SessionMessageReceived
                                                                                                 :
                                                                                                 HostStatusImage.Source
                                                                                                     =
                                                                                                     (ImageSource)
                                                                                                     FindResource(
                                                                                                         "ReceivingImage");
                                                                                                 Status.Set(
                                                                                                     States.Info,
                                                                                                     Properties.
                                                                                                         Resources.
                                                                                                         MessageReceived_Message,
                                                                                                     e.Session);

                                                                                                 break;
                                                                                             case
                                                                                                 HostEventTypes.
                                                                                                     SessionDisconnected
                                                                                                 :
                                                                                                 HostStatusImage.Source
                                                                                                     =
                                                                                                     (ImageSource)
                                                                                                     FindResource(
                                                                                                         "StartedImage");

                                                                                                 break;
                                                                                             case HostEventTypes.Started
                                                                                                 :
                                                                                                 HostStatusImage.Source
                                                                                                     =
                                                                                                     (ImageSource)
                                                                                                     FindResource(
                                                                                                         "StartedImage");
                                                                                                 Status.Set(
                                                                                                     States.Info,
                                                                                                     Properties.
                                                                                                         Resources.
                                                                                                         Started_Message);

                                                                                                 break;
                                                                                             case HostEventTypes.Stopped
                                                                                                 :
                                                                                                 HostStatusImage.Source
                                                                                                     =
                                                                                                     (ImageSource)
                                                                                                     FindResource(
                                                                                                         "StoppedImage");
                                                                                                 Status.Set(
                                                                                                     States.Info,
                                                                                                     Properties.
                                                                                                         Resources.
                                                                                                         Stopped_Message);

                                                                                                 break;
                                                                                         }
                                                                                     }));
                          };

            #endregion

            #region watch message collection changes

            host.Messages.CollectionChanged += (sender, e) => {
                                                   Dispatcher.Invoke(DispatcherPriority.Normal, (Action) (() => {
                                                                                                              switch (
                                                                                                                  e.
                                                                                                                      Action
                                                                                                                  ) {
                                                                                                                      case
                                                                                                                          NotifyCollectionChangedAction
                                                                                                                              .
                                                                                                                              Add
                                                                                                                          :
                                                                                                                          MessagesControl
                                                                                                                              .
                                                                                                                              Items
                                                                                                                              .
                                                                                                                              Add
                                                                                                                              (e
                                                                                                                                   .
                                                                                                                                   NewItems
                                                                                                                                   [
                                                                                                                                       0
                                                                                                                                   ]);

                                                                                                                          break;
                                                                                                                      case
                                                                                                                          NotifyCollectionChangedAction
                                                                                                                              .
                                                                                                                              Remove
                                                                                                                          : {
                                                                                                                              var
                                                                                                                                  index
                                                                                                                                      =
                                                                                                                                      MessagesControl
                                                                                                                                          .
                                                                                                                                          Items
                                                                                                                                          .
                                                                                                                                          IndexOf
                                                                                                                                          (e
                                                                                                                                               .
                                                                                                                                               OldItems
                                                                                                                                               [
                                                                                                                                                   0
                                                                                                                                               ]);
                                                                                                                              MessagesControl
                                                                                                                                  .
                                                                                                                                  Items
                                                                                                                                  .
                                                                                                                                  Remove
                                                                                                                                  (e
                                                                                                                                       .
                                                                                                                                       OldItems
                                                                                                                                       [
                                                                                                                                           0
                                                                                                                                       ]);

                                                                                                                              if
                                                                                                                                  (
                                                                                                                                  index >=
                                                                                                                                  MessagesControl
                                                                                                                                      .
                                                                                                                                      Items
                                                                                                                                      .
                                                                                                                                      Count)
                                                                                                                                  index
                                                                                                                                      =
                                                                                                                                      MessagesControl
                                                                                                                                          .
                                                                                                                                          Items
                                                                                                                                          .
                                                                                                                                          Count -
                                                                                                                                      1;

                                                                                                                              if
                                                                                                                                  (
                                                                                                                                  index >
                                                                                                                                  -1) {
                                                                                                                                  var
                                                                                                                                      item
                                                                                                                                          =
                                                                                                                                          (
                                                                                                                                          ListViewItem
                                                                                                                                          )
                                                                                                                                          MessagesControl
                                                                                                                                              .
                                                                                                                                              ItemContainerGenerator
                                                                                                                                              .
                                                                                                                                              ContainerFromIndex
                                                                                                                                              (index);
                                                                                                                                  if
                                                                                                                                      (
                                                                                                                                      item !=
                                                                                                                                      null) {
                                                                                                                                      item
                                                                                                                                          .
                                                                                                                                          IsSelected
                                                                                                                                          =
                                                                                                                                          true;
                                                                                                                                      item
                                                                                                                                          .
                                                                                                                                          Focus
                                                                                                                                          ();
                                                                                                                                  }
                                                                                                                              }
                                                                                                                          }

                                                                                                                          break;
                                                                                                                      case
                                                                                                                          NotifyCollectionChangedAction
                                                                                                                              .
                                                                                                                              Reset
                                                                                                                          : {
                                                                                                                              MessagesControl
                                                                                                                                  .
                                                                                                                                  Items
                                                                                                                                  .
                                                                                                                                  Clear
                                                                                                                                  ();
                                                                                                                              foreach
                                                                                                                                  (
                                                                                                                                  var
                                                                                                                                      item
                                                                                                                                      in
                                                                                                                                      host
                                                                                                                                          .
                                                                                                                                          Messages
                                                                                                                                  )
                                                                                                                                  MessagesControl
                                                                                                                                      .
                                                                                                                                      Items
                                                                                                                                      .
                                                                                                                                      Add
                                                                                                                                      (item);
                                                                                                                          }

                                                                                                                          break;
                                                                                                              }

                                                                                                              HostMessageCount
                                                                                                                  .Text
                                                                                                                  =
                                                                                                                  host.
                                                                                                                      Messages
                                                                                                                      .
                                                                                                                      Count
                                                                                                                      .
                                                                                                                      ToString
                                                                                                                      ();
                                                                                                          }));
                                               };

            #endregion

            #region ui events

            var openItems = (Action) (() => {
                                          var items = MessagesControl.SelectedItems
                                              .Cast<MessageInfo>().ToArray();
                                          foreach (var info in items) {
                                              Process.Start(info.Path);
                                          }
                                      });
            var deleteItems = (Action) (() => {
                                            var items = MessagesControl.SelectedItems
                                                .Cast<MessageInfo>().ToArray();
                                            foreach (var info in items) {
                                                host.Messages.Delete(info.Id);
                                            }
                                        });
            MessagesControl.MouseDoubleClick += (sender, e) => openItems();
            MessagesControl.KeyUp += (sender, e) => {
                                         switch (e.Key) {
                                             case Key.Enter:
                                                 openItems();
                                                 break;
                                             case Key.Delete:
                                                 deleteItems();
                                                 break;
                                         }
                                     };
            HostMessageCount.ToolTip = Properties.Resources.MessageCount_Title;
            HostSettingsImage.ToolTip = Properties.Resources.Settings_Title;
            HostSettingsImage.MouseUp += (sender, e) => {
                                             SettingsControl.Visibility = SettingsControl.IsVisible
                                                                              ? Visibility.Collapsed
                                                                              : Visibility.Visible;
                                         };
            HostSettingsApplyButton.Click += (sender, e) => {
                                                 var wasStarted = host.Status == HostStates.Started;
                                                 if (wasStarted) host.Stop();

                                                 if (Validate()) {
                                                     host.Configure(Model);

                                                     MessagesControl.Focus();
                                                     Settings.Default.Save();

                                                     SettingsControl.Visibility = Visibility.Collapsed;
                                                 }

                                                 if (wasStarted) StartHost(host);
                                             };
            HostStatusImage.MouseUp += (sender, e) => {
                                           if (host.Status != HostStates.Started) StartHost(host);
                                           else host.Stop();
                                       };
            HostCloseImage.ToolTip = Properties.Resources.Close_Title;

            #endregion

            StartHost(host);
        }

        #region host

        public readonly Host Host;
        public readonly Status Status;

        private void StartHost(Host host) {
            try {
                host.Start();

                MessagesControl.Items.Clear();
                foreach (var item in host.Messages) {
                    MessagesControl.Items.Add(item);
                }

                HostMessageCount.Text = host.Messages.Count.ToString();
                HostControl.Text = host.ToString();
            }
            catch (Exception ex) {
                Status.Set(States.Error, Properties.Resources.NotStarted_Exception);
                Status.Log(ex);
            }
        }

        #endregion

        #region messages grid

        private GridViewColumnHeader _currentHeader;
        private ListSortDirection _currentHeaderDirection;

        private void MessagesControl_Click(object sender, RoutedEventArgs e) {
            var header = e.OriginalSource as GridViewColumnHeader;

            if (header != null) {
                // first show the header icon
                var direction = ListSortDirection.Ascending;

                if (header == _currentHeader) {
                    // switch direction
                    direction = _currentHeaderDirection == ListSortDirection.Ascending
                                    ? ListSortDirection.Descending
                                    : ListSortDirection.Ascending;
                }
                else if (_currentHeader != null) {
                    _currentHeader.Column.HeaderTemplate = (DataTemplate) FindResource("columnHeader");
                }
                header.Column.HeaderTemplate = (DataTemplate) FindResource(string.Format("columnHeader{0}", direction));
                _currentHeader = header;
                _currentHeaderDirection = direction;

                // do the sort on the items in the grid
                var dataView =
                    CollectionViewSource.GetDefaultView(MessagesControl.Items);
                if (dataView == null) return;

                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(
                    new SortDescription((string) header.Column.Header, direction));
                dataView.Refresh();
            }
        }

        #endregion

        #region validation

        /// <summary>
        ///   <para>Model</para>
        /// </summary>
        private HostConfiguration Model {
            get { return SettingsControl.DataContext as HostConfiguration; }
            set { SettingsControl.DataContext = value; }
        }

        /// <summary>
        ///   <para>Validate the Setting form</para>
        /// </summary>
        /// <returns>True if valid</returns>
        private bool Validate() {
            HostSettingsStatusControl.Content = string.Empty;

            var ipExp = IPAddressEditControl.GetBindingExpression(TextBox.TextProperty);
            ipExp.UpdateSource();

            var portExp = PortEditControl.GetBindingExpression(TextBox.TextProperty);
            portExp.UpdateSource();

            if (!Validation.GetHasError(IPAddressEditControl)
                && !Validation.GetHasError(PortEditControl)) {
                // check combo is not in use
                if (Host.Test(Model.IPAddress, Model.Port)) return true;

                // assume taken
                HostSettingsStatusControl.Content
                    = Properties.Resources.IPPort_Taken_Message;

                Validation.MarkInvalid(ipExp, new ValidationError(new IPAddressValidationRule(), ipExp));
                Validation.MarkInvalid(portExp, new ValidationError(new PortValidationRule(), portExp));
            }

            return false;
        }

        #endregion
    }
}