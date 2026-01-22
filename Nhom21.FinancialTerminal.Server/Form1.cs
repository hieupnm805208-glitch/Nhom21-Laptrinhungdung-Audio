using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Nhom21.FinancialTerminal.Server;

public partial class Form1 : Form
{
    private TcpListener _listener;
    private List<TcpClient> _clients = new List<TcpClient>();
    private List<Stock> _stocks = new List<Stock>();
    private System.Windows.Forms.Timer _simulationTimer;
    private Random _rng = new Random();
    private bool _isRunning = false;

    public Form1()
    {
        InitializeComponent();
        InitializeStocks();
        InitializeServer();
    }

    private void InitializeStocks()
    {
        string[] symbols = { "FPT", "VNM", "HPG", "VIC", "GAS", "MSN", "MBB", "TCB", "VCB", "ACB" };
        double[] initialPrices = { 105.2, 72.5, 28.3, 45.0, 98.2, 65.4, 22.1, 33.5, 90.0, 25.4 };

        for (int i = 0; i < symbols.Length; i++)
        {
            _stocks.Add(new Stock(symbols[i], initialPrices[i]));
        }
    }

    private void InitializeServer()
    {
        _simulationTimer = new System.Windows.Forms.Timer();
        _simulationTimer.Interval = 1000;
        _simulationTimer.Tick += SimulationTimer_Tick;
    }

    private async void StartServer()
    {
        try
        {
            _listener = new TcpListener(IPAddress.Any, 8888);
            _listener.Start();
            _isRunning = true;
            _simulationTimer.Start();
            Log("Server started on port 8888...");

            while (_isRunning)
            {
                var client = await _listener.AcceptTcpClientAsync();
                lock (_clients)
                {
                    _clients.Add(client);
                }
                Log($"New client connected: {client.Client.RemoteEndPoint}");
                _ = HandleClientAsync(client);
            }
        }
        catch (Exception ex)
        {
            Log($"Server error: {ex.Message}");
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            // Keep the connection open until it's closed by the client or server stops
            var buffer = new byte[1024];
            var stream = client.GetStream();
            while (_isRunning && client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // Client disconnected
            }
        }
        catch
        {
            // Client likely disconnected
        }
        finally
        {
            lock (_clients)
            {
                _clients.Remove(client);
            }
            Log($"Client disconnected.");
        }
    }

    private void SimulationTimer_Tick(object sender, EventArgs e)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var stock in _stocks)
        {
            stock.UpdatePrice(_rng);
            sb.AppendLine(stock.ToString());
        }

        Broadcast(sb.ToString());
    }

    private void Broadcast(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\nEOF\n");
        List<TcpClient> disconnectedClients = new List<TcpClient>();

        lock (_clients)
        {
            foreach (var client in _clients)
            {
                try
                {
                    if (client.Connected)
                    {
                        var stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                    }
                    else
                    {
                        disconnectedClients.Add(client);
                    }
                }
                catch
                {
                    disconnectedClients.Add(client);
                }
            }

            foreach (var client in disconnectedClients)
            {
                _clients.Remove(client);
            }
        }
    }

    private void Log(string message)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(new Action(() => Log(message)));
            return;
        }
        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        if (!_isRunning)
        {
            btnStart.Text = "Stop Server";
            StartServer();
        }
        else
        {
            _isRunning = false;
            _listener.Stop();
            _simulationTimer.Stop();
            btnStart.Text = "Start Server";
            Log("Server stopped.");
        }
    }
}
