
namespace LiveRoulette
{
    partial class BuyDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuyDialog));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.labRemaing = new System.Windows.Forms.Label();
            this.labUntilDate = new System.Windows.Forms.Label();
            this.labPrice = new System.Windows.Forms.Label();
            this.labCardNo = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(202, 133);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 44);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(31, 133);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(74, 44);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // labRemaing
            // 
            this.labRemaing.AutoSize = true;
            this.labRemaing.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labRemaing.Location = new System.Drawing.Point(40, 17);
            this.labRemaing.Name = "labRemaing";
            this.labRemaing.Size = new System.Drawing.Size(68, 15);
            this.labRemaing.TabIndex = 3;
            this.labRemaing.Text = "Remaining";
            // 
            // labUntilDate
            // 
            this.labUntilDate.AutoSize = true;
            this.labUntilDate.Location = new System.Drawing.Point(19, 104);
            this.labUntilDate.Name = "labUntilDate";
            this.labUntilDate.Size = new System.Drawing.Size(226, 13);
            this.labUntilDate.TabIndex = 4;
            this.labUntilDate.Text = "After payment you can use this software until -.";
            // 
            // labPrice
            // 
            this.labPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labPrice.Location = new System.Drawing.Point(54, 49);
            this.labPrice.Name = "labPrice";
            this.labPrice.Size = new System.Drawing.Size(200, 15);
            this.labPrice.TabIndex = 5;
            this.labPrice.Text = "Price";
            this.labPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labCardNo
            // 
            this.labCardNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labCardNo.Location = new System.Drawing.Point(54, 78);
            this.labCardNo.Name = "labCardNo";
            this.labCardNo.Size = new System.Drawing.Size(200, 15);
            this.labCardNo.TabIndex = 6;
            this.labCardNo.Text = "Card No";
            this.labCardNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(234, 79);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(42, 13);
            this.linkLabel1.TabIndex = 7;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Update";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // BuyDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 191);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.labCardNo);
            this.Controls.Add(this.labPrice);
            this.Controls.Add(this.labUntilDate);
            this.Controls.Add(this.labRemaing);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BuyDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buy Software";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.BuyDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Label labRemaing;
        private System.Windows.Forms.Label labUntilDate;
        private System.Windows.Forms.Label labPrice;
        private System.Windows.Forms.Label labCardNo;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}