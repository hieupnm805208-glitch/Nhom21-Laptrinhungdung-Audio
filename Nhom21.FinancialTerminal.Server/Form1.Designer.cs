namespace Nhom21.FinancialTerminal.Server;

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
        txtLog = new TextBox();
        btnStart = new Button();
        SuspendLayout();
        // 
        // txtLog
        // 
        txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtLog.Location = new Point(12, 41);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(560, 308);
        txtLog.TabIndex = 0;
        // 
        // btnStart
        // 
        btnStart.Location = new Point(12, 12);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(110, 23);
        btnStart.TabIndex = 1;
        btnStart.Text = "Start Server";
        btnStart.UseVisualStyleBackColor = true;
        btnStart.Click += btnStart_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(584, 361);
        Controls.Add(btnStart);
        Controls.Add(txtLog);
        Name = "Form1";
        Text = "Nhom21 Financial Terminal - Server";
        ResumeLayout(false);
        PerformLayout();
    }

    private TextBox txtLog;
    private Button btnStart;

    #endregion
}
