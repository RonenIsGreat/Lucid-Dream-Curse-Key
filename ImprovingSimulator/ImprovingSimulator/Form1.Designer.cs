namespace ImprovingSimulator
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
            this.BeamBusFasTasCheckBox = new System.Windows.Forms.CheckBox();
            this.StaveBusFasTasCheckBox = new System.Windows.Forms.CheckBox();
            this.StaveBusCasCheckBox = new System.Windows.Forms.CheckBox();
            this.PrsStaveBusCheckBox = new System.Windows.Forms.CheckBox();
            this.IdrsCheckBox = new System.Windows.Forms.CheckBox();
            this.BeamBusCasCheckBox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SendByNumber = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NumberOfMessagesLabel = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.SequenceSendingRadioBtn = new System.Windows.Forms.RadioButton();
            this.QuantitySendingRadioBtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.HeadingLabel = new System.Windows.Forms.Label();
            this.BannerLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BeamBusFasTasCheckBox
            // 
            this.BeamBusFasTasCheckBox.AutoSize = true;
            this.BeamBusFasTasCheckBox.Location = new System.Drawing.Point(420, 177);
            this.BeamBusFasTasCheckBox.Name = "BeamBusFasTasCheckBox";
            this.BeamBusFasTasCheckBox.Size = new System.Drawing.Size(170, 24);
            this.BeamBusFasTasCheckBox.TabIndex = 0;
            this.BeamBusFasTasCheckBox.Text = "Beam Bus Fas/Tas";
            this.BeamBusFasTasCheckBox.UseVisualStyleBackColor = true;
            // 
            // StaveBusFasTasCheckBox
            // 
            this.StaveBusFasTasCheckBox.AutoSize = true;
            this.StaveBusFasTasCheckBox.Location = new System.Drawing.Point(420, 480);
            this.StaveBusFasTasCheckBox.Name = "StaveBusFasTasCheckBox";
            this.StaveBusFasTasCheckBox.Size = new System.Drawing.Size(173, 24);
            this.StaveBusFasTasCheckBox.TabIndex = 1;
            this.StaveBusFasTasCheckBox.Text = "Stave Bus Fas/Tas ";
            this.StaveBusFasTasCheckBox.UseVisualStyleBackColor = true;
            // 
            // StaveBusCasCheckBox
            // 
            this.StaveBusCasCheckBox.AutoSize = true;
            this.StaveBusCasCheckBox.Location = new System.Drawing.Point(420, 417);
            this.StaveBusCasCheckBox.Name = "StaveBusCasCheckBox";
            this.StaveBusCasCheckBox.Size = new System.Drawing.Size(140, 24);
            this.StaveBusCasCheckBox.TabIndex = 2;
            this.StaveBusCasCheckBox.Text = "Stave Bus Cas";
            this.StaveBusCasCheckBox.UseVisualStyleBackColor = true;
            // 
            // PrsStaveBusCheckBox
            // 
            this.PrsStaveBusCheckBox.AutoSize = true;
            this.PrsStaveBusCheckBox.Location = new System.Drawing.Point(420, 363);
            this.PrsStaveBusCheckBox.Name = "PrsStaveBusCheckBox";
            this.PrsStaveBusCheckBox.Size = new System.Drawing.Size(135, 24);
            this.PrsStaveBusCheckBox.TabIndex = 3;
            this.PrsStaveBusCheckBox.Text = "Prs Stave Bus";
            this.PrsStaveBusCheckBox.UseVisualStyleBackColor = true;
            // 
            // IdrsCheckBox
            // 
            this.IdrsCheckBox.AutoSize = true;
            this.IdrsCheckBox.Location = new System.Drawing.Point(420, 293);
            this.IdrsCheckBox.Name = "IdrsCheckBox";
            this.IdrsCheckBox.Size = new System.Drawing.Size(75, 24);
            this.IdrsCheckBox.TabIndex = 4;
            this.IdrsCheckBox.Text = "IDRS";
            this.IdrsCheckBox.UseVisualStyleBackColor = true;
            // 
            // BeamBusCasCheckBox
            // 
            this.BeamBusCasCheckBox.AutoSize = true;
            this.BeamBusCasCheckBox.Location = new System.Drawing.Point(420, 231);
            this.BeamBusCasCheckBox.Name = "BeamBusCasCheckBox";
            this.BeamBusCasCheckBox.Size = new System.Drawing.Size(141, 24);
            this.BeamBusCasCheckBox.TabIndex = 5;
            this.BeamBusCasCheckBox.Text = "Beam Bus Cas";
            this.BeamBusCasCheckBox.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(937, 470);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 42);
            this.button1.TabIndex = 6;
            this.button1.Text = "Start Sending";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SendByNumber
            // 
            this.SendByNumber.Location = new System.Drawing.Point(937, 330);
            this.SendByNumber.Name = "SendByNumber";
            this.SendByNumber.Size = new System.Drawing.Size(167, 42);
            this.SendByNumber.TabIndex = 8;
            this.SendByNumber.Text = "Send By Number";
            this.SendByNumber.UseVisualStyleBackColor = true;
            this.SendByNumber.Click += new System.EventHandler(this.SendByNumber_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(933, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = "Number Of Messages :";
            // 
            // NumberOfMessagesLabel
            // 
            this.NumberOfMessagesLabel.AutoSize = true;
            this.NumberOfMessagesLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.NumberOfMessagesLabel.Location = new System.Drawing.Point(862, 97);
            this.NumberOfMessagesLabel.Name = "NumberOfMessagesLabel";
            this.NumberOfMessagesLabel.Size = new System.Drawing.Size(0, 20);
            this.NumberOfMessagesLabel.TabIndex = 10;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(937, 276);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(167, 26);
            this.numericUpDown1.TabIndex = 11;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SequenceSendingRadioBtn
            // 
            this.SequenceSendingRadioBtn.AutoSize = true;
            this.SequenceSendingRadioBtn.Location = new System.Drawing.Point(6, 37);
            this.SequenceSendingRadioBtn.Name = "SequenceSendingRadioBtn";
            this.SequenceSendingRadioBtn.Size = new System.Drawing.Size(170, 24);
            this.SequenceSendingRadioBtn.TabIndex = 12;
            this.SequenceSendingRadioBtn.TabStop = true;
            this.SequenceSendingRadioBtn.Text = "Sequence Sending";
            this.SequenceSendingRadioBtn.UseVisualStyleBackColor = true;
            this.SequenceSendingRadioBtn.CheckedChanged += new System.EventHandler(this.SequenceSendingRadioBtn_CheckedChanged);
            // 
            // QuantitySendingRadioBtn
            // 
            this.QuantitySendingRadioBtn.AutoSize = true;
            this.QuantitySendingRadioBtn.Location = new System.Drawing.Point(6, 91);
            this.QuantitySendingRadioBtn.Name = "QuantitySendingRadioBtn";
            this.QuantitySendingRadioBtn.Size = new System.Drawing.Size(156, 24);
            this.QuantitySendingRadioBtn.TabIndex = 13;
            this.QuantitySendingRadioBtn.TabStop = true;
            this.QuantitySendingRadioBtn.Text = "Quantity Sending";
            this.QuantitySendingRadioBtn.UseVisualStyleBackColor = true;
            this.QuantitySendingRadioBtn.CheckedChanged += new System.EventHandler(this.QuantitySendingRadioBtn_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SequenceSendingRadioBtn);
            this.groupBox1.Controls.Add(this.QuantitySendingRadioBtn);
            this.groupBox1.Location = new System.Drawing.Point(49, 140);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 162);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // HeadingLabel
            // 
            this.HeadingLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(112)))), ((int)(((byte)(144)))));
            this.HeadingLabel.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeadingLabel.ForeColor = System.Drawing.Color.White;
            this.HeadingLabel.Location = new System.Drawing.Point(-1, 0);
            this.HeadingLabel.Margin = new System.Windows.Forms.Padding(0);
            this.HeadingLabel.Name = "HeadingLabel";
            this.HeadingLabel.Size = new System.Drawing.Size(1214, 96);
            this.HeadingLabel.TabIndex = 15;
            this.HeadingLabel.Text = "Lucid Dream Simulator";
            this.HeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BannerLabel
            // 
            this.BannerLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(97)))), ((int)(((byte)(133)))));
            this.BannerLabel.Location = new System.Drawing.Point(-1, 96);
            this.BannerLabel.Name = "BannerLabel";
            this.BannerLabel.Size = new System.Drawing.Size(1214, 24);
            this.BannerLabel.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(112)))), ((int)(((byte)(144)))));
            this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
            this.label2.Location = new System.Drawing.Point(-1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 96);
            this.label2.TabIndex = 17;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1214, 644);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BannerLabel);
            this.Controls.Add(this.HeadingLabel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.NumberOfMessagesLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SendByNumber);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BeamBusCasCheckBox);
            this.Controls.Add(this.IdrsCheckBox);
            this.Controls.Add(this.PrsStaveBusCheckBox);
            this.Controls.Add(this.StaveBusCasCheckBox);
            this.Controls.Add(this.StaveBusFasTasCheckBox);
            this.Controls.Add(this.BeamBusFasTasCheckBox);
            this.Name = "Form1";
            this.Text = "Improved Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox BeamBusFasTasCheckBox;
        private System.Windows.Forms.CheckBox StaveBusFasTasCheckBox;
        private System.Windows.Forms.CheckBox StaveBusCasCheckBox;
        private System.Windows.Forms.CheckBox PrsStaveBusCheckBox;
        private System.Windows.Forms.CheckBox IdrsCheckBox;
        private System.Windows.Forms.CheckBox BeamBusCasCheckBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button SendByNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label NumberOfMessagesLabel;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.RadioButton SequenceSendingRadioBtn;
        private System.Windows.Forms.RadioButton QuantitySendingRadioBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label HeadingLabel;
        private System.Windows.Forms.Label BannerLabel;
        private System.Windows.Forms.Label label2;
    }
}

