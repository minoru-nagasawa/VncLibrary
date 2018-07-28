namespace SampleVncViewer
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.c_addressTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.c_passwordTextBox = new System.Windows.Forms.TextBox();
            this.c_vnc33CheckBox = new System.Windows.Forms.CheckBox();
            this.c_connectButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.c_vncControl = new VncUiLibrary.VncControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Address";
            // 
            // c_addressTextBox
            // 
            this.c_addressTextBox.Location = new System.Drawing.Point(65, 6);
            this.c_addressTextBox.Name = "c_addressTextBox";
            this.c_addressTextBox.Size = new System.Drawing.Size(100, 19);
            this.c_addressTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(171, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // c_passwordTextBox
            // 
            this.c_passwordTextBox.Location = new System.Drawing.Point(231, 6);
            this.c_passwordTextBox.Name = "c_passwordTextBox";
            this.c_passwordTextBox.PasswordChar = '*';
            this.c_passwordTextBox.Size = new System.Drawing.Size(100, 19);
            this.c_passwordTextBox.TabIndex = 3;
            // 
            // c_vnc33CheckBox
            // 
            this.c_vnc33CheckBox.AutoSize = true;
            this.c_vnc33CheckBox.Location = new System.Drawing.Point(337, 8);
            this.c_vnc33CheckBox.Name = "c_vnc33CheckBox";
            this.c_vnc33CheckBox.Size = new System.Drawing.Size(58, 16);
            this.c_vnc33CheckBox.TabIndex = 4;
            this.c_vnc33CheckBox.Text = "Use3.3";
            this.c_vnc33CheckBox.UseVisualStyleBackColor = true;
            // 
            // c_connectButton
            // 
            this.c_connectButton.Location = new System.Drawing.Point(401, 4);
            this.c_connectButton.Name = "c_connectButton";
            this.c_connectButton.Size = new System.Drawing.Size(75, 23);
            this.c_connectButton.TabIndex = 5;
            this.c_connectButton.Text = "Connect";
            this.c_connectButton.UseVisualStyleBackColor = true;
            this.c_connectButton.Click += new System.EventHandler(this.c_connectButton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // c_vncControl
            // 
            this.c_vncControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.c_vncControl.Location = new System.Drawing.Point(12, 31);
            this.c_vncControl.Name = "c_vncControl";
            this.c_vncControl.Size = new System.Drawing.Size(464, 350);
            this.c_vncControl.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 393);
            this.Controls.Add(this.c_vncControl);
            this.Controls.Add(this.c_connectButton);
            this.Controls.Add(this.c_vnc33CheckBox);
            this.Controls.Add(this.c_passwordTextBox);
            this.Controls.Add(this.c_addressTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox c_addressTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox c_passwordTextBox;
        private System.Windows.Forms.CheckBox c_vnc33CheckBox;
        private System.Windows.Forms.Button c_connectButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private VncUiLibrary.VncControl c_vncControl;
    }
}

