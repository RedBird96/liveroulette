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
    public partial class MailInput : Form
    {
        public string strMailName;
        public bool CheckMailOpening;
        public MailInput(string UserEmail)
        {
            InitializeComponent();
            this.chkKeepEmail.Checked = false;
            this.txtMachineId.Text = Global.gStrHWID;
            strMailName = UserEmail;
            if (UserEmail != "")
            { 
                this.txtMailAddress.Text = UserEmail;
                this.chkKeepEmail.Checked = true;
                CheckMailOpening = true;
            }
            else
            {
                this.chkKeepEmail.Checked = false;
                CheckMailOpening = false;

            }
        }

        private void MailInput_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.btnOk.Enabled = false;
            if (Login())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            this.btnOk.Enabled = true;

        }

        private void chkKeepEmail_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ch = (CheckBox)sender;
            CheckMailOpening = ch.Checked;
        }

        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MailRegister mailReg;
            mailReg = new MailRegister();
            mailReg.SetInterface(true);

            mailReg.SetMaileName(this.txtMailAddress.Text);
            if (mailReg.ShowDialog() == DialogResult.OK)
            {
                this.txtMailAddress.Text = mailReg.mailName;
                strMailName = mailReg.mailName;
            }
        }

        private void MailInput_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private bool Login()
        {
            strMailName = this.txtMailAddress.Text;
            if (strMailName.Length == 0)
            {
                MessageBox.Show("Please input email again.", "Notification");
                return false;
            }

            if (!Global.stripe.CheckExistMail(strMailName, Global.gStrHWID))
            {
                
                return false;
            }
            return true;
        }

        private void txtMailAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.btnOk.Enabled = false;
                if (Login())
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                this.btnOk.Enabled = true;
            }
        }
    }
}
