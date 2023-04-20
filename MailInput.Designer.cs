
namespace LiveRoulette
{
    partial class MailInput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailInput));
            this.btnOk = new System.Windows.Forms.Button();
            this.txtMailAddress = new System.Windows.Forms.TextBox();
            this.chkKeepEmail = new System.Windows.Forms.CheckBox();
            this.txtMachineId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkRegister = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(109, 111);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(156, 37);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Log In";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtMailAddress
            // 
            this.txtMailAddress.Location = new System.Drawing.Point(85, 59);
            this.txtMailAddress.Name = "txtMailAddress";
            this.txtMailAddress.Size = new System.Drawing.Size(217, 20);
            this.txtMailAddress.TabIndex = 0;
            this.txtMailAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMailAddress_KeyPress);
            // 
            // chkKeepEmail
            // 
            this.chkKeepEmail.AutoSize = true;
            this.chkKeepEmail.Location = new System.Drawing.Point(140, 86);
            this.chkKeepEmail.Name = "chkKeepEmail";
            this.chkKeepEmail.Size = new System.Drawing.Size(94, 17);
            this.chkKeepEmail.TabIndex = 3;
            this.chkKeepEmail.Text = "Remember me";
            this.chkKeepEmail.UseVisualStyleBackColor = true;
            this.chkKeepEmail.CheckedChanged += new System.EventHandler(this.chkKeepEmail_CheckedChanged);
            // 
            // txtMachineId
            // 
            this.txtMachineId.Location = new System.Drawing.Point(85, 24);
            this.txtMachineId.Name = "txtMachineId";
            this.txtMachineId.ReadOnly = true;
            this.txtMachineId.Size = new System.Drawing.Size(217, 20);
            this.txtMachineId.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Machine ID : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "User Email : ";
            // 
            // linkRegister
            // 
            this.linkRegister.AutoSize = true;
            this.linkRegister.Location = new System.Drawing.Point(12, 134);
            this.linkRegister.Name = "linkRegister";
            this.linkRegister.Size = new System.Drawing.Size(46, 13);
            this.linkRegister.TabIndex = 7;
            this.linkRegister.TabStop = true;
            this.linkRegister.Text = "Register";
            this.linkRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkRegister_LinkClicked);
            // 
            // MailInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 159);
            this.Controls.Add(this.linkRegister);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMachineId);
            this.Controls.Add(this.chkKeepEmail);
            this.Controls.Add(this.txtMailAddress);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MailInput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Roulette";
            this.Load += new System.EventHandler(this.MailInput_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MailInput_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtMailAddress;
        private System.Windows.Forms.CheckBox chkKeepEmail;
        private System.Windows.Forms.TextBox txtMachineId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkRegister;
    }
}