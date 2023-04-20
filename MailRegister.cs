using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveRoulette
{
    public partial class MailRegister : Form
    {
        public string userName;
        public string mailName;
        public CardInfo cardData;
        public bool IsReg;
        public MailRegister()
        {
            InitializeComponent();
            this.txtHWID.Text = Global.gStrHWID;
            cardData = new CardInfo();
            IsReg = true;
        }

        public void SetMaileName(string name)
        {
            this.txtMailName.Text = name;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (this.txtMailName.Text.Length == 0 || this.txtHWID.Text.Length == 0)
            {
                MessageBox.Show("Please input email correctly.", "Notification");
                return;
            }
            if (this.txtCardNo.Text.Length == 0 
                || this.txtCVC.Text.Length == 0 || this.txtExpMonth.Text.Length == 0 || this.txtExpYear.Text.Length == 0)
            {
                MessageBox.Show("Please input card information correctly.", "Notification");
                return;
            }

            this.btnRegister.Enabled = false;
            this.btnCancel.Enabled = false;

            userName = this.txtHWID.Text;
            mailName = this.txtMailName.Text;
            cardData.cardNo = this.txtCardNo.Text;
            cardData.expYear = long.Parse(this.txtExpYear.Text);
            cardData.expMonth = long.Parse(this.txtExpMonth.Text);
            cardData.cvc = this.txtCVC.Text;
            if (IsReg)  //Register Dialog
            { 
                if (!Global.stripe.CheckCardNo(cardData))
                {
                    this.btnRegister.Enabled = true;
                    this.btnCancel.Enabled = true;
                    return;
                }

                if (!Global.stripe.RegisterMail(mailName, cardData, Global.gStrHWID))
                {
                    this.btnRegister.Enabled = true;
                    this.btnCancel.Enabled = true;
                    return;
                }
            }
            else    // Update Dialog
            {
                if (!Global.stripe.UpdateCardInfo(cardData))
                {
                    this.btnRegister.Enabled = true;
                    this.btnCancel.Enabled = true;
                    //MessageBox.Show("Card Info update failed.", "Notification");
                    return;
                }

                MessageBox.Show("Card Info has been updated successfully.", "Notification");
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtCVC_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            
            if (!char.IsDigit(ch) && ch != '\b' && ch != '\u0016' && ch != '\u0003')
                e.Handled = true;
        }

        private void txtCardNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if ((!char.IsDigit(ch) ) && ch != '\b' && ch != '\u0016' && ch != '\u0003')
                e.Handled = true;
        }

        private void txtExpYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != '\b' && ch != '\u0016' && ch != '\u0003')
                e.Handled = true;

        }

        private void txtExpMonth_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch)  && ch != '\b' && ch != '\u0016' && ch != '\u0003')
                e.Handled = true;
        }

        private void MailRegister_Load(object sender, EventArgs e)
        {
            this.label8.Text = $"You can use free for {Global.stripe.feeProductInfo.metaDataTrialDay} days of this software.";
        }

        public void SetInterface(bool isRegister)
        {
            if (isRegister) // Register dialog
            {
                this.Text = "Email Register";
                this.txtMailName.ReadOnly = false;
                this.label8.Visible = true;
                this.btnRegister.Text = "Register";
                this.label9.Text = "After the trial period expired, a monthly fee is required.";
            }
            else    // Update dialog
            {
                this.Text = "Update Card Info";
                this.txtMailName.ReadOnly = true;
                this.label8.Visible = false;
                this.btnRegister.Text = "Update";
                this.label9.Text = "Please input the Card No, CVC and Expire Date.";
            }
            IsReg = isRegister;
        }
    }
}
