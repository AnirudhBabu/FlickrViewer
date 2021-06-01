namespace FlickrViewer
{
    partial class FlickrViewerForm
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
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pbSelected = new System.Windows.Forms.PictureBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lsbImages = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelected)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(18, 25);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(216, 20);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Enter Flickr search tags here:";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(245, 20);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(640, 26);
            this.txtSearch.TabIndex = 1;
            // 
            // pbSelected
            // 
            this.pbSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSelected.Location = new System.Drawing.Point(242, 62);
            this.pbSelected.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbSelected.Name = "pbSelected";
            this.pbSelected.Size = new System.Drawing.Size(754, 484);
            this.pbSelected.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSelected.TabIndex = 3;
            this.pbSelected.TabStop = false;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(895, 17);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(105, 35);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // lsbImages
            // 
            this.lsbImages.FormattingEnabled = true;
            this.lsbImages.ItemHeight = 20;
            this.lsbImages.Location = new System.Drawing.Point(22, 62);
            this.lsbImages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lsbImages.Name = "lsbImages";
            this.lsbImages.Size = new System.Drawing.Size(211, 484);
            this.lsbImages.TabIndex = 5;
            this.lsbImages.SelectedIndexChanged += new System.EventHandler(this.imagesListBox_SelectedIndexChanged);
            // 
            // FlickrViewerForm
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1019, 563);
            this.Controls.Add(this.lsbImages);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.pbSelected);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FlickrViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Flickr Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.pbSelected)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.PictureBox pbSelected;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ListBox lsbImages;
    }
}

