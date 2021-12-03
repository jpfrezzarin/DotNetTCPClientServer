using SimpleTcp;
using System;
using System.Text;
using System.Windows;

namespace TCPClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleTcpClient _client;

        public MainWindow()
        {
            InitializeComponent();

            InitializeClient();
        }

        private void InitializeClient()
        {
            _client = new(txtServer.Text);
            _client.Events.Connected += OnClientConnected;
            _client.Events.Disconnected += OnClientDisconnected;
            _client.Events.DataReceived += OnDataReceived;
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtInfos.Text += $"Server connected.{Environment.NewLine}";
            });
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtInfos.Text += $"Server disconnected.{Environment.NewLine}";
            });
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtInfos.Text += $"Server: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
            });
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _client.Connect();
                btnConnect.IsEnabled = false;
                
                EnableDisableSendButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!_client.IsConnected) return;

            if (string.IsNullOrEmpty(txtMessage.Text)) return;

            _client.Send(txtMessage.Text);
            txtInfos.Text += $"Me: {txtMessage.Text}{Environment.NewLine}";
            txtMessage.Text = string.Empty;
        }
        private void EnableDisableSendButton()
        {
            btnSend.IsEnabled = _client.IsConnected;
        }
    }
}
