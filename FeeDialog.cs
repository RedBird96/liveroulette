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
    public partial class FeeDialog : Form
    {
        public FeeDialog()
        {
            InitializeComponent();
        }

        public void SetPrice(ProductInfo pro)
        {
            DateTime dtExpire = Global.stripe.GetCurrentTime().AddMonths(1);
            this.labFeePrice.Text = (pro.price / 100.0).ToString("N2") + $"{pro.currencyUnit} / {pro.intervalPeriod}";
            this.labFeeSuffix.Text = $"{dtExpire.ToString("yyyy-MM-dd")}";
            this.labCardNo.Text = Global.card.cardNo;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string endDate, outString;
            this.btnConfirm.Enabled = false;
            this.btnCancel.Enabled = false;
            if (!Global.stripe.PayInCompleteInvoice(out endDate, out outString))
            {
                MessageBox.Show(outString, "Notification");
            }
            DialogResult = DialogResult.OK;
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

        private void FeeDialog_Load(object sender, EventArgs e)
        {

        }

        private void linkEditInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            MailRegister mailReg;
            mailReg = new MailRegister();
            mailReg.SetInterface(false);

            mailReg.SetMaileName(Global.gAccountMail);
            if (mailReg.ShowDialog() == DialogResult.OK)
            {
                this.labCardNo.Text = Global.card.cardNo;
            }

        }
    }
}
