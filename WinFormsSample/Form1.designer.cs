namespace WinFormsSample
{
    partial class Form1
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
                DisposableSet.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.authorNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.authorLastNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.starPictureBox = new System.Windows.Forms.PictureBox();
            this.authorNameLabel = new System.Windows.Forms.Label();
            this.bookNameLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ratingTrackBar = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.booksComboBox = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.starsImageList = new System.Windows.Forms.ImageList(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.starPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratingTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(114, 48);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(145, 20);
            this.nameTextBox.TabIndex = 1;
            // 
            // authorNameTextBox
            // 
            this.authorNameTextBox.Location = new System.Drawing.Point(114, 74);
            this.authorNameTextBox.Name = "authorNameTextBox";
            this.authorNameTextBox.Size = new System.Drawing.Size(145, 20);
            this.authorNameTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Author Name";
            // 
            // authorLastNameTextBox
            // 
            this.authorLastNameTextBox.Location = new System.Drawing.Point(114, 100);
            this.authorLastNameTextBox.Name = "authorLastNameTextBox";
            this.authorLastNameTextBox.Size = new System.Drawing.Size(145, 20);
            this.authorLastNameTextBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Author Last Name";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.starPictureBox);
            this.panel1.Controls.Add(this.authorNameLabel);
            this.panel1.Controls.Add(this.bookNameLabel);
            this.panel1.Location = new System.Drawing.Point(12, 225);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(247, 104);
            this.panel1.TabIndex = 6;
            // 
            // starPictureBox
            // 
            this.starPictureBox.Location = new System.Drawing.Point(20, 72);
            this.starPictureBox.Name = "starPictureBox";
            this.starPictureBox.Size = new System.Drawing.Size(80, 16);
            this.starPictureBox.TabIndex = 2;
            this.starPictureBox.TabStop = false;
            // 
            // authorNameLabel
            // 
            this.authorNameLabel.AutoSize = true;
            this.authorNameLabel.Location = new System.Drawing.Point(17, 42);
            this.authorNameLabel.Name = "authorNameLabel";
            this.authorNameLabel.Size = new System.Drawing.Size(99, 13);
            this.authorNameLabel.TabIndex = 1;
            this.authorNameLabel.Text = "Some Author Name";
            // 
            // bookNameLabel
            // 
            this.bookNameLabel.AutoSize = true;
            this.bookNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bookNameLabel.Location = new System.Drawing.Point(17, 15);
            this.bookNameLabel.Name = "bookNameLabel";
            this.bookNameLabel.Size = new System.Drawing.Size(107, 13);
            this.bookNameLabel.TabIndex = 0;
            this.bookNameLabel.Text = "Some Book Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(73, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Rating";
            // 
            // ratingTrackBar
            // 
            this.ratingTrackBar.Location = new System.Drawing.Point(114, 127);
            this.ratingTrackBar.Maximum = 5;
            this.ratingTrackBar.Name = "ratingTrackBar";
            this.ratingTrackBar.Size = new System.Drawing.Size(145, 45);
            this.ratingTrackBar.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(76, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Book";
            // 
            // booksComboBox
            // 
            this.booksComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.booksComboBox.FormattingEnabled = true;
            this.booksComboBox.Location = new System.Drawing.Point(114, 21);
            this.booksComboBox.Name = "booksComboBox";
            this.booksComboBox.Size = new System.Drawing.Size(145, 21);
            this.booksComboBox.TabIndex = 10;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(103, 178);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 11;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(184, 178);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 12;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // starsImageList
            // 
            this.starsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("starsImageList.ImageStream")));
            this.starsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.starsImageList.Images.SetKeyName(0, "star0.png");
            this.starsImageList.Images.SetKeyName(1, "star1.png");
            this.starsImageList.Images.SetKeyName(2, "star2.png");
            this.starsImageList.Images.SetKeyName(3, "star3.png");
            this.starsImageList.Images.SetKeyName(4, "star4.png");
            this.starsImageList.Images.SetKeyName(5, "star5.png");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 343);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.booksComboBox);
            this.Controls.Add(this.ratingTrackBar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.authorNameTextBox);
            this.Controls.Add(this.authorLastNameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "Books";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.starPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratingTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox authorNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox authorLastNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar ratingTrackBar;
        private System.Windows.Forms.Label authorNameLabel;
        private System.Windows.Forms.Label bookNameLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox booksComboBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.PictureBox starPictureBox;
        private System.Windows.Forms.ImageList starsImageList;
    }
}

