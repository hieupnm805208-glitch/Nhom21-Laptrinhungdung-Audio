namespace Nhom21.FinancialTerminal.Client;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        panelSidebar = new Panel();
        btnConnect = new Button();
        txtPort = new TextBox();
        lblPort = new Label();
        txtIP = new TextBox();
        lblIP = new Label();
        dgvStocks = new DataGridView();
        panelSidebar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvStocks).BeginInit();
        SuspendLayout();
        // 
        // panelSidebar
        // 
        panelSidebar.Controls.Add(btnConnect);
        panelSidebar.Controls.Add(txtPort);
        panelSidebar.Controls.Add(lblPort);
        panelSidebar.Controls.Add(txtIP);
        panelSidebar.Controls.Add(lblIP);
        panelSidebar.Dock = DockStyle.Left;
        panelSidebar.Location = new Point(0, 0);
        panelSidebar.Name = "panelSidebar";
        panelSidebar.Size = new Size(200, 450);
            panelSidebar.BackColor = Color.FromArgb(45, 45, 45);
        panelSidebar.TabIndex = 0;

        // 
        // headerPanel
        // 
        headerPanel = new Panel();
        headerPanel.Location = new Point(200, 0);
        headerPanel.Size = new Size(600, 60);
        headerPanel.Paint += HeaderPanel_Paint;
        
        lblTitle = new Label();
        lblTitle.Text = "Nhom21 Financial Terminal - Khách hàng";
        lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(15, 15);
        headerPanel.Controls.Add(lblTitle);

        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        statusLabel.Text = "Đã ngắt kết nối";
        themeToggleButton = new ToolStripDropDownButton();
        themeToggleButton.Text = "Giao diện";
        themeToggleButton.DropDownItems.Add("Tối", null, (s, e) => SetTheme(true));
        themeToggleButton.DropDownItems.Add("Sáng", null, (s, e) => SetTheme(false));
        statusStrip.Items.Add(statusLabel);
        statusStrip.Items.Add(themeToggleButton);

        // 
        // btnConnect
        // 
        btnConnect.Location = new Point(12, 107);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(176, 23);
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.ForeColor = Color.White;
            btnConnect.BackColor = Color.ForestGreen;
        btnConnect.TabIndex = 4;
        btnConnect.Text = "Kết nối";
        btnConnect.UseVisualStyleBackColor = true;
        btnConnect.Click += btnConnect_Click;
        // 
        // txtPort
        // 
        txtPort.Location = new Point(12, 78);
        txtPort.Name = "txtPort";
        txtPort.Size = new Size(176, 23);
        txtPort.TabIndex = 3;
        txtPort.Text = "8888";
        // 
        // lblPort
        // 
        lblPort.AutoSize = true;
        lblPort.Location = new Point(12, 60);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(32, 15);
        lblPort.TabIndex = 2;
        lblPort.Text = "Port:";
        // 
        // txtIP
        // 
        txtIP.Location = new Point(12, 34);
        txtIP.Name = "txtIP";
        txtIP.Size = new Size(176, 23);
        txtIP.TabIndex = 1;
        txtIP.Text = "127.0.0.1";
        // 
        // lblIP
        // 
        lblIP.AutoSize = true;
        lblIP.Location = new Point(12, 16);
        lblIP.Name = "lblIP";
        lblIP.Size = new Size(55, 15);
        lblIP.TabIndex = 0;
        lblIP.Text = "Server IP:";
        // 
        // dgvStocks
        // 
        dgvStocks.AllowUserToAddRows = false;
        dgvStocks.AllowUserToDeleteRows = false;
        dgvStocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvStocks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvStocks.Dock = DockStyle.Fill;
        dgvStocks.Location = new Point(200, 0);
        dgvStocks.Name = "dgvStocks";
        dgvStocks.ReadOnly = true;
        dgvStocks.RowHeadersVisible = false;
        dgvStocks.Size = new Size(600, 450);
            dgvStocks.EnableHeadersVisualStyles = false;
            dgvStocks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60,60,60);
            dgvStocks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStocks.RowTemplate.Height = 30;
            dgvStocks.DefaultCellStyle.BackColor = Color.FromArgb(30,30,30);
            dgvStocks.DefaultCellStyle.ForeColor = Color.White;
            dgvStocks.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(40,40,40);

            trendColumn = new DataGridViewImageColumn();
            trendColumn.Name = "Trend";
            trendColumn.HeaderText = "Biểu đồ";
            trendColumn.Width = 80;
            dgvStocks.Columns.Add(trendColumn);

        dgvStocks.TabIndex = 1;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(headerPanel);
        Controls.Add(statusStrip);
        Controls.Add(dgvStocks);
        Controls.Add(panelSidebar);
        Name = "Form1";
        Text = "";
        this.FormBorderStyle = FormBorderStyle.None;
        this.DoubleBuffered = true;
        panelSidebar.ResumeLayout(false);
        panelSidebar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvStocks).EndInit();
        ResumeLayout(false);
    }

    private Panel panelSidebar;
    private Label lblIP;
    private TextBox txtIP;
    private Label lblPort;
    private TextBox txtPort;
    private Button btnConnect;
    private DataGridView dgvStocks;
        private Panel headerPanel;
        private Label lblTitle;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripDropDownButton themeToggleButton;
        private DataGridViewImageColumn trendColumn;

    #endregion
}
