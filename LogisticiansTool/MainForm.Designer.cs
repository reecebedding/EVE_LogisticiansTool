namespace LogisticiansTool
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageAPIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetSavedRoutesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetSavedAPIKeysMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearCacheMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.errorLogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewChangeLogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(655, 526);
            this.tabControl.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(655, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageAPIToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // manageAPIToolStripMenuItem
            // 
            this.manageAPIToolStripMenuItem.Image = global::LogisticiansTool.Properties.Resources.api;
            this.manageAPIToolStripMenuItem.Name = "manageAPIToolStripMenuItem";
            this.manageAPIToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.manageAPIToolStripMenuItem.Text = "Manage API";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::LogisticiansTool.Properties.Resources.exit;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetSavedRoutesMenuItem,
            this.resetSavedAPIKeysMenuItem,
            this.clearCacheMenuItem,
            this.toolStripSeparator1,
            this.toolStripSeparator2,
            this.errorLogMenuItem,
            this.viewChangeLogMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // resetSavedRoutesMenuItem
            // 
            this.resetSavedRoutesMenuItem.Image = global::LogisticiansTool.Properties.Resources.journey;
            this.resetSavedRoutesMenuItem.Name = "resetSavedRoutesMenuItem";
            this.resetSavedRoutesMenuItem.Size = new System.Drawing.Size(199, 22);
            this.resetSavedRoutesMenuItem.Text = "Reset Saved Routes";
            // 
            // resetSavedAPIKeysMenuItem
            // 
            this.resetSavedAPIKeysMenuItem.Image = global::LogisticiansTool.Properties.Resources.api;
            this.resetSavedAPIKeysMenuItem.Name = "resetSavedAPIKeysMenuItem";
            this.resetSavedAPIKeysMenuItem.Size = new System.Drawing.Size(199, 22);
            this.resetSavedAPIKeysMenuItem.Text = "Reset Saved API Keys";
            // 
            // clearCacheMenuItem
            // 
            this.clearCacheMenuItem.Image = global::LogisticiansTool.Properties.Resources.cache;
            this.clearCacheMenuItem.Name = "clearCacheMenuItem";
            this.clearCacheMenuItem.Size = new System.Drawing.Size(199, 22);
            this.clearCacheMenuItem.Text = "Clear Cache";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(196, 6);
            // 
            // errorLogMenuItem
            // 
            this.errorLogMenuItem.Image = global::LogisticiansTool.Properties.Resources.errorLog;
            this.errorLogMenuItem.Name = "errorLogMenuItem";
            this.errorLogMenuItem.Size = new System.Drawing.Size(199, 22);
            this.errorLogMenuItem.Text = "View System Log";
            // 
            // viewChangeLogMenuItem
            // 
            this.viewChangeLogMenuItem.Image = global::LogisticiansTool.Properties.Resources.ChangeLog;
            this.viewChangeLogMenuItem.Name = "viewChangeLogMenuItem";
            this.viewChangeLogMenuItem.Size = new System.Drawing.Size(199, 22);
            this.viewChangeLogMenuItem.Text = "View Change Log";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::LogisticiansTool.Properties.Resources.QuestionMark;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.aboutToolStripMenuItem.Text = "About Logisticians Tool";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(655, 550);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Logisticians Tool";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageAPIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorLogMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearCacheMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem viewChangeLogMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetSavedRoutesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetSavedAPIKeysMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

    }
}

