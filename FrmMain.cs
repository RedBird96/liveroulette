using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Automation;

namespace LiveRoulette
{
    public partial class FrmMain : Form
    {
        private string siteURL = "";
        private bool bIsOpenPage = true;
        private int nRemovedTablePreCnt;
        ListView[] listView_StatsArr;
        GroupBox[] groupBox_StatsArr;
        MailInput mailPrev;
        FeeDialog feeDlg;
        
        private string EmailOpening;
        private double RemainDays;
        private bool ExistFeeInvoice;
        private bool BuyDialogOpen;
        private bool TrialStoppedMsg;
        public int SHOWBUY_LEFTDAY = 2;
        public FrmMain()
        {
            InitializeComponent();

        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            InitTableGroup();
            LoadSettingsFromConfig();
            Global.gStrHWID = HwIdGenerator.GetHardwareID();
            if (Global.gStrHWID == "Error")
            {
                this.Close();
                return;
            }

            Global.stripe = new StripeAPI();

            mailPrev = new MailInput(EmailOpening);
            
            feeDlg = new FeeDialog();
            var dlgRes = mailPrev.ShowDialog();
            if (dlgRes == DialogResult.Cancel)
            {
                this.Close();
                return;
            }

            Global.gAccountMail = mailPrev.strMailName;
            if (mailPrev.CheckMailOpening && Global.gAccountMail.Length != 0)
            {
                EmailOpening = Global.gAccountMail;
            }
            else
            {
                EmailOpening = "";
            }

            Global.AccountMode = Global.ACCOUNT_STATUS.NONE;
            GetRemainDay();
            Thread th = new Thread(GetRemainDayThread);
            th.Start();
            th.IsBackground = true;

            tmUpdateCheck.Start();
            tmDailyTargetMsg.Start();
            tmStripeStatus.Start();
        }

        private void InitTableGroup()
        {
            listView_StatsArr = new ListView[4];
            listView_StatsArr[0] = this.listView1_Stats;
            listView_StatsArr[1] = this.listView2_Stats;
            listView_StatsArr[2] = this.listView3_Stats;
            listView_StatsArr[3] = this.listView4_Stats;

            groupBox_StatsArr = new GroupBox[4];
            groupBox_StatsArr[0] = this.groupTable1;
            groupBox_StatsArr[1] = this.groupTable2;
            groupBox_StatsArr[2] = this.groupTable3;
            groupBox_StatsArr[3] = this.groupTable4;

            this.comboURL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            Global.log = new Log(this);
            Global.tbMonitor = new TablesMonitor();
            Global.gAutoBetStatus = Global.AUTOBET_STATUS.AUTOBET_STOPPED;
            Global.gbOperationMode = Global.LIVESCENARIO_MODE.LIVE_MODE;
            Global.gbIsSimulationMode = chkUseSimulation.Checked;
            siteURL = this.comboURL.Text;
            Global.gSiteName = Global.gSiteNameArray[0];
            ExistFeeInvoice = false;
            BuyDialogOpen = false;
            TrialStoppedMsg = false;
            RemainDays = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Global.AccountMode == Global.ACCOUNT_STATUS.NONE)
                return;

            if (ExistFeeInvoice)
            {
                MessageBox.Show("Payment has been expired.\nPlease process a monthly payment to use software continuously.", "Notification");
                return;
            }

            closeOpendCasinoSite();
            if (bIsOpenPage)
            {
                Global.gSiteName = Global.gSiteNameArray[this.comboURL.SelectedIndex];
                new Thread(OpenPage).Start();
                btnTestScenario.Enabled = false;
            }
            else
            { 
                btnTestScenario.Enabled = true;
                Global.gAutoBetStatus = Global.AUTOBET_STATUS.AUTOBET_STOPPED;
                ClearAllTables();
            }
            bIsOpenPage = !bIsOpenPage;
        }
               
