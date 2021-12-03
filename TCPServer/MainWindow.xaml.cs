using SimpleTcp;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TCPServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleTcpServer _server;

        public MainWindow()
        {
            InitializeComponent();

            InitializeServer();
        }

        private void InitializeServer()
        {
            _server = new(txtServer.Text);
            _server.Events.ClientConnected += OnClientConnected;
            _server.Events.ClientDisconnected += OnClientDisconnected;
            _server.Events.DataReceived += OnDataReceived;
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtInfos.Text += $"{e.IpPort} connected.{Environment.NewLine}";
                lstClientIP.Items.Add(e.IpPort);
            });
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtInfos.Text += $"{e.IpPort} disconnected.{Environment.NewLine}";
                lstClientIP.Items.Remove(e.IpPort);
            });
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtInfos.Text += $"{e.IpPort}: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
            });
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _server.Start();
                txtInfos.Text += $"Server started.{Environment.NewLine}";

                btnStart.IsEnabled = false;
                EnableDisableSendButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!_server.IsListening) return;

            if (lstClientIP.SelectedItem == null) return;

            if (string.IsNullOrEmpty(txtMessage.Text)) return;

            _server.Send(lstClientIP.SelectedItem.ToString(), txtMessage.Text);
            txtInfos.Text += $"Server: {txtMessage.Text}{Environment.NewLine}";
            txtMessage.Text = string.Empty;
        }

        private void lstClientIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableDisableSendButton();
        }

        private void EnableDisableSendButton()
        {
            btnSend.IsEnabled = _server.IsListening && lstClientIP.SelectedItem != null;
        }
    }
}
