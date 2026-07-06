namespace BarcodeScannerDemo
{
    partial class BarcodeDemo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCloseCurrentTab = new System.Windows.Forms.Button();
            this.btnAddTab = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblScanStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.inventoryPageControl1 = new BarcodeScannerDemo.InventoryPageControl();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCloseCurrentTab
            // 
            this.btnCloseCurrentTab.Location = new System.Drawing.Point(704, 12);
            this.btnCloseCurrentTab.Name = "btnCloseCurrentTab";
            this.btnCloseCurrentTab.Size = new System.Drawing.Size(75, 23);
            this.btnCloseCurrentTab.TabIndex = 1;
            this.btnCloseCurrentTab.Text = "CloseCurrentTab";
            this.btnCloseCurrentTab.UseVisualStyleBackColor = true;
            this.btnCloseCurrentTab.Click += new System.EventHandler(this.btnCloseCurrentTab_Click);
            // 
            // btnAddTab
            // 
            this.btnAddTab.Location = new System.Drawing.Point(623, 12);
            this.btnAddTab.Name = "btnAddTab";
            this.btnAddTab.Size = new System.Drawing.Size(75, 23);
            this.btnAddTab.TabIndex = 0;
            this.btnAddTab.Text = "AddTab";
            this.btnAddTab.UseVisualStyleBackColor = true;
            this.btnAddTab.Click += new System.EventHandler(this.btnAddTab_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblScanStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 406);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 44);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "Scanner Ready";
            // 
            // lblScanStatus
            // 
            this.lblScanStatus.Name = "lblScanStatus";
            this.lblScanStatus.Size = new System.Drawing.Size(48, 39);
            this.lblScanStatus.Text = "Ready...";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCloseCurrentTab);
            this.panel1.Controls.Add(this.btnAddTab);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 359);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 47);
            this.panel1.TabIndex = 7;
            this.panel1.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 359);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.inventoryPageControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 333);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Barcode Scan";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // inventoryPageControl1
            // 
            this.inventoryPageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inventoryPageControl1.Location = new System.Drawing.Point(3, 3);
            this.inventoryPageControl1.Name = "inventoryPageControl1";
            this.inventoryPageControl1.ScannerEngine = null;
            this.inventoryPageControl1.Size = new System.Drawing.Size(786, 327);
            this.inventoryPageControl1.TabIndex = 0;
            // 
            // BarcodeDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "BarcodeDemo";
            this.Text = "Barcode Scanner Demo";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCloseCurrentTab;
        private System.Windows.Forms.Button btnAddTab;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblScanStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private InventoryPageControl inventoryPageControl1;
    }
}

