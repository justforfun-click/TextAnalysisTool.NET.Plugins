namespace TextAnalysisTool.NET.Plugin
{
    partial class Visualizer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Visualizer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.newParserTemplateBtn = new System.Windows.Forms.ToolStripButton();
            this.loadParserBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadBtn = new System.Windows.Forms.ToolStripButton();
            this.reloadParserBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyBtn = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.plotPanel = new System.Windows.Forms.Panel();
            this.parsersListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newParserTemplateBtn,
            this.loadParserBtn,
            this.toolStripSeparator1,
            this.reloadBtn,
            this.reloadParserBtn,
            this.toolStripSeparator2,
            this.copyBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1093, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // newParserTemplateBtn
            // 
            this.newParserTemplateBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newParserTemplateBtn.Image = ((System.Drawing.Image)(resources.GetObject("newParserTemplateBtn.Image")));
            this.newParserTemplateBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newParserTemplateBtn.Name = "newParserTemplateBtn";
            this.newParserTemplateBtn.Size = new System.Drawing.Size(23, 22);
            this.newParserTemplateBtn.Text = "toolStripButton1";
            this.newParserTemplateBtn.ToolTipText = "New Parser Template...";
            this.newParserTemplateBtn.Click += new System.EventHandler(this.newParserTemplateBtn_Click);
            // 
            // loadParserBtn
            // 
            this.loadParserBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadParserBtn.Image = ((System.Drawing.Image)(resources.GetObject("loadParserBtn.Image")));
            this.loadParserBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadParserBtn.Name = "loadParserBtn";
            this.loadParserBtn.Size = new System.Drawing.Size(23, 22);
            this.loadParserBtn.Text = "toolStripButton1";
            this.loadParserBtn.ToolTipText = "Load Parser...";
            this.loadParserBtn.Click += new System.EventHandler(this.loadParserBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // reloadBtn
            // 
            this.reloadBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reloadBtn.Image = ((System.Drawing.Image)(resources.GetObject("reloadBtn.Image")));
            this.reloadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadBtn.Name = "reloadBtn";
            this.reloadBtn.Size = new System.Drawing.Size(23, 22);
            this.reloadBtn.Text = "toolStripButton1";
            this.reloadBtn.ToolTipText = "Reload (F5)";
            this.reloadBtn.Click += new System.EventHandler(this.reloadBtn_Click);
            // 
            // reloadParserBtn
            // 
            this.reloadParserBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reloadParserBtn.Image = ((System.Drawing.Image)(resources.GetObject("reloadParserBtn.Image")));
            this.reloadParserBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadParserBtn.Name = "reloadParserBtn";
            this.reloadParserBtn.Size = new System.Drawing.Size(23, 22);
            this.reloadParserBtn.Text = "toolStripButton1";
            this.reloadParserBtn.ToolTipText = "Reload Parser";
            this.reloadParserBtn.Click += new System.EventHandler(this.reloadParserBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // copyBtn
            // 
            this.copyBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyBtn.Image = ((System.Drawing.Image)(resources.GetObject("copyBtn.Image")));
            this.copyBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyBtn.Name = "copyBtn";
            this.copyBtn.Size = new System.Drawing.Size(23, 22);
            this.copyBtn.Text = "toolStripButton1";
            this.copyBtn.ToolTipText = "Copy Graph (Ctrl + C)";
            this.copyBtn.Click += new System.EventHandler(this.copyBtn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.plotPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.parsersListView);
            this.splitContainer1.Panel2MinSize = 200;
            this.splitContainer1.Size = new System.Drawing.Size(1093, 747);
            this.splitContainer1.SplitterDistance = 543;
            this.splitContainer1.TabIndex = 1;
            // 
            // plotPanel
            // 
            this.plotPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotPanel.Location = new System.Drawing.Point(0, 0);
            this.plotPanel.Name = "plotPanel";
            this.plotPanel.Size = new System.Drawing.Size(1093, 543);
            this.plotPanel.TabIndex = 0;
            // 
            // parsersListView
            // 
            this.parsersListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.parsersListView.CheckBoxes = true;
            this.parsersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.parsersListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parsersListView.FullRowSelect = true;
            this.parsersListView.HideSelection = false;
            this.parsersListView.Location = new System.Drawing.Point(0, 0);
            this.parsersListView.MinimumSize = new System.Drawing.Size(0, 200);
            this.parsersListView.Name = "parsersListView";
            this.parsersListView.Size = new System.Drawing.Size(1093, 200);
            this.parsersListView.TabIndex = 0;
            this.parsersListView.UseCompatibleStateImageBehavior = false;
            this.parsersListView.View = System.Windows.Forms.View.Details;
            this.parsersListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.parsersListView_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 22;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Parser Name";
            this.columnHeader2.Width = 200;
            // 
            // Visualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1093, 772);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Visualizer";
            this.Text = "Visualizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Visualizer_FormClosing);
            this.Load += new System.EventHandler(this.Visualizer_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Visualizer_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView parsersListView;
        private System.Windows.Forms.Panel plotPanel;
        private System.Windows.Forms.ToolStripButton newParserTemplateBtn;
        private System.Windows.Forms.ToolStripButton loadParserBtn;
        private System.Windows.Forms.ToolStripButton reloadBtn;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripButton reloadParserBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton copyBtn;
    }
}