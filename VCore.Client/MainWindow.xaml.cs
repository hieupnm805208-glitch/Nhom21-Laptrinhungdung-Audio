using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using VCore.Client.Core;
using VCore.Common.Models;

namespace VCore.Client
{
    public partial class MainWindow : Window
    {
        private SocketClient _client;
        private ObservableCollection<string> _messages = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            chatMessages.ItemsSource = _messages;
            _client = new SocketClient();
            _client.OnPacketReceived += Client_OnPacketReceived;
        }

        private void Client_OnPacketReceived(Packet packet)
        {
            // Cần dùng Dispatcher vì Socket chạy ở luồng khác với luồng UI
            Dispatcher.Invoke(() =>
            {
                if (packet.Type == PacketType.Message)
                {
                    string msg = Encoding.UTF8.GetString(packet.Payload);
                    _messages.Add(msg);
                    scrollMessages.ScrollToBottom();
                }
            });
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnConnect.IsEnabled = false;
                lblStatus.Text = "Đang kết nối...";
                
                await _client.ConnectAsync(txtIp.Text, int.Parse(txtPort.Text));
                
                lblStatus.Text = "Trạng thái: Đã kết nối";
                lblStatus.Foreground = System.Windows.Media.Brushes.Green;

                // Gửi tên đăng nhập
                var loginPacket = new Packet(PacketType.Login, Encoding.UTF8.GetBytes(txtUsername.Text));
                await _client.SendPacketAsync(loginPacket);
                
                _messages.Add("--- Đã kết nối tới Server ---");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}");
                btnConnect.IsEnabled = true;
                lblStatus.Text = "Trạng thái: Lỗi!";
                lblStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private async void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await SendMessage();
            }
        }

        private async Task SendMessage()
        {
            string msg = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            try
            {
                var packet = new Packet(PacketType.Message, Encoding.UTF8.GetBytes(msg));
                await _client.SendPacketAsync(packet);
                _messages.Add($"Tôi: {msg}");
                txtMessage.Clear();
                scrollMessages.ScrollToBottom();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi gửi tin: {ex.Message}");
            }
        }
    }
}
