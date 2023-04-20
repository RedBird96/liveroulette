using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;

namespace LiveRoulette
{
    class GlobalData
    {
        private static decimal gdayStartBalance;
        private static decimal gCurrentBalance;

        private static string CONFIG_NAME = "Config.ini";
        public static List<Global.BET_SPOT> glstNumbers = new List<Global.BET_SPOT>();

        public static List<Global.BET_SPOT> glstTestNumbers = new List<Global.BET_SPOT>();

        public static int[,] gstackAmount = new int[,]{ { 1, 2, 4, 7, 11, 17, 26, 40, 61, 93, 141, 214,324,324,324,324,324 }, 
                                                        { 2,4,8,14,22,34,52,80,122,186,282,428,648,978,978,978,978},
                                                        {  3,6,12,21,33,51,78,120,183,279,423,642,972,1467,1467,1467,1467  },
                                                        { 4,8,16,28,44,68,104,160,244,372,564,856,1296,1956,1956,1956,1956},
                                                        { 5, 10, 20, 35, 55, 85, 130, 200, 305, 465, 705, 1070, 1620, 2445, 3690, 5550, 8350} };
        public static decimal[] gdailytarget = { 20, 40, 50, 60, 70};
        
        public static Global.BET_SPOT[] GetStatsNumbersExceptZero(List<Global.BET_SPOT> lstSpinAll)
        {
            List<Global.BET_SPOT> lstStats = new List<Global.BET_SPOT>();
            for (int i = 0; i < lstSpinAll.Count; i++)
                if (lstSpinAll[i] != Global.BET_SPOT.ZERO)
                    lstStats.Add(lstSpinAll[i]);
            return lstStats.ToArray();
        }

        public static void SetStartBalance(decimal balance, bool writeForce = false)
        {
            try { 
                string configPath = Path.Combine(Directory.GetCurrentDirectory(), CONFIG_NAME);
                var iniManager = new FileIniDataParser();
                var data = iniManager.ReadFile(configPath);
                string cuDt = DateTime.Now.ToString("yyyy-MM-dd");
                string startStakeDt = data[Global.gSiteName]["BalanceDate"];

                if (startStakeDt != cuDt || writeForce || gdayStartBalance == 0)
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    data[Global.gSiteName]["StartBalance"] = balance.ToString();
                    data[Global.gSiteName]["BalanceDate"] = cuDt;
                    iniManager.WriteFile(configPath, data);
                }
                gdayStartBalance = balance;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static decimal GetStartBalance()
        {
            try
            {
                CheckExistINIFile();
                string configPath = Path.Combine(Directory.GetCurrentDirectory(), CONFIG_NAME);
                var iniManager = new FileIniDataParser();
                var data = iniManager.ReadFile(configPath);

                string startBalanceConfig = data[Global.gSiteName]["StartBalance"];

                if (decimal.TryParse(startBalanceConfig, out gdayStartBalance) == false)
                    return 0m;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return gdayStartBalance;
        }

        private static void CheckExistINIFile()
        {
            
                string configPath = Path.Combine(Directory.GetCurrentDirectory(), CONFIG_NAME);
                if (!File.Exists(configPath))
                {
                    FileStream configStream = File.Create(configPath);
                    configStream.Close();
                    
                    FileIniDataParser Parser = new FileIniDataParser();
                    IniData data = Parser.ReadFile(CONFIG_NAME);

                    foreach(string strName in Global.gSiteNameArray)
                    { 
                        data.Sections.AddSection(strName);

                        data[strName].AddKey("StartBalance", "0");
                        data[strName].AddKey("BalanceDate", "");
                    }
                    Parser.WriteFile(CONFIG_NAME, data);

                }
            
        }

        public static void SetCurrentBalance(decimal balance)
        {
            gCurrentBalance = balance;
        }

        public static decimal GetCurrentBalance()
        {
            return gCurrentBalance;
        }

        public static decimal GetStartStake(decimal balance)
        {
            if (balance >= 0 && balance < 500)
                return 1m;
            if (balance >= 500 && balance < 2000)
                return 2m;
            if (balance >= 2000 && balance < 3000)
                return 3m;
            if (balance >= 3000 && balance < 4000)
                return 4m;
            return 5m;
        }
        public static int GetStackAmountBySequence(int sequence, decimal balance)
        {
            int result = 0;
            decimal startstake = GetStartStake(balance);
            int index = Convert.ToInt32(startstake) - 1;
            int amount_index;
            if (index == 0)
                amount_index = sequence % 13;
            else if (index != 4)
                amount_index = sequence % 14;
            else
                amount_index = sequence % 17;

            result = gstackAmount[index, amount_index];
            return result;
        }

        public static decimal GetDailyTarget()
        {
            decimal startstake = GetStartStake(GetCurrentBalance());
            int index = Convert.ToInt32(startstake) - 1;
            return gdailytarget[index];
        }
        public static void AddValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Minimal);
        }

        public static DialogResult DialogBox(string title, string promptText, string button1 = "OK", string button2 = "Cancel", string button3 = null)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button button_1 = new Button();
            Button button_2 = new Button();
            Button button_3 = new Button();

            int buttonStartPos = 228; //Standard two button position


            if (button3 != null)
                buttonStartPos = 228 - 81;
            else
            {
                button_3.Visible = false;
                button_3.Enabled = false;
            }


            form.Text = title;

            // Label
            label.Text = promptText;
            label.SetBounds(9, 20, 372, 13);
            label.Font = new Font("Microsoft Tai Le", 10, FontStyle.Regular);

            textBox.SetBounds(12, 36, 372, 20);
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;

            button_1.Text = button1;
            button_2.Text = button2;
            button_3.Text = button3 ?? string.Empty;
            button_1.DialogResult = DialogResult.OK;
            button_2.DialogResult = DialogResult.Cancel;
            button_3.DialogResult = DialogResult.Yes;


            button_1.SetBounds(buttonStartPos, 72, 75, 23);
            button_2.SetBounds(buttonStartPos + 81, 72, 75, 23);
            button_3.SetBounds(buttonStartPos + (2 * 81), 72, 75, 23);

            label.AutoSize = true;
            button_1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, button_1, button_2 });
            if (button3 != null)
                form.Controls.Add(button_3);

            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = button_1;
            form.CancelButton = button_2;

            DialogResult dialogResult = form.ShowDialog();
            return dialogResult;
        }
    }
}
