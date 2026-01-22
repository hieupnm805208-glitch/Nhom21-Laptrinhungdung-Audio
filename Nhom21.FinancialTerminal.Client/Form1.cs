using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Nhom21.FinancialTerminal.Client;

public partial class Form1 : Form
{
    private TcpClient? _client;
    private NetworkStream _stream;
    private bool _isConnected = false;
    private StringBuilder _messageBuffer = new StringBuilder();

    public Form1()
    {
        InitializeComponent();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        dgvStocks.Columns.Add("Symbol", "Symbol");
        dgvStocks.Columns.Add("Price", "Price");
        dgvStocks.Columns.Add("Change", "Change (%)");
        
        // Custom styling for performance and readability
        dgvStocks.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        if (!_isConnected)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(txtIP.Text, int.Parse(txtPort.Text));
                _stream = _client.GetStream();
                _isConnected = true;
                btnConnect.Text = "Disconnect";
                btnConnect.BackColor = Color.LightCoral;
                
                _ = ReceiveDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            Disconnect();
        }
    }

    private void Disconnect()
    {
        _isConnected = false;
        _client?.Close();
        btnConnect.Text = "Connect";
        btnConnect.BackColor = SystemColors.Control;
    }

    private async Task ReceiveDataAsync()
    {
        byte[] buffer = new byte[4096];
        try
        {
            while (_isConnected && _client.Connected)
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                _messageBuffer.Append(data);

                string content = _messageBuffer.ToString();
                if (content.Contains("\nEOF\n"))
                {
                    string[] packets = content.Split(new[] { "\nEOF\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var packet in packets)
                    {
                        UpdateUI(packet);
                    }
                    _messageBuffer.Clear();
                }
            }
        }
        catch
        {
            // Handle disconnection
        }
        finally
        {
            if (_isConnected)
            {
                this.Invoke(new Action(() => Disconnect()));
                MessageBox.Show("Lost connection to server.", "Connection Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private void UpdateUI(string data)
    {
        if (dgvStocks.InvokeRequired)
        {
            dgvStocks.Invoke(new Action(() => UpdateUI(data)));
            return;
        }

        string[] lines = data.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            string[] parts = line.Split('|');
            if (parts.Length == 3)
            {
                string symbol = parts[0];
                string price = parts[1];
                string change = parts[2];

                bool found = false;
                foreach (DataGridViewRow row in dgvStocks.Rows)
                {
                    if (row.Cells["Symbol"].Value?.ToString() == symbol)
                    {
                        UpdateRow(row, price, change);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    int index = dgvStocks.Rows.Add(symbol, price, change);
                    UpdateRow(dgvStocks.Rows[index], price, change);
                }
            }
        }
    }

    private void UpdateRow(DataGridViewRow row, string price, string change)
    {
        row.Cells["Price"].Value = price;
        row.Cells["Change"].Value = change;

        if (change.StartsWith("-"))
        {
            row.DefaultCellStyle.ForeColor = Color.Red;
            row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 230, 230);
        }
        else
        {
            row.DefaultCellStyle.ForeColor = Color.Green;
            row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 255, 230);
        }
    }

    // Dragging logic
    [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
    private extern static void ReleaseCapture();

    [DllImport("user32.dll", EntryPoint = "SendMessage")]
    private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

    private void HeaderPanel_Paint(object sender, PaintEventArgs e)
    {
        // Gradient background
        using (LinearGradientBrush brush = new LinearGradientBrush(headerPanel.ClientRectangle, 
            Color.FromArgb(44, 62, 80), Color.FromArgb(76, 161, 175), LinearGradientMode.Horizontal))
        {
            e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
        }
        
        // Add drag event
        headerPanel.MouseDown += (s, ev) => 
        {
            if (ev.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        };
    }

    private void SetTheme(bool isDark)
    {
        if (isDark)
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            dgvStocks.BackgroundColor = Color.FromArgb(30, 30, 30);
            dgvStocks.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgvStocks.DefaultCellStyle.ForeColor = Color.White;
            dgvStocks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
            dgvStocks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            panelSidebar.BackColor = Color.FromArgb(45, 45, 45);
            lblTitle.ForeColor = Color.White;
        }
        else
        {
            this.BackColor = Color.WhiteSmoke;
            dgvStocks.BackgroundColor = Color.White;
            dgvStocks.DefaultCellStyle.BackColor = Color.White;
            dgvStocks.DefaultCellStyle.ForeColor = Color.Black;
            dgvStocks.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvStocks.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            panelSidebar.BackColor = Color.White;
            lblTitle.ForeColor = Color.Black;
        }
    }
}
