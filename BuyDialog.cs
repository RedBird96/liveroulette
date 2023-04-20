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
    public partial class BuyDialog : Form
    {
        public bool existInvoice;
        public BuyDialog()
        {
            InitializeComponent();
            existInvoice = false;

        }

        public void SetLabel(ProductInfo proInfo, int remaingDay)
        {
            DateTime dtExpire = Global.stripe.GetCurrentTime().AddMonths(1);
            this.labRemaing.Text = $"{remaingDay} day(s) until expiry date";

            this.labPrice.Text = (proInfo.price / 100.0).ToString("N2") + $"{proInfo.currencyUnit} / {proInfo.intervalPeriod}";
            this.labUntilDate.Text = $"After payment you can use this software until {dtExpire.ToString("yyyy-MM-dd")}";
            this.labCardNo.Text = $"CardNo: {Global.card.cardNo}";
        }

        public void SetAccountInvoice()
        {

        }
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            string endDate = "";
            string outString = "";
            this.btnConfirm.Enabled = false;
            this.btnCancel.Enabled = false;
            if (Global.AccountMode == Global.ACCOUNT_STATUS.TRIAL_MODE)
            {
                Global.stripe.UpdateSubscription();
                if (!Global.stripe.PayInCompleteInvoice(out endDate, out outString))
                {
                    MessageBox.Show(outString, "Notification");
                    return;
                }
            }
            else
            {
                if (existInvoice)
                {
                    if (!Global.stripe.PayInCompleteInvoice(out endDate, out outString))
                    {
                        MessageBox.Show(outString, "Notification");
                        return;
                    }
                }
                else
                {
                    if (!Global.stripe.Payment((long)Global.stripe.feeProductInfo.price, out endDate, out outString))
                    {
                        MessageBox.Show(outString, "Notification");
                        return;
                    }
                }
            }
            this.btnConfirm.Enabled = true;
            this.btnCancel.Enabled = true;
            this.Hide();
            return;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
            return;
        }

        private void BuyDialog_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MailRegister mailReg;
            mailReg = new MailRegister();
            mailReg.SetInterface(false);

            mailReg.SetMaileName(Global.gAccountMail);
            if (mailReg.ShowDialog() == DialogResult.OK)
            {
                this.labCardNo.Text = $"CardNo: {Global.card.cardNo}";
            }
        }
    }
}