        private void closeOpendCasinoSite()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            if (procsChrome.Length <= 0)
            {
                return;
            }
            else
            {
                foreach (Process proc in procsChrome)
                {
                    // the chrome process must have a window 
                    if (proc.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }
                    // find the automation element
                    AutomationElement elm = AutomationElement.FromHandle(proc.MainWindowHandle);
                    AutomationElement elmUrlBar = elm.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

                    if (elmUrlBar != null)
                    {
                        AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
                        if (patterns.Length > 0)
                        {
                            ValuePattern val = (ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0]);
                            string []urls = val.Current.Value.Split('/');
                            Console.WriteLine("Chrome URL found: " + urls[0]);
                            for (int i = 0;i < this.comboURL.Items.Count;i ++)
                            {
                                string urllist = this.comboURL.Items[i].ToString();
                                if (urllist.Contains(urls[0]) && urls[0].Length != 0)
                                {
                                    if (Global.elementInterace != null)
                                        Global.elementInterace.CloseDrive();
                                    proc.CloseMainWindow();
                                }
                                    
                            }
                            
                        }
                    }
                    proc.Close();
                }
            }
        }
        private void btnProcessLogin_Click(object sender, EventArgs e)
        {
            Global.elementInterace.LogIn(txtUserID.Text, txtPassword.Text);
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            SaveSettingsConfig();
            tmUpdateCheck.Stop();
            tmDailyTargetMsg.Stop();
            tmStripeStatus.Stop();
            if (Global.elementInterace != null)
                Global.elementInterace.CloseDrive();
        }

        private delegate void delegateUpdateStatsListView(int nTableIndex);

        public void UpdateStatsListView(int nTableIndex)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delegateUpdateStatsListView(UpdateStatsListView));
                return;
            }
            int nIndex = 1;

            Table selectTable = Global.tbMonitor.m_lsTable.ElementAt(nTableIndex).Value;
            int nTableListIndex = selectTable.m_nTableListIndexUI;
            if (selectTable.m_lstNumbers.Count == 0)
            {
                listView_StatsArr[nTableListIndex].Items.Clear();
                return;
            }
            groupBox_StatsArr[nTableListIndex].Text = selectTable.m_strTableName;

            for (int i = selectTable.m_nChangedSpinCount - 1; i >= 0; i--)
            {
                ListViewItem lvItem = new ListViewItem((i + 1).ToString());
                for (int j = 1; j < listView_StatsArr[nTableListIndex].Columns.Count; j++)
                    lvItem.SubItems.Add("");

                lvItem.UseItemStyleForSubItems = false;
                Global.BET_SPOT betNumber = selectTable.m_lstNumbers[i];
                Global.BET_SPOT betColor = Global.GetBetColor(betNumber);

                lvItem.SubItems[colStatsNumber.Index].Text = Global.GetBetSpotString(betNumber);
                if (betColor == Global.BET_SPOT.COLOR_RED)
                    lvItem.SubItems[colStatsNumber.Index].ForeColor = Color.Red;
                else if (betColor == Global.BET_SPOT.ZERO)
                    lvItem.SubItems[colStatsNumber.Index].ForeColor = Color.Green;

                if (betNumber != Global.BET_SPOT.ZERO)
                {
                    Global.BET_SPOT betOddEven = Global.GetBetOddEven(betNumber);
                    lvItem.SubItems[colStatsOddEven.Index].Text = Global.GetBetSpotString(betOddEven);

                    Global.BET_SPOT betDozen = Global.GetBetDozen(betNumber);
                    lvItem.SubItems[colStatsDozen.Index].Text = Global.GetBetSpotString(betDozen);

                    Global.BET_SPOT betColumn = Global.GetBetColumn(betNumber);
                    lvItem.SubItems[colStatsColumn.Index].Text = Global.GetBetSpotString(betColumn);

                    Global.BET_SPOT betHalf = Global.GetBetHalf(betNumber);
                    lvItem.SubItems[colStatsHalf.Index].Text = Global.GetBetSpotString(betHalf);
                }
                listView_StatsArr[nTableListIndex].Items.Insert(0, lvItem);
            }
            foreach (ListViewItem item in listView_StatsArr[nTableListIndex].Items)
            {
                item.SubItems[0].Text = nIndex.ToString();
                nIndex++;
            }
            selectTable.m_nChangedSpinCount = 0;
            
        }

        private delegate void delegateEmptyStatsListView(int nTableIndex);
        public void EmptyStatsListView(int nTableIndex)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delegateEmptyStatsListView(EmptyStatsListView));
                return;
            }
            Table selectTable = Global.tbMonitor.m_lsRemovedTable.ElementAt(nTableIndex);
            int nTableListIndex = selectTable.m_nTableListIndexUI;
            listView_StatsArr[nTableListIndex].Items.Clear();
            groupBox_StatsArr[nTableListIndex].Text = "Table :";

        }

        private void ClearAllTables()
        {
            int index = 0;
            foreach (ListView lstVi in listView_StatsArr)
            { 
                lstVi.Items.Clear();
                groupBox_StatsArr[index].Text = "Table :";
                index++;
            }
            labCurrentBalance.Text = "0";

        }
        private void btnOpenStats_Click(object sender, EventArgs e)
        {
        }

        private void btnClickNumberStats_Click(object sender, EventArgs e)
        {
            
        }

        private void btnCheckNumStatsOpen_Click(object sender, EventArgs e)
        {
        }

        private void btnReadStatistics_Click(object sender, EventArgs e)
        {
        }

        private void btnGetTimer_Click(object sender, EventArgs e)
        {
            Global.elementInterace.GetTimer();
        }

        private void btnGetStatus_Click(object sender, EventArgs e)
        {
            Global.elementInterace.InitParentFrame();
        }

        private void btnGetBalance_Click(object sender, EventArgs e)
        {
        }

        private void btnGetTableName_Click(object sender, EventArgs e)
        {
        }

        private void btnCloseTable_Click(object sender, EventArgs e)
        {
//            Global.elementInterace.CloseTable();
        }

        private void btnGotoRobby_Click(object sender, EventArgs e)
        {
            Global.elementInterace.GotoRobby();
        }       

        private void btnOpenActivatedTable_Click(object sender, EventArgs e)
        {
            Global.elementInterace.OpenActivatedTable();
        }


        private void btnPlace_Click(object sender, EventArgs e)
        {

        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
           
        }

        private void btnBet_Click(object sender, EventArgs e)
        {
            if (bIsOpenPage)
            {
                Global.log.addLog("Please open page", Global.LOG_LEVEL_LISTFILE);
                return;
            }

            if (ExistFeeInvoice)
            {
                MessageBox.Show("Payment has been expired.\nPlease process payment monthly fee to use software continuously.", "Notification");
                return;
            }

            if (Global.gAutoBetStatus == Global.AUTOBET_STATUS.AUTOBET_STOPPED)
            {
                if (Global.tbMonitor.m_lsTable.Count == 0)
                {
                    nRemovedTablePreCnt = 0;
                    Global.tbMonitor.Start();
                }
                else
                    Global.log.addLog("Auto Betting Started", Global.LOG_LEVEL_LISTFILE);
                btnAutoBet.Text = "Stop Auto Betting";
                Global.gAutoBetStatus = Global.AUTOBET_STATUS.AUTOBET_STARTED;
                Global.gbDailyReachedStatus = false;
            }
            else
            {
                btnAutoBet.Text = "Start Auto Betting";
                Global.gAutoBetStatus = Global.AUTOBET_STATUS.AUTOBET_STOPPED;
                Global.log.addLog("Auto Betting Stopped", Global.LOG_LEVEL_LISTFILE);
            }
            
        }

        private void btnTestScenario_Click(object sender, EventArgs e)
        {
            Global.gbOperationMode = Global.LIVESCENARIO_MODE.SCENARIOTEST_MODE;
            new ScenarioTest().ShowDialog();
        }

        private void OpenPage()
        {
            Global.elementInterace = new WebElementInterface(siteURL);
            if (Global.elementInterace.driver == null)
            {
                bIsOpenPage = true;
            }
        }

        private void chkbetsimulreal_Click(object sender, EventArgs e)
        {
            for (int iTableIndex = 0;iTableIndex < Global.tbMonitor.m_lsTable.Count; iTableIndex ++)
            { 
                for (int algoIndex = 0; algoIndex < Global.tbMonitor.m_lsTable.ElementAt(iTableIndex).Value.m_lstCriterias.Count; algoIndex++)
                {
                    Global.tbMonitor.m_lsTable.ElementAt(iTableIndex).Value.m_lstCriterias[algoIndex].SetInitSequence();
                }
            }
            Global.gbIsSimulationMode = chkUseSimulation.Checked;
        }

        private void comboURL_SelectedIndexChanged(object sender, EventArgs e)
        {
            siteURL = this.comboURL.Text;
        }

        delegate void delegateSetText(string text);

        public void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            try
            {
                if (this.txtLog.InvokeRequired)
                {
                    delegateSetText d = new delegateSetText(SetText);
                    this.txtLog.Invoke(d, new object[] { text });
                    return;
                }

                this.txtLog.AppendText(text);
            }
            catch
            {
                Console.WriteLine("Add Log Exception");
            }
        }

        private void tmUpdateCheck_Tick(object sender, EventArgs e)
        {

            labDailyTarget.Text = GlobalData.GetDailyTarget().ToString();
            labStartBalance.Text = GlobalData.GetStartBalance().ToString();
            labCurrentBalance.Text = GlobalData.GetCurrentBalance().ToString();

            for (int iTableIndex = 0; iTableIndex < Global.tbMonitor.m_lsTable.Count; iTableIndex ++)
            { 
                if (Global.tbMonitor.m_lsTable.ElementAt(iTableIndex).Value.m_nChangedSpinCount != 0)
                {
                    UpdateStatsListView(iTableIndex);
                }
            }

            int nRemovedTableCnt = Global.tbMonitor.m_lsRemovedTable.Count;
            if (nRemovedTableCnt != nRemovedTablePreCnt)
            {
                for (int iTableIndex = nRemovedTablePreCnt; iTableIndex < nRemovedTableCnt; iTableIndex ++)
                {
                    EmptyStatsListView(iTableIndex);
                }
                nRemovedTablePreCnt = nRemovedTableCnt;
            }

            if (Global.gAutoBetStatus == Global.AUTOBET_STATUS.AUTOBET_STARTED)
                btnAutoBet.Text = "Stop Auto Betting";
            else
                btnAutoBet.Text = "Start Auto Betting";

            if (bIsOpenPage)
                btnOpenPage.Text = "Open Page";
            else
                btnOpenPage.Text = "Close Page";
        }

        private void LoadSettingsFromConfig()
        {
            string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"];
            string useSimulation = System.Configuration.ConfigurationManager.AppSettings["UseSimulation"];
            string useCheckEmail = System.Configuration.ConfigurationManager.AppSettings["UseCheckEmail"];
            if (siteUrl == null || useSimulation == null || useCheckEmail == null)
            {
                GlobalData.AddValue("SiteUrl", comboURL.Text);
                GlobalData.AddValue("UseSimulation", chkUseSimulation.Checked == true ? "1" : "0");
                GlobalData.AddValue("UseCheckEmail", "");

                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                comboURL.Text = siteUrl;
                chkUseSimulation.Checked = useSimulation == "1" ? true : false;
                Global.gbIsSimulationMode = chkUseSimulation.Checked;
                EmailOpening = useCheckEmail;
            }
        }

        private void SaveSettingsConfig()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["SiteUrl"].Value = comboURL.Text;
            config.AppSettings.Settings["UseSimulation"].Value = chkUseSimulation.Checked == true ? "1" : "0";
            config.AppSettings.Settings["UseCheckEmail"].Value = EmailOpening;
            config.Save(ConfigurationSaveMode.Full, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void tmDailyTargetMsg_Tick(object sender, EventArgs e)
        {
            if (Global.gbDailyReachedStatus)
            {
                this.tmDailyTargetMsg.Stop();
                if (GlobalData.DialogBox("Alert", "You have reached your daily target, we recommend restarting the bot tomorrow", "Continue", "Leave") == DialogResult.OK)
                    GlobalData.SetStartBalance(GlobalData.GetCurrentBalance(), true);
                else
                    Global.gAutoBetStatus = Global.AUTOBET_STATUS.AUTOBET_STOPPED;

                Global.gbDailyReachedStatus = false;
                this.tmDailyTargetMsg.Start();
            }
        }

        private string GetRemainString(double remainDay)
        {
            string result = "";
            result = (Convert.ToInt32(Math.Ceiling(remainDay))).ToString() + " days";
            return result;
        }
        private void tmStripeStatus_Tick(object sender, EventArgs e)
        {
            CheckRemainDays();
        }

        private void GetRemainDayThread()
        {
            while (true)
            {
                GetRemainDay();
                ShowBuyButton();
                Thread.Sleep(1000 * 60 * 1);
            }
        }

        private void GetRemainDay()
        {
            ExistFeeInvoice = Global.stripe.CheckInCompleteInvoice();
            bool trialmode = Global.stripe.CheckTrialMode();
            if (trialmode)
            {
                RemainDays = Global.stripe.GetRemainTrialDays();
                Global.AccountMode = Global.ACCOUNT_STATUS.TRIAL_MODE;
            }
            else
            {
                RemainDays = Global.stripe.GetRemainPaidDays();
                Global.AccountMode = Global.ACCOUNT_STATUS.PAY_MODE;
            }
        }

        private delegate void delegateShowBuyButton();
        public void ShowBuyButton()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delegateShowBuyButton(ShowBuyButton));
                return;
            }
            if (RemainDays <= SHOWBUY_LEFTDAY || ExistFeeInvoice)
                this.btnPayFee.Visible = true;
            else
                this.btnPayFee.Visible = false;
        }
        private void GetRemainPaidDays()
        {
            ExistFeeInvoice = Global.stripe.CheckInCompleteInvoice();
            RemainDays = Global.stripe.GetRemainPaidDays();
            Global.AccountMode = Global.ACCOUNT_STATUS.PAY_MODE;
        }
        private void CheckRemainDays()
        {
            if (Global.AccountMode == Global.ACCOUNT_STATUS.NONE)
                return;

            if (Global.AccountMode == Global.ACCOUNT_STATUS.TRIAL_MODE)
            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
                string labText = "";
                labText = GetRemainString(RemainDays) + " left of trial";
                this.labRemainDay.Text = labText;
            }
            else
            {
                if (ExistFeeInvoice)
                {

                    tmStripeStatus.Stop();
                    Global.gAutoBetStatus = Global.AUTOBET_STATUS.AUTOBET_STOPPED;
                    string labText = "";
                    labText = "Payment is expired";
                    this.labRemainDay.Text = labText;
                    if (!TrialStoppedMsg)
                    {
                        TrialStoppedMsg = true;
                        Global.log.addLog("Your payment has been expired.", Global.LOG_LEVEL_LISTFILE);
                        feeDlg.SetPrice(Global.stripe.feeProductInfo);
                        if (feeDlg.ShowDialog() == DialogResult.Cancel)
                        {
                            tmStripeStatus.Start();
                            return;
                        }
                        
                        Global.gAutoBetStatus = Global.AUTOBET_STATUS.AUTOBET_STARTED;
                        GetRemainPaidDays();
                        ShowBuyButton();
                        ExistFeeInvoice = false;
                    }
                    tmStripeStatus.Start();
                }
                else if(RemainDays > 0 && !ExistFeeInvoice)
                {
                    string labText ="";
                    labText = GetRemainString(RemainDays) + " left until expired";
                    this.labRemainDay.Text = labText;
                }
            }
        }

        private void btnPayFee_Click(object sender, EventArgs e)
        {
            if (BuyDialogOpen)
                return;

            Thread thFee = new Thread(PayFeeThread);
            thFee.Start();
            thFee.IsBackground = true;
        }

        private void PayFeeThread()
        {
            BuyDialogOpen = true;
            
            if (ExistFeeInvoice)
            {
                feeDlg.SetPrice(Global.stripe.feeProductInfo);
                if (feeDlg.ShowDialog() == DialogResult.OK)
                    GetRemainPaidDays();
            }
            else
            {
                BuyDialog buyDlg = new BuyDialog();
                buyDlg.SetLabel(Global.stripe.feeProductInfo, Convert.ToInt32(RemainDays));
                if (buyDlg.ShowDialog() == DialogResult.OK)
                    GetRemainPaidDays();
            }
            BuyDialogOpen = false;
        }
    }
}
