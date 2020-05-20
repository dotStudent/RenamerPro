namespace RenamerPro
{
    partial class frmMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.tbTvdbId = new System.Windows.Forms.TextBox();
            this.lbTvShows = new System.Windows.Forms.ListBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.gBFiles = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(661, 381);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbTvdbId
            // 
            this.tbTvdbId.Location = new System.Drawing.Point(69, 12);
            this.tbTvdbId.Name = "tbTvdbId";
            this.tbTvdbId.Size = new System.Drawing.Size(120, 22);
            this.tbTvdbId.TabIndex = 1;
            // 
            // lbTvShows
            // 
            this.lbTvShows.FormattingEnabled = true;
            this.lbTvShows.ItemHeight = 16;
            this.lbTvShows.Location = new System.Drawing.Point(69, 54);
            this.lbTvShows.Name = "lbTvShows";
            this.lbTvShows.Size = new System.Drawing.Size(222, 84);
            this.lbTvShows.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(216, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // gBFiles
            // 
            this.gBFiles.Location = new System.Drawing.Point(394, 63);
            this.gBFiles.Name = "gBFiles";
            this.gBFiles.Size = new System.Drawing.Size(377, 100);
            this.gBFiles.TabIndex = 4;
            this.gBFiles.TabStop = false;
            this.gBFiles.Text = "Drag Files here";
            this.gBFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.gBFiles_DragDrop);
            this.gBFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.gBFiles_DragEnter);
            this.gBFiles.DragLeave += new System.EventHandler(this.gBFiles_DragLeave);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gBFiles);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lbTvShows);
            this.Controls.Add(this.tbTvdbId);
            this.Controls.Add(this.button1);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbTvdbId;
        private System.Windows.Forms.ListBox lbTvShows;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox gBFiles;
    }
}

