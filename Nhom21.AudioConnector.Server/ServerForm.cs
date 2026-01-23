using System.Net;
using System.Net.Sockets;

namespace Nhom21.AudioConnector.Server
{
    public partial class ServerForm : Form
    {
        private TcpListener _listener;
        private List<TcpClient> _clients = new List<TcpClient>();
        private const int Port = 8888;
        private bool _isRunning = false;

        public ServerForm()
        {
            InitializeComponent();
            this.Load += ServerForm_Load;
        }

        private void ServerForm_Load(object? sender, EventArgs e)
        {
            UpdateStatus("Server Status: Ready to start. Click 'Start Server' button.");
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_isRunning) return;
            btnStart.Enabled = false;
            btnStart.Text = "Server Running...";
            await StartServerAsync();
        }

        private async Task StartServerAsync()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, Port);
                _listener.Start();
                _isRunning = true;
                Log($"[Server] Listening on port {Port}...");
                UpdateStatus($"Running on port {Port} | Clients: 0");

                while (_isRunning)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    lock (_clients)
                    {
                        _clients.Add(client);
                    }
                    Log($"[Server] Client connected from {client.Client.RemoteEndPoint}");
                    UpdateClientCount();
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                Log($"[Error] Server error: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient sourceClient)
        {
            var buffer = new byte[8192];
            var stream = sourceClient.GetStream();

            try
            {
                while (sourceClient.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    BroadcastAudio(buffer, bytesRead, sourceClient);
                }
            }
            catch (Exception ex)
            {
                // Client likely disconnected
            }
            finally
            {
                lock (_clients)
                {
                    _clients.Remove(sourceClient);
                }
                sourceClient.Close();
                Log($"[Server] Client disconnected.");
                UpdateClientCount();
            }
        }

        private void BroadcastAudio(byte[] data, int length, TcpClient sender)
        {
            lock (_clients)
            {
                _clients.RemoveAll(c => !c.Connected);

                foreach (var client in _clients)
                {
                    if (client != sender && client.Connected)
                    {
                        try
                        {
                            var stream = client.GetStream();
                            stream.Write(data, 0, length);
                        }
                        catch
                        {
                            // Ignored
                        }
                    }
                }
            }
        }

        private void Log(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(Log), message);
                return;
            }
            txtLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}");
            txtLog.ScrollToCaret();
        }

        private void UpdateStatus(string status)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action<string>(UpdateStatus), status);
                return;
            }
            lblStatus.Text = status;
        }

        private void UpdateClientCount()
        {
            lock (_clients)
            {
                UpdateStatus($"Running on port {Port} | Clients: {_clients.Count}");
            }
        }
    }
}
