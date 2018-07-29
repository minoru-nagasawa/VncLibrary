namespace SampleVncViewer
{
    partial class SettingDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.c_buttonRemoveEncoding = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.c_buttonDownEncoding = new System.Windows.Forms.Button();
            this.c_buttonUpEncoding = new System.Windows.Forms.Button();
            this.c_buttonAddEncoding = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.c_listBoxEncodingsUsed = new System.Windows.Forms.ListBox();
            this.c_listBoxEncodingsUnused = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.c_radioButtonColourDepth8Colour8 = new System.Windows.Forms.RadioButton();
            this.c_radioButtonColourDepth8Colour64 = new System.Windows.Forms.RadioButton();
            this.c_radioButtonColourDepth16 = new System.Windows.Forms.RadioButton();
            this.c_radioButtonColourReceived = new System.Windows.Forms.RadioButton();
            this.c_radioButtonColourDepth24 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.c_radioButtonProtocol38 = new System.Windows.Forms.RadioButton();
            this.c_radioButtonProtocol37 = new System.Windows.Forms.RadioButton();
            this.c_radioButtonProtocol33 = new System.Windows.Forms.RadioButton();
            this.c_radioButtonProtocolReceived = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.c_checkBoxSendKeyboard = new System.Windows.Forms.CheckBox();
            this.c_checkBoxSendPointer = new System.Windows.Forms.CheckBox();
            this.c_buttonCancel = new System.Windows.Forms.Button();
            this.c_buttonOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.c_buttonRemoveEncoding);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.c_buttonDownEncoding);
            this.groupBox1.Controls.Add(this.c_buttonUpEncoding);
            this.groupBox1.Controls.Add(this.c_buttonAddEncoding);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.c_listBoxEncodingsUsed);
            this.groupBox1.Controls.Add(this.c_listBoxEncodingsUnused);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 179);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Encoding";
            // 
            // c_buttonRemoveEncoding
            // 
            this.c_buttonRemoveEncoding.Location = new System.Drawing.Point(150, 100);
            this.c_buttonRemoveEncoding.Name = "c_buttonRemoveEncoding";
            this.c_buttonRemoveEncoding.Size = new System.Drawing.Size(75, 23);
            this.c_buttonRemoveEncoding.TabIndex = 1;
            this.c_buttonRemoveEncoding.Text = "<< Remove";
            this.c_buttonRemoveEncoding.UseVisualStyleBackColor = true;
            this.c_buttonRemoveEncoding.Click += new System.EventHandler(this.c_buttonRemoveEncoding_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Put the encode in order of priority";
            // 
            // c_buttonDownEncoding
            // 
            this.c_buttonDownEncoding.Location = new System.Drawing.Point(310, 150);
            this.c_buttonDownEncoding.Name = "c_buttonDownEncoding";
            this.c_buttonDownEncoding.Size = new System.Drawing.Size(59, 23);
            this.c_buttonDownEncoding.TabIndex = 1;
            this.c_buttonDownEncoding.Text = "Down";
            this.c_buttonDownEncoding.UseVisualStyleBackColor = true;
            this.c_buttonDownEncoding.Click += new System.EventHandler(this.c_buttonDownEncoding_Click);
            // 
            // c_buttonUpEncoding
            // 
            this.c_buttonUpEncoding.Location = new System.Drawing.Point(245, 150);
            this.c_buttonUpEncoding.Name = "c_buttonUpEncoding";
            this.c_buttonUpEncoding.Size = new System.Drawing.Size(59, 23);
            this.c_buttonUpEncoding.TabIndex = 1;
            this.c_buttonUpEncoding.Text = "Up";
            this.c_buttonUpEncoding.UseVisualStyleBackColor = true;
            this.c_buttonUpEncoding.Click += new System.EventHandler(this.c_buttonUpEncoding_Click);
            // 
            // c_buttonAddEncoding
            // 
            this.c_buttonAddEncoding.Location = new System.Drawing.Point(150, 71);
            this.c_buttonAddEncoding.Name = "c_buttonAddEncoding";
            this.c_buttonAddEncoding.Size = new System.Drawing.Size(75, 23);
            this.c_buttonAddEncoding.TabIndex = 1;
            this.c_buttonAddEncoding.Text = "Add >>";
            this.c_buttonAddEncoding.UseVisualStyleBackColor = true;
            this.c_buttonAddEncoding.Click += new System.EventHandler(this.c_buttonAddEncoding_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose encodes to use";
            // 
            // c_listBoxEncodingsUsed
            // 
            this.c_listBoxEncodingsUsed.FormattingEnabled = true;
            this.c_listBoxEncodingsUsed.ItemHeight = 12;
            this.c_listBoxEncodingsUsed.Location = new System.Drawing.Point(231, 34);
            this.c_listBoxEncodingsUsed.Name = "c_listBoxEncodingsUsed";
            this.c_listBoxEncodingsUsed.Size = new System.Drawing.Size(138, 112);
            this.c_listBoxEncodingsUsed.TabIndex = 0;
            // 
            // c_listBoxEncodingsUnused
            // 
            this.c_listBoxEncodingsUnused.FormattingEnabled = true;
            this.c_listBoxEncodingsUnused.ItemHeight = 12;
            this.c_listBoxEncodingsUnused.Location = new System.Drawing.Point(6, 34);
            this.c_listBoxEncodingsUnused.Name = "c_listBoxEncodingsUnused";
            this.c_listBoxEncodingsUnused.Size = new System.Drawing.Size(138, 136);
            this.c_listBoxEncodingsUnused.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.c_radioButtonColourDepth8Colour8);
            this.groupBox2.Controls.Add(this.c_radioButtonColourDepth8Colour64);
            this.groupBox2.Controls.Add(this.c_radioButtonColourDepth16);
            this.groupBox2.Controls.Add(this.c_radioButtonColourReceived);
            this.groupBox2.Controls.Add(this.c_radioButtonColourDepth24);
            this.groupBox2.Location = new System.Drawing.Point(397, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 133);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Colour level";
            // 
            // c_radioButtonColourDepth8Colour8
            // 
            this.c_radioButtonColourDepth8Colour8.AutoSize = true;
            this.c_radioButtonColourDepth8Colour8.Location = new System.Drawing.Point(5, 107);
            this.c_radioButtonColourDepth8Colour8.Name = "c_radioButtonColourDepth8Colour8";
            this.c_radioButtonColourDepth8Colour8.Size = new System.Drawing.Size(116, 16);
            this.c_radioButtonColourDepth8Colour8.TabIndex = 0;
            this.c_radioButtonColourDepth8Colour8.Text = "8 colour (Depth 8)";
            this.c_radioButtonColourDepth8Colour8.UseVisualStyleBackColor = true;
            // 
            // c_radioButtonColourDepth8Colour64
            // 
            this.c_radioButtonColourDepth8Colour64.AutoSize = true;
            this.c_radioButtonColourDepth8Colour64.Location = new System.Drawing.Point(5, 85);
            this.c_radioButtonColourDepth8Colour64.Name = "c_radioButtonColourDepth8Colour64";
            this.c_radioButtonColourDepth8Colour64.Size = new System.Drawing.Size(122, 16);
            this.c_radioButtonColourDepth8Colour64.TabIndex = 0;
            this.c_radioButtonColourDepth8Colour64.Text = "64 colour (Depth 8)";
            this.c_radioButtonColourDepth8Colour64.UseVisualStyleBackColor = true;
            // 
            // c_radioButtonColourDepth16
            // 
            this.c_radioButtonColourDepth16.AutoSize = true;
            this.c_radioButtonColourDepth16.Location = new System.Drawing.Point(5, 63);
            this.c_radioButtonColourDepth16.Name = "c_radioButtonColourDepth16";
            this.c_radioButtonColourDepth16.Size = new System.Drawing.Size(146, 16);
            this.c_radioButtonColourDepth16.TabIndex = 0;
            this.c_radioButtonColourDepth16.Text = "65535 colour (Depth 16)";
            this.c_radioButtonColourDepth16.UseVisualStyleBackColor = true;
            // 
            // c_radioButtonColourReceived
            // 
            this.c_radioButtonColourReceived.AutoSize = true;
            this.c_radioButtonColourReceived.Checked = true;
            this.c_radioButtonColourReceived.Location = new System.Drawing.Point(5, 19);
            this.c_radioButtonColourReceived.Name = "c_radioButtonColourReceived";
            this.c_radioButtonColourReceived.Size = new System.Drawing.Size(125, 16);
            this.c_radioButtonColourReceived.TabIndex = 0;
            this.c_radioButtonColourReceived.TabStop = true;
            this.c_radioButtonColourReceived.Text = "Use received colour";
            this.c_radioButtonColourReceived.UseVisualStyleBackColor = true;
            // 
            // c_radioButtonColourDepth24
            // 
            this.c_radioButtonColourDepth24.AutoSize = true;
            this.c_radioButtonColourDepth24.Location = new System.Drawing.Point(5, 41);
            this.c_radioButtonColourDepth24.Name = "c_radioButtonColourDepth24";
            this.c_radioButtonColourDepth24.Size = new System.Drawing.Size(173, 16);
            this.c_radioButtonColourDepth24.TabIndex = 0;
            this.c_radioButtonColourDepth24.Text = "16.7 million colour (Depth 24)";
            this.c_radioButtonColourDepth24.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.c_radioButtonProtocol38);
            this.groupBox3.Controls.Add(this.c_radioButtonProtocol37);
            this.groupBox3.Controls.Add(this.c_radioButtonProtocol33);
            this.groupBox3.Controls.Add(this.c_radioButtonProtocolReceived);
            this.groupBox3.Location = new System.Drawing.Point(397, 151);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(203, 111);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Protocol";
            // 
            // c_radioButtonProtocol38
            // 
            this.c_radioButtonProtocol38.AutoSize = true;
            this.c_radioButtonProtocol38.Location = new System.Drawing.Point(6, 84);
            this.c_radioButtonProtocol38.Name = "c_radioButtonProtocol38";
            this.c_radioButtonProtocol38.Size = new System.Drawing.Size(190, 16);
            this.c_radioButtonProtocol38.TabIndex = 0;
            this.c_radioButtonProtocol38.Text = "Use protocol version 3.8 if failed";
            this.c_radioButtonProtocol38.UseVisualStyleBackColor = true;
            // 
            // c_radioButtonProtocol37
            // 
            this.c_radioButtonProtocol37.AutoSize = true;
            this.c_radioButtonProtocol37.Location = new System.Drawing.Point(6, 62);
            this.c_radioButtonProtocol37.Name = "c_radioButtonProtocol37";
            this.c_radioButtonProtocol37.Size = new System.Drawing.Size(190, 16);
            this.c_radioButtonProtocol37.TabIndex = 0;
            this.c_radioButtonProtocol37.Text = "Use protocol version 3.7 if failed";
            this.c_radioButtonProtocol37.UseVisualStyleBackColor = true;
            // 
            // c_radioButtonProtocol33
            // 
            this.c_radioButtonProtocol33.AutoSize = true;
            this.c_radioButtonProtocol33.Location = new System.Drawing.Point(6, 40);
            this.c_radioButtonProtocol33.Name = "c_radioButtonProtocol33";
            this.c_radioButtonProtocol33.Size = new System.Drawing.Size(190, 16);
            this.c_radioButtonProtocol33.TabIndex = 0;
            this.c_radioButtonProtocol33.Text = "Use protocol version 3.3 if failed";
            this.c_radioButtonProtocol33.UseVisualStyleBackColor = true;
            // 
            // c_radioButtonProtocolReceived
            // 
            this.c_radioButtonProtocolReceived.AutoSize = true;
            this.c_radioButtonProtocolReceived.Checked = true;
            this.c_radioButtonProtocolReceived.Location = new System.Drawing.Point(6, 18);
            this.c_radioButtonProtocolReceived.Name = "c_radioButtonProtocolReceived";
            this.c_radioButtonProtocolReceived.Size = new System.Drawing.Size(176, 16);
            this.c_radioButtonProtocolReceived.TabIndex = 0;
            this.c_radioButtonProtocolReceived.TabStop = true;
            this.c_radioButtonProtocolReceived.Text = "Use received protocol version";
            this.c_radioButtonProtocolReceived.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.c_checkBoxSendKeyboard);
            this.groupBox4.Controls.Add(this.c_checkBoxSendPointer);
            this.groupBox4.Location = new System.Drawing.Point(12, 197);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(379, 65);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Misc";
            // 
            // c_checkBoxSendKeyboard
            // 
            this.c_checkBoxSendKeyboard.AutoSize = true;
            this.c_checkBoxSendKeyboard.Checked = true;
            this.c_checkBoxSendKeyboard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.c_checkBoxSendKeyboard.Location = new System.Drawing.Point(6, 40);
            this.c_checkBoxSendKeyboard.Name = "c_checkBoxSendKeyboard";
            this.c_checkBoxSendKeyboard.Size = new System.Drawing.Size(187, 16);
            this.c_checkBoxSendKeyboard.TabIndex = 0;
            this.c_checkBoxSendKeyboard.Text = "Send keyboard events to server";
            this.c_checkBoxSendKeyboard.UseVisualStyleBackColor = true;
            // 
            // c_checkBoxSendPointer
            // 
            this.c_checkBoxSendPointer.AutoSize = true;
            this.c_checkBoxSendPointer.Checked = true;
            this.c_checkBoxSendPointer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.c_checkBoxSendPointer.Location = new System.Drawing.Point(6, 18);
            this.c_checkBoxSendPointer.Name = "c_checkBoxSendPointer";
            this.c_checkBoxSendPointer.Size = new System.Drawing.Size(176, 16);
            this.c_checkBoxSendPointer.TabIndex = 0;
            this.c_checkBoxSendPointer.Text = "Send pointer events to server";
            this.c_checkBoxSendPointer.UseVisualStyleBackColor = true;
            // 
            // c_buttonCancel
            // 
            this.c_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.c_buttonCancel.Location = new System.Drawing.Point(524, 268);
            this.c_buttonCancel.Name = "c_buttonCancel";
            this.c_buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.c_buttonCancel.TabIndex = 1;
            this.c_buttonCancel.Text = "Cancel";
            this.c_buttonCancel.UseVisualStyleBackColor = true;
            // 
            // c_buttonOk
            // 
            this.c_buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.c_buttonOk.Location = new System.Drawing.Point(443, 268);
            this.c_buttonOk.Name = "c_buttonOk";
            this.c_buttonOk.Size = new System.Drawing.Size(75, 23);
            this.c_buttonOk.TabIndex = 1;
            this.c_buttonOk.Text = "OK";
            this.c_buttonOk.UseVisualStyleBackColor = true;
            // 
            // SettingDialog
            // 
            this.AcceptButton = this.c_buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.c_buttonCancel;
            this.ClientSize = new System.Drawing.Size(611, 297);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.c_buttonOk);
            this.Controls.Add(this.c_buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingDialog";
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button c_buttonRemoveEncoding;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button c_buttonDownEncoding;
        private System.Windows.Forms.Button c_buttonUpEncoding;
        private System.Windows.Forms.Button c_buttonAddEncoding;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.ListBox c_listBoxEncodingsUsed;
        public System.Windows.Forms.RadioButton c_radioButtonColourDepth8Colour8;
        public System.Windows.Forms.RadioButton c_radioButtonColourDepth8Colour64;
        public System.Windows.Forms.RadioButton c_radioButtonColourDepth16;
        public System.Windows.Forms.RadioButton c_radioButtonColourDepth24;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.RadioButton c_radioButtonProtocol38;
        public System.Windows.Forms.RadioButton c_radioButtonProtocol37;
        public System.Windows.Forms.RadioButton c_radioButtonProtocol33;
        public System.Windows.Forms.RadioButton c_radioButtonProtocolReceived;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.CheckBox c_checkBoxSendKeyboard;
        public System.Windows.Forms.CheckBox c_checkBoxSendPointer;
        private System.Windows.Forms.Button c_buttonCancel;
        private System.Windows.Forms.Button c_buttonOk;
        public System.Windows.Forms.ListBox c_listBoxEncodingsUnused;
        public System.Windows.Forms.RadioButton c_radioButtonColourReceived;
    }
}