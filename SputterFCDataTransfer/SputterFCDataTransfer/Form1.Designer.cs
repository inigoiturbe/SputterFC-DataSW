namespace SputterFCDataTransfer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.LOTS_list = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FirstSubModules_Combo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LastSubModules_Combo = new System.Windows.Forms.ComboBox();
            this.Output_ListBox = new System.Windows.Forms.ListBox();
            this.TransferProgress = new System.Windows.Forms.ProgressBar();
            this.Transfer_button = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Delete_Button = new System.Windows.Forms.Button();
            this.FirstLabel_Combo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.FirstLabel_Num = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.LastLabel_Combo = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.LastLabel_Num = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FirstLabel_Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LastLabel_Num)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "LOT Numbers";
            // 
            // LOTS_list
            // 
            this.LOTS_list.FormattingEnabled = true;
            this.LOTS_list.ItemHeight = 16;
            this.LOTS_list.Location = new System.Drawing.Point(15, 36);
            this.LOTS_list.Name = "LOTS_list";
            this.LOTS_list.Size = new System.Drawing.Size(120, 532);
            this.LOTS_list.TabIndex = 2;
            this.LOTS_list.SelectedIndexChanged += new System.EventHandler(this.LOTS_list_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "First Label";
            // 
            // FirstSubModules_Combo
            // 
            this.FirstSubModules_Combo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.FirstSubModules_Combo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.FirstSubModules_Combo.FormattingEnabled = true;
            this.FirstSubModules_Combo.Location = new System.Drawing.Point(141, 39);
            this.FirstSubModules_Combo.Name = "FirstSubModules_Combo";
            this.FirstSubModules_Combo.Size = new System.Drawing.Size(291, 24);
            this.FirstSubModules_Combo.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(450, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Last label";
            // 
            // LastSubModules_Combo
            // 
            this.LastSubModules_Combo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.LastSubModules_Combo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.LastSubModules_Combo.FormattingEnabled = true;
            this.LastSubModules_Combo.Location = new System.Drawing.Point(453, 39);
            this.LastSubModules_Combo.Name = "LastSubModules_Combo";
            this.LastSubModules_Combo.Size = new System.Drawing.Size(291, 24);
            this.LastSubModules_Combo.TabIndex = 6;
            // 
            // Output_ListBox
            // 
            this.Output_ListBox.FormattingEnabled = true;
            this.Output_ListBox.ItemHeight = 16;
            this.Output_ListBox.Location = new System.Drawing.Point(141, 148);
            this.Output_ListBox.Name = "Output_ListBox";
            this.Output_ListBox.Size = new System.Drawing.Size(603, 420);
            this.Output_ListBox.TabIndex = 14;
            // 
            // TransferProgress
            // 
            this.TransferProgress.Location = new System.Drawing.Point(141, 97);
            this.TransferProgress.Name = "TransferProgress";
            this.TransferProgress.Size = new System.Drawing.Size(490, 34);
            this.TransferProgress.TabIndex = 13;
            this.TransferProgress.Click += new System.EventHandler(this.TransferProgress_Click);
            // 
            // Transfer_button
            // 
            this.Transfer_button.Location = new System.Drawing.Point(637, 97);
            this.Transfer_button.Name = "Transfer_button";
            this.Transfer_button.Size = new System.Drawing.Size(107, 34);
            this.Transfer_button.TabIndex = 12;
            this.Transfer_button.Text = "Transfer";
            this.Transfer_button.UseVisualStyleBackColor = true;
            this.Transfer_button.Click += new System.EventHandler(this.Transfer_button_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Delete_Button
            // 
            this.Delete_Button.Location = new System.Drawing.Point(15, 574);
            this.Delete_Button.Name = "Delete_Button";
            this.Delete_Button.Size = new System.Drawing.Size(107, 34);
            this.Delete_Button.TabIndex = 16;
            this.Delete_Button.Text = "Delete";
            this.Delete_Button.UseVisualStyleBackColor = true;
            this.Delete_Button.Click += new System.EventHandler(this.Delete_Button_Click);
            // 
            // FirstLabel_Combo
            // 
            this.FirstLabel_Combo.FormattingEnabled = true;
            this.FirstLabel_Combo.Items.AddRange(new object[] {
            "Label",
            "Position"});
            this.FirstLabel_Combo.Location = new System.Drawing.Point(218, 9);
            this.FirstLabel_Combo.Name = "FirstLabel_Combo";
            this.FirstLabel_Combo.Size = new System.Drawing.Size(214, 24);
            this.FirstLabel_Combo.TabIndex = 17;
            this.FirstLabel_Combo.SelectedIndexChanged += new System.EventHandler(this.FirstLabel_Combo_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(138, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Position";
            // 
            // FirstLabel_Num
            // 
            this.FirstLabel_Num.DecimalPlaces = 1;
            this.FirstLabel_Num.Location = new System.Drawing.Point(202, 68);
            this.FirstLabel_Num.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.FirstLabel_Num.Name = "FirstLabel_Num";
            this.FirstLabel_Num.Size = new System.Drawing.Size(120, 22);
            this.FirstLabel_Num.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(328, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 17);
            this.label5.TabIndex = 20;
            this.label5.Text = "m";
            // 
            // LastLabel_Combo
            // 
            this.LastLabel_Combo.FormattingEnabled = true;
            this.LastLabel_Combo.Items.AddRange(new object[] {
            "Label",
            "Position"});
            this.LastLabel_Combo.Location = new System.Drawing.Point(530, 10);
            this.LastLabel_Combo.Name = "LastLabel_Combo";
            this.LastLabel_Combo.Size = new System.Drawing.Size(214, 24);
            this.LastLabel_Combo.TabIndex = 21;
            this.LastLabel_Combo.SelectedIndexChanged += new System.EventHandler(this.LastLabel_Combo_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(640, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 17);
            this.label6.TabIndex = 24;
            this.label6.Text = "m";
            // 
            // LastLabel_Num
            // 
            this.LastLabel_Num.DecimalPlaces = 1;
            this.LastLabel_Num.Location = new System.Drawing.Point(514, 68);
            this.LastLabel_Num.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.LastLabel_Num.Name = "LastLabel_Num";
            this.LastLabel_Num.Size = new System.Drawing.Size(120, 22);
            this.LastLabel_Num.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(450, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 17);
            this.label7.TabIndex = 22;
            this.label7.Text = "Position";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 616);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.LastLabel_Num);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.LastLabel_Combo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.FirstLabel_Num);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FirstLabel_Combo);
            this.Controls.Add(this.Delete_Button);
            this.Controls.Add(this.Output_ListBox);
            this.Controls.Add(this.TransferProgress);
            this.Controls.Add(this.Transfer_button);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LastSubModules_Combo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FirstSubModules_Combo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LOTS_list);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "FC Sputter Data Transfer";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FirstLabel_Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LastLabel_Num)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox LOTS_list;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox FirstSubModules_Combo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox LastSubModules_Combo;
        private System.Windows.Forms.ListBox Output_ListBox;
        private System.Windows.Forms.ProgressBar TransferProgress;
        private System.Windows.Forms.Button Transfer_button;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button Delete_Button;
        private System.Windows.Forms.ComboBox FirstLabel_Combo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown FirstLabel_Num;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox LastLabel_Combo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown LastLabel_Num;
        private System.Windows.Forms.Label label7;
    }
}

