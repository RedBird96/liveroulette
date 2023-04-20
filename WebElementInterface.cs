using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveRoulette
{
    public class WebElementInterface
    {
        public IWebDriver driver = null;

        public bool bFrameSwitched = false;

        public string chromeFilePath = "";

        public bool bLoadContainerUse = false;

        public int nSingleFrameIndex = 0;

        public WebElementInterface(string urlParam)
        {
            try
            {
                if (!CheckChromeVersion())
                    return;

                chromeFilePath = GetChromePath();
                var driverService = ChromeDriverService.CreateDefaultService(@".\");
                driverService.HideCommandPromptWindow = true;

                ChromeOptions options = new ChromeOptions();
                int xPos = 100;
                int yPos = 100;
                options.AddArgument("--window-size=1000,800");
                options.AddArgument($"--window-position={xPos},{yPos}");
                options.AddArgument("--disable-web-security");

                options.BinaryLocation = chromeFilePath;

                driver = new ChromeDriver(driverService, options);

                bFrameSwitched = false;

                driver.Url = urlParam;
                driver.Navigate();

                bLoadContainerUse = false;
            }
            catch
            {
                Global.log.addLog("Page Error", Global.LOG_LEVEL_ONLYFILE);
            }
        }

        private bool CheckChromeVersion()
        {

            string strChromeVersion;
            RegistryKey browserKeys;

            browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
            if (browserKeys == null)
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");

            string[] browserNames = browserKeys.GetSubKeyNames();
            if (!browserNames.Contains("Google Chrome"))
            {
                Global.log.addLog("Not installed Goold Chrome on PC", Global.LOG_LEVEL_LISTFILE);
                return false;
            }

            string path = GetChromePath();
            strChromeVersion = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
            string driverPath = Path.Combine(Directory.GetCurrentDirectory(), "chromedriver.exe");
            if (!File.Exists(driverPath))
            {
                Global.log.addLog("Not exist chromedriver.exe \n You can download chromedriver.exe from here https://chromedriver.chromium.org/downloads", Global.LOG_LEVEL_LISTFILE);
                return false;
            }

            var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = driverPath,
                    Arguments = "-v",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            );
            using (StreamReader reader = process.StandardOutput)
            { 
                string result = reader.ReadToEnd();
                string[] strOutputArr = result.Split(' ');
                string strChromedriverVersion = strOutputArr[1];

                Global.log.addLog($"Browser Version : {strChromeVersion},  Driver Version : {strChromedriverVersion}", Global.LOG_LEVEL_ONLYFILE);
                string[] browser_version = strChromeVersion.Split('.');
                string[] driver_version = strChromedriverVersion.Split('.');
                if (string.Compare(browser_version[0], driver_version[0]) > 0)
                {
                    Global.log.addLog("Please update to the latest version of Google Chrome and chromedriver.exe.  \n You can download chromedriver.exe from here https://chromedriver.chromium.org/downloads", Global.LOG_LEVEL_LISTFILE);
                    return false;
                }
            }
            
            return true;
        }
        private string GetChromePath()
        {
            try
            {
                string chromeBinaryPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "Path", null);
                if (chromeBinaryPath != null)
                {
                    chromeBinaryPath = Path.Combine(chromeBinaryPath, "chrome.exe");
                    return chromeBinaryPath;
                }
            }
            catch (Exception ex)
            {
                Global.log.addLog(ex.StackTrace, Global.LOG_LEVEL_ONLYFILE);
            }
            return "";
        }

        public void CheckFrameContainer()
        {
            try {
                ReadOnlyCollection<IWebElement> freamlist = driver.FindElements(By.TagName("iframe"));
                ReadOnlyCollection<IWebElement> elPa = driver.FindElements(By.ClassName("loader-frame-container"));
                if (elPa.Count != 0)
                    bLoadContainerUse = true;
                driver.SwitchTo().Frame(nSingleFrameIndex);
                ReadOnlyCollection<IWebElement> elFr =  driver.FindElements(By.ClassName("loader-frame-container"));
                if (elFr.Count != 0)
                    bLoadContainerUse = true;

                ReadOnlyCollection<IWebElement> elFr1 = driver.FindElements(By.CssSelector("[data-role='statistics-button']"));
                GotoParentFrame();
            }
            catch(Exception ex)
            {
                Global.log.addLog($"CheckFrameContainer : {ex.Message}");
            }
        }


        public IWebDriver SearchParentFrame(IWebElement iFrame)
        {
            driver.SwitchTo().Frame(iFrame);
            CheckPinCloseButton();
            CheckSessionReminderButton();
            ReadOnlyCollection<IWebElement> gameFrames = driver.FindElements(By.TagName("iframe"));
            if (driver.FindElements(By.CssSelector("[data-role='statistics-button']")).Count != 0)
                return driver.SwitchTo().ParentFrame();

            if (gameFrames.Count == 0)
            {
               GotoParentFrame();
            }
            else
            {
                IWebDriver parent = null;
                foreach (IWebElement ie in gameFrames)
                {
                    parent = SearchParentFrame(ie);
                    if (parent != null)
                        return parent;
                }
            }
            return null;
        }
        public bool InitParentFrame()
        {
            try { 
                driver.SwitchTo().DefaultContent();
                bFrameSwitched = false;
                CheckPinCloseButton();
                CheckSessionReminderButton();
                ReadOnlyCollection<IWebElement> gameFrames = driver.FindElements(By.TagName("iframe"));
                if (gameFrames == null)
                    return false;

                IWebDriver parent = null;
                foreach (IWebElement ie in gameFrames)
                {
                    parent = SearchParentFrame(ie);
                    if (parent != null)
                        break;
                }

                if (parent == null)
                    return false;

                nSingleFrameIndex = 0;
                gameFrames = driver.FindElements(By.TagName("iframe"));
                for (nSingleFrameIndex = 0; nSingleFrameIndex < gameFrames.Count(); nSingleFrameIndex ++)
                {
                    driver.SwitchTo().Frame(nSingleFrameIndex);
                    if (driver.FindElements(By.CssSelector("[data-role='statistics-button']")).Count != 0)
                        break;
                    GotoParentFrame();
                }
                GotoParentFrame();
                CheckFrameContainer();

                return true;
            }
            catch(Exception ex)
            {
                Global.log.addLog($"InitParentFrame : {ex.Message}");
            }

            return false;
        }

        private bool SwitchTableFrameByIndex(int index)
        {
            int nTableIndex = bLoadContainerUse ? index : nSingleFrameIndex;
            /*ReadOnlyCollection<IWebElement> gameFrames = driver.FindElements(By.TagName("iframe"));
            if (gameFrames == null)
                return false;

            if (index >= gameFrames.Count)
                return false;
            */
            driver.SwitchTo().Frame(nTableIndex);

            return true;
        }
        private bool SwitchTableFrameByName(string name)
        {
            ReadOnlyCollection<IWebElement> gameFrames = driver.FindElements(By.TagName("iframe"));
            int index;
            if (gameFrames == null)
                return false;

            for(index = 0; index < gameFrames.Count; index++)
            {
                driver.SwitchTo().Frame(index);
                List<string> table_one = new List<string>();
                getTableInfo(out table_one);
                if (table_one[0] == name)
                    break;
                GotoParentFrame();
            }

            return true;
        }

        public void GotoParentFrame()
        {
            try
            {
                driver.SwitchTo().ParentFrame();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public string GetBalance(int nTableIndex)
        {
            string result = "";
            try
            {
                if (SwitchTableFrameByIndex(nTableIndex) == false)
                {
                    Global.log.addLog("Failed to switch table frame", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }
                ReadOnlyCollection<IWebElement> _elsBalanceLabel = driver.FindElements(By.CssSelector("[data-role='balance-label__value']"));
                if (_elsBalanceLabel.Count == 0)
                {
                    GotoParentFrame();
                    Global.log.addLog("Failed to find [data-role='balance-label__value']", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }

                IWebElement elBalance = _elsBalanceLabel[0];
                result = elBalance.Text;
            }
            catch (Exception ex)
            {
                Global.log.addLog($"GetBalance : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }
            GotoParentFrame();
            return result;
        }

        public bool CheckPlaceInfoTime(IWebElement elePlaceTimer)
        {
            bool result = false;
            try
            {
                if (elePlaceTimer == null)
                {
                    ReadOnlyCollection<IWebElement> elePlace = driver.FindElements(By.CssSelector("[data-role='footer-circle-timer']"));
                    if (elePlace.Count == 0)
                        return result;
                    elePlaceTimer = elePlace[0];
                    
                }
                IWebElement subEle = elePlaceTimer.FindElement(By.CssSelector("[class^='container']"));
                result = true;
            }
            catch (Exception ex)
            {
                
            }

            return result;
        }

        public bool CheckPlaceInfoText(IWebElement elPlaceText)
        {
            bool result = false;
            try
            {
                ReadOnlyCollection<IWebElement> elePlace = driver.FindElements(By.CssSelector("[data-role='traffic-light']"));
                if (elePlace == null || elePlace.Count == 0)
                    return result;

                string text = elePlace[0].Text;
                if (text.Length != 0 && (text == "PLACE YOUR BETS" || text == "BETS CLOSING"))
                    result = true;
            }
            catch (Exception ex)
            {
                Global.log.addLog($"CheckPlaceInfoText : {ex.Message}");
            }

            return result;
        }

        public string GetTotalBet()
        {
            string result = "";
            try
            {
                ReadOnlyCollection<IWebElement> _elsTotablBetLabel = driver.FindElements(By.CssSelector("[data-role='total-bet-label__value']"));
                if (_elsTotablBetLabel.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='total-bet-label__value']", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }

                IWebElement elTotalBet = _elsTotablBetLabel[0];
                result = elTotalBet.Text;
            }
            catch (Exception ex)
            {
                Global.log.addLog($"GetTotalBet : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }

            return result;
        }

        public void LogIn(string userName, string serPwd)
        {

            if (driver == null)
            {
                return;
            }

            try
            {
                ReadOnlyCollection<IWebElement> elsOpenLogin = driver.FindElements(By.ClassName("open-login"));
                if (elsOpenLogin.Count == 0)
                    return;

                IWebElement elementOpenLogin = elsOpenLogin[0];
                elementOpenLogin.Click();
                Thread.Sleep(200);

                IWebElement elInputUserName = driver.FindElement(By.Id("input-username"));
                IWebElement elInputUserPassword = driver.FindElement(By.Id("input-password"));

                elInputUserName.SendKeys(userName);
                Thread.Sleep(100);
                elInputUserPassword.SendKeys(serPwd);

                ReadOnlyCollection<IWebElement> elsLoginButton = driver.FindElements(By.ClassName("loginBtn"));
                if (elsLoginButton.Count == 0)
                {
                    MessageBox.Show("Can't find login button! Please contact the developer.", "Notification");
                    return;
                }

                IWebElement elementLogin = elsLoginButton[0];

                elementLogin.Click();

            }
            catch (Exception ex)
            {
                return;
            }
        }
        
        public void OpenStatus()
        {
            try
            {
                ReadOnlyCollection<IWebElement> _elsStatisticsButtons = driver.FindElements(By.CssSelector("[data-role='statistics-button']"));
                if (_elsStatisticsButtons.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='statistics-button']", Global.LOG_LEVEL_ONLYFILE);
                    return;
                }

                IWebElement elStatisticsButton = _elsStatisticsButtons[0];
                string className = elStatisticsButton.GetAttribute("class");
                if (className.Contains("buttonStateActivated"))
                {
                    // already activated
                    Global.log.addLog("deactivate Statistics.", Global.LOG_LEVEL_ONLYFILE);
                }
                else
                {
                    // in default
                    Global.log.addLog("activate Statistics.", Global.LOG_LEVEL_ONLYFILE);
                }

                elStatisticsButton.Click();
            }
            catch (Exception ex)
            {
                Global.log.addLog($"OpenStatus : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }
        }

        public void ClickNumberStatus()
        {
            try
            {
                ReadOnlyCollection<IWebElement> _elsPaginators = driver.FindElements(By.CssSelector("[data-role='paginator-item-numbers']"));
                if (_elsPaginators.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='paginator-item-numbers']", Global.LOG_LEVEL_ONLYFILE);
                    return;
                }

                IWebElement elNumbersButton = _elsPaginators[0];
                string dataRoleValue = elNumbersButton.GetAttribute("data-role-value");
                if (string.CompareOrdinal(dataRoleValue, "active") == 0)
                {
                    // already activated
                    Global.log.addLog("already activated.", Global.LOG_LEVEL_ONLYFILE);
                }
                else
                {
                    // in default
                    Global.log.addLog("not activated. will click", Global.LOG_LEVEL_ONLYFILE);
                }

                elNumbersButton.Click();
            }
            catch (Exception ex)
            {
                Global.log.addLog($"ClickNumberStatus : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }
        }

        private void CloseTutorial()
        {
            // 1. Click Next Button
            ReadOnlyCollection<IWebElement> _elsCloseFirstButtons = driver.FindElements(By.CssSelector("[data-role='tutorial.next']"));
            if (_elsCloseFirstButtons.Count == 0)
            {
                return;
            }
            IWebElement elCloseFirstButton = _elsCloseFirstButtons[0];
            elCloseFirstButton.Click();
            Thread.Sleep(1000);

            // 2. Click Close Button
            ReadOnlyCollection<IWebElement> _elsCloseLasttButtons = driver.FindElements(By.CssSelector("[data-role='tutorials.close']"));
            if (_elsCloseLasttButtons.Count == 0)
            {
                return;
            }
            IWebElement elCloseLasttButton = _elsCloseLasttButtons[0];
            elCloseLasttButton.Click();
            Thread.Sleep(200);

        }
        public bool CheckNumStatusOpen()
        {
            bool result = false;

            try
            {
                // 1. Click Statistics Button
                ReadOnlyCollection<IWebElement> _elsStatisticsButtons = driver.FindElements(By.CssSelector("[data-role='statistics-button']"));
                if (_elsStatisticsButtons.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='statistics-button']", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }

                IWebElement elStatisticsButton = _elsStatisticsButtons[0];
                string className = elStatisticsButton.GetAttribute("class");
                if (className.Contains("buttonStateDefault"))
                {
                    // already activated
//                    Global.log.addLog("Click Statistics button", Global.LOG_LEVEL_ONLYFILE);
                    elStatisticsButton.Click();
                    Thread.Sleep(100);
                }

                // 2. Click paginator-numbers
                ReadOnlyCollection<IWebElement> _elsPaginators = driver.FindElements(By.CssSelector("[data-role='paginator-item-numbers']"));
                if (_elsPaginators.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='paginator-item-numbers']", Global.LOG_LEVEL_ONLYFILE);
                    result = false;
                }
                
                IWebElement elNumbersButton = _elsPaginators[0];

                string dataRoleValue = elNumbersButton.GetAttribute("data-role-value");
                if (string.CompareOrdinal(dataRoleValue, "inactive") == 0)
                {
                    int retry = 0;
                    int nOldY = 0;
                    bool bFound = false;
                    while (++retry < 10)
                    {
                        int nCurY = elNumbersButton.Location.Y;
                        if (nOldY == nCurY)
                        {
                            bFound = true;
                            break;
                        }
                        nOldY = nCurY;
                        Thread.Sleep(200);
                    }
                    if (!bFound)
                    {
                        Global.log.addLog("Loading Statistic Panel");
                        return false;
                    }

//                    Global.log.addLog("Click numbers", Global.LOG_LEVEL_ONLYFILE);
                    elNumbersButton.Click();
                    result = true;
                }
                else if (string.CompareOrdinal(dataRoleValue, "active") == 0)   // already activated
                    result = true;
            }
            catch (Exception ex)
            {
                Global.log.addLog($"CheckNumStatusOpen : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }
            return result;
        }

        public string ReadStatistics(int nTableIndex,int type, out IWebElement spotEle, out IWebElement balanceElement, out IWebElement totalBetElement)
        {
            string result ="";
            spotEle = null;
            balanceElement = null;
            totalBetElement = null;
            try
            {
                if (SwitchTableFrameByIndex(nTableIndex) == false)
                {
                    Global.log.addLog("Failed to switch table frame", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }

                if (!CheckNumStatusOpen())
                {
                    return result;
                }


                ReadOnlyCollection<IWebElement> elsRecentnumbers = driver.FindElements(By.CssSelector("[data-role='recent-number']"));
                if (elsRecentnumbers.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='recent-number']", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }

                IWebElement pa_rc = GetParent(elsRecentnumbers[0]);
                string pa_recent = pa_rc.Text;
                char ch = '\n';
                pa_recent = pa_recent.Replace(ch.ToString(), String.Empty);
                pa_recent = pa_recent.Replace('\r', ' ');
                result = pa_recent;

                spotEle = pa_rc;
                if (type == 0)      //To get top row only
                {
                    return result;
                }

                ReadOnlyCollection<IWebElement> elsStats = driver.FindElements(By.CssSelector("[data-role='statistics']"));
                if (elsStats.Count == 0)
                {
                    GotoParentFrame();
                    //Global.log.addLog("Failed to find [data-role='statistics']", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }

                IWebElement pa = GetParent(elsStats[0]);
                string staArr = pa.Text;
                staArr = staArr.Replace(ch.ToString(), String.Empty);
                staArr = staArr.Replace('\r', ' ');
                result = result + " ";
                result = result + staArr;

                ReadBalanceElement(out balanceElement);

                ReadTotalBetElement(out totalBetElement);
            }
            catch (Exception ex)
            {
             //   Global.log.addLog(ex.Message, Global.LOG_LEVEL_ONLYFILE);
             //   Global.log.addLog(ex.StackTrace, Global.LOG_LEVEL_ONLYFILE);
            }
            GotoParentFrame();
            return result;
        }

        public bool ReadBalanceElement(out IWebElement balanceEle)
        {
            bool result = false;
            balanceEle = null;
            try {

                ReadOnlyCollection<IWebElement> _elsBalanceLabel = driver.FindElements(By.CssSelector("[data-role='balance-label__value']"));
                if (_elsBalanceLabel.Count != 0)
                {
                    result = true;
                    balanceEle = _elsBalanceLabel[0];
                }
            }
            catch(Exception ex)
            {
                Global.log.addLog(ex.Message);
            }
            return result;
        }
        public bool ReadTotalBetElement(out IWebElement totalBetEle)
        {
            bool result = false;
            totalBetEle = null;
            try
            {

                ReadOnlyCollection<IWebElement> _elsTotalBetLabel = driver.FindElements(By.CssSelector("[data-role='total-bet-label']"));
                if (_elsTotalBetLabel.Count != 0)
                {
                    result = true;
                    totalBetEle = _elsTotalBetLabel[0];
                }
            }
            catch (Exception ex)
            {
                Global.log.addLog(ex.Message);
            }
            return result;
        }

        public bool ReadPlaceTimerElement(out IWebElement placeTimerEle)
        {
            bool result = false;
            placeTimerEle = null;
            try
            {
                ReadOnlyCollection < IWebElement > elePlace = driver.FindElements(By.CssSelector("[data-role='footer-circle-timer']"));
                if (elePlace != null && elePlace.Count != 0)
                {
                    result = true;
                    placeTimerEle = elePlace[0];
                }
            }
            catch (Exception ex)
            {
                Global.log.addLog(ex.Message);
            }
            return result;
        }

        public bool ReadPlaceTextElement(int nTableIndex, out IWebElement placeTextEle)
        {
            bool result = false;
            placeTextEle = null;
            try
            {
                if (SwitchTableFrameByIndex(nTableIndex) == false)
                {
                    Global.log.addLog("Failed to switch table frame", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }
                ReadOnlyCollection<IWebElement> elePlace = driver.FindElements(By.CssSelector("[data-role='traffic-light']"));
                if (elePlace.Count != 0)
                {
                    result = true;
                    placeTextEle = elePlace[0];
                }
            }
            catch(Exception ex)
            {
                Global.log.addLog(ex.Message);
            }
            GotoParentFrame();
            return result;
        }
        public void GetTimer()
        {
            try
            {
                IWebElement elTimer = driver.FindElement(By.Id("timer-time"));
                if (elTimer == null)
                {
                    Global.log.addLog("Failed to find [id='timer-time']", Global.LOG_LEVEL_ONLYFILE);
                    return;
                }

//              Global.log.addLog($"Timer {elTimer.Text}", Global.LOG_LEVEL_ONLYFILE);
            }
            catch (Exception ex)
            {
                Global.log.addLog($"GetTimer : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }
        }
        public string GetChipsFromStatus()
        {
            string result = "";
            try
            {
                ReadOnlyCollection<IWebElement> elsRecentchips = driver.FindElements(By.CssSelector("[data-role='chip-stack']"));
                string text = elsRecentchips[0].Text;
                if (text.Length > 0)
                {
                    string[] splits = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < splits.Count(); i++)
                        result = result + splits[i] + " ";
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return result;
        }

        public string GetBalanceFromStatus(IWebElement elBalance)
        {
            string result = "";
            try { 
                result = elBalance.Text; // Balance
            }
            catch(Exception ex)
            {

            }
            return result;
        }

        public string GetUpdateSpinsFromStatus(int nCheckNumStaticsOpen, IWebElement ele)
        {
            string result = "";
            try { 
                if (nCheckNumStaticsOpen % 50 == 0)
                { 
                    if (!CheckNumStatusOpen())
                    {
                        return result;
                    }

                    CheckUnMuteButton();

                    CheckPinCloseButton();

                    CloseTutorial();
                }

                CheckContinueButton();

                /*ReadOnlyCollection<IWebElement> elsRecentnumbers = driver.FindElements(By.CssSelector("[data-role='recent-number']"));
                if (elsRecentnumbers.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='recent-number']", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }
                IWebElement pa_rc = GetParent(elsRecentnumbers[0]);
                string pa_recent = pa_rc.Text;*/
                string pa_recent = ele.Text;
                char ch = '\n';
                pa_recent = pa_recent.Replace(ch.ToString(), String.Empty);
                pa_recent = pa_recent.Replace('\r', ' ');
                result = pa_recent;
            }
            catch(Exception ex)
            {
                Global.log.addLog("Spin Element has been changed");
                Global.log.addLog(ex.Message);
            }
            return result;
        }

        public bool CheckFrameContainerChange()
        {
            try {
                if (bLoadContainerUse)
                    return true;

                CheckFrameContainer();
                if (bLoadContainerUse)
                    driver.SwitchTo().Frame(nSingleFrameIndex);
                return true;
            }
            catch(Exception ex)
            {
            }
            return false;
        }

        public List<string> GetTableNames()
        {
            List<string> result = new List<string>();
            try
            {
                if (!bLoadContainerUse)
                {
                    driver.SwitchTo().Frame(nSingleFrameIndex);
                    getTableInfo(out result);
                    GotoParentFrame();
                }
                else
                {
                    int retry = 0;
                    while(++retry < 3)
                    {
                        ReadOnlyCollection<IWebElement> _elsFrameList = driver.FindElements(By.TagName("iframe"));
                        if (_elsFrameList.Count == 0)
                        {
                            InitParentFrame();
                            Global.log.addLog("Failed to find table frame tag", Global.LOG_LEVEL_ONLYFILE);
                            return result;
                        }

                        foreach (IWebElement elFrame in _elsFrameList)
                        {
                            driver.SwitchTo().Frame(elFrame);
                            List<string> table_one = new List<string>();
                            getTableInfo(out table_one);
                            result.AddRange(table_one);
                            GotoParentFrame();
                        }
                        if (result.Count == 0)
                        {
                            Global.log.addLog($"InitParentFrame because no table name {_elsFrameList.Count}");
                            InitParentFrame();
                        }
                        else
                            break;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                InitParentFrame();

                if (bLoadContainerUse)
                { 
                    ReadOnlyCollection<IWebElement> _elsFrameList = driver.FindElements(By.TagName("iframe"));
                    if (_elsFrameList.Count == 0)
                    {
                        return result;
                    }

                    foreach (IWebElement elFrame in _elsFrameList)
                    {
                        driver.SwitchTo().Frame(elFrame);
                        List<string> table_one = new List<string>();
                        getTableInfo(out table_one);
                        result.AddRange(table_one);
                        GotoParentFrame();
                    }
                }
            }
            return result;
        }

        public bool getTableInfo(out List<string> result)
        {
            bool result_flag = false;
            result = new List<string>();
            ReadOnlyCollection<IWebElement> elsRecentnumbers = driver.FindElements(By.CssSelector("[data-role='statistics-button']"));
            if (elsRecentnumbers.Count == 0)
            {
                return result_flag;
            }
            ReadOnlyCollection<IWebElement> _elsTableNames = driver.FindElements(By.CssSelector("[data-role='bet-limits-header']"));
            string removeLine = "\n";
            string name = _elsTableNames[0].Text.Replace(removeLine, "");
            removeLine = "\r";
            name = name.Replace(removeLine, "");
            result.Add(name);
            result_flag = true;
            return result_flag;
        }
        public bool CloseTable(string tableName)
        {
            try
            {
                ReadOnlyCollection<IWebElement> _elsClose = driver.FindElements(By.CssSelector("[data-role='close-button']"));
                if (_elsClose.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='close-button']", Global.LOG_LEVEL_ONLYFILE);
                    return false;
                }
                
                IWebElement elCloseTable = _elsClose[0];
                elCloseTable.Click();
                InitParentFrame();

                return true;
            }
            catch (Exception ex)
            {
                InitParentFrame();
                SwitchTableFrameByName(tableName);
                Global.log.addLog($"CloseTable : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }
            return false;
        }
        public void GotoRobby()
        {
            try
            {
                ReadOnlyCollection<IWebElement> _elsRobby = driver.FindElements(By.CssSelector("[data-role='lobby-button']"));
                if (_elsRobby.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='lobby-button']", Global.LOG_LEVEL_ONLYFILE);
                    return;
                }

                IWebElement elRobby = _elsRobby[0];
                elRobby.Click();
            }
            catch (Exception ex)
            {
                Global.log.addLog($"GotoRobby : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }
        }

        public void OpenActivatedTable()
        {
            try
            {
                ReadOnlyCollection<IWebElement> _elsTables = driver.FindElements(By.CssSelector("[data-role='table-block']"));
                if (_elsTables.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='table-block']", Global.LOG_LEVEL_ONLYFILE);
                    return;
                }
                foreach (IWebElement el in _elsTables)
                {
                    string className = el.GetAttribute("class");
                    if (className.Contains("occupied"))
                    {
                        el.Click();
                        return;
                    }
                }
                Global.log.addLog("Failed to find occupied table.", Global.LOG_LEVEL_ONLYFILE);
            }
            catch (Exception ex)
            {
                Global.log.addLog($"OpenActivatedTable : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }
        }

        public bool ClickActiveTable(string tableName)
        {
            ReadOnlyCollection<IWebElement> _elTableContainer = driver.FindElements(By.CssSelector("[data-role='table-block']"));
            if (_elTableContainer.Count == 0)
            {
                Thread.Sleep(100);
                return false;
            }
            foreach (IWebElement iEle in _elTableContainer)
            {
                string[] line = iEle.Text.Split('\n');
                foreach (string lineTemp in line)
                {
                    string[] lineTempArr = lineTemp.Split('\r');
                    string[] tableNameArr = tableName.Split('£');
                    if (tableNameArr[0].Length < 1)
                    {
                        continue;
                    }
                    if (tableNameArr[0] == lineTempArr[0])
                    {
                        IWebElement cl = iEle.FindElement(By.CssSelector("[class^='lobby-table-block-cnt']"));
                        String javascript = "arguments[0].dispatchEvent(new MouseEvent('click', {view: window, bubbles:true, cancelable: true}))";
                        IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                        executor.ExecuteScript(javascript, cl);
                        return true;
                    }
                }
            }

            return false;
        }
        public bool SetActiveTable(string tableName)
        {
            try 
            { 
                int retry = 0;
                bool IsaddClicked = false;
                while (++retry < 10)
                {
                    ReadOnlyCollection<IWebElement> gameFrames = driver.FindElements(By.TagName("iframe"));
                    if (gameFrames.Count == 0)
                    {
                        if (ClickActiveTable(tableName))
                        { 
                            Global.log.addLog($"{tableName} table added");
                            GotoParentFrame();
                            return true;
                        }
                        InitParentFrame();
                        return false;
                    }
                    else
                    {
                        int frameCount = gameFrames.Count;
                        for (int index = 0; index < frameCount; index++ )
                        {
                            driver.SwitchTo().Frame(index);
                            if (ClickActiveTable(tableName))
                            {
                                GotoParentFrame();
                                Global.log.addLog($"{tableName} table added");
                                return true;
                            }
                            if (!IsaddClicked && addNewTable())
                            {
                                IsaddClicked = true;
                                checkLobbyTableadded();
                                frameCount = frameCount + 1;
//                                Global.log.addLog($"frameCount: {frameCount}");
                            }
                            GotoParentFrame();
                        }
                    }
                    Thread.Sleep(1000);
                }
                Global.log.addLog($"{tableName} add table retry 10 times");
            }
            catch(Exception ex)
            {

            }

            return false;
        }

        public bool checkLobbyTableadded()
        {
            GotoParentFrame();
            ReadOnlyCollection<IWebElement> gameFrames = driver.FindElements(By.TagName("iframe"));
            if (gameFrames.Count == 0)
                return false;

            driver.SwitchTo().Frame(gameFrames.Last());

            int retry = 0;
            while(++retry < 5)
            { 
                ReadOnlyCollection<IWebElement> _elTableContainer = driver.FindElements(By.CssSelector("[data-role='table-block']"));
                if (_elTableContainer.Count != 0)
                {
                    return true;
                }
                Thread.Sleep(1000);
            }

            return false;
        }
        public bool addNewTable()
        {
            ReadOnlyCollection<IWebElement> _elTableContainer = driver.FindElements(By.CssSelector("[data-role='plus-table-button']"));
            if (_elTableContainer.Count == 0 || _elTableContainer[0].Displayed == false)
            {
                return false;
            }

            _elTableContainer[0].Click();
            return true;
        }
        public IWebElement GetParent(IWebElement node)
        {
            return node.FindElement(By.XPath(".."));
        }

        public bool SetFrameForPrepare(int nTableIndex)
        {
            bool result = false;
            try
            {
//                Global.log.addLog($"Table Switching Before");
                if (!SwitchTableFrameByIndex(nTableIndex))
                {
                    Global.log.addLog("Failed to switch table frame", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }
//                Global.log.addLog($"Table Switching After");
                CheckPinCloseButton();
                result = true;
            }
            catch (Exception ex)
            {
            //    Global.log.addLog(ex.Message, Global.LOG_LEVEL_ONLYFILE);
            //    Global.log.addLog(ex.StackTrace, Global.LOG_LEVEL_ONLYFILE);
            }
            return result;
        }
        public IWebElement getPlaceSpot(int nTableIndex, string betspotid)
        {
            IWebElement spot_element = null;
            try
            { 

                ReadOnlyCollection<IWebElement> _elsBettingGrid = driver.FindElements(By.CssSelector("[class^='betting-grid']"));
                if (_elsBettingGrid.Count == 0)
                {
                    Global.log.addLog("Failed to find betting-grid", Global.LOG_LEVEL_ONLYFILE);
                    return spot_element;
                }

                ReadOnlyCollection<IWebElement> _elsBetSpots = _elsBettingGrid[0].FindElements(By.CssSelector($"[data-bet-spot-id='{betspotid}']"));
                if (_elsBetSpots.Count == 0)
                {
                    Global.log.addLog("Failed to find chip", Global.LOG_LEVEL_ONLYFILE);
                    return spot_element;
                }

                spot_element = _elsBetSpots[0];
            }
            catch(Exception ex)
            {
                Global.log.addLog($"getPlaceSpot : {ex.Message}");

            }
            return spot_element;
        }
        public IWebElement getPlaceChip(string chipid)
        {
            try { 
                IWebElement result = null;

                ReadOnlyCollection<IWebElement> _elsChipStack = driver.FindElements(By.CssSelector("[data-role='chip-stack']"));
                if (_elsChipStack.Count == 0)
                {
                    Global.log.addLog("Failed to find chip-stack", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }
            
                string selectorParam = $"[data-role='chip'][data-value='{chipid}']";
                ReadOnlyCollection<IWebElement> _elsChips = driver.FindElements(By.CssSelector(selectorParam));
                if (_elsChips.Count == 0)
                {
                    Global.log.addLog($"Failed to find chip : {selectorParam}", Global.LOG_LEVEL_ONLYFILE);
                    return result;
                }

                IWebElement elChip = _elsChips[0];
                result = elChip;
                return result;
            }
            catch(Exception ex)
            {
                Global.log.addLog($"getPlaceChip : {ex.Message}");
            }
            return null;
        }

        public bool setPlaceChip(IWebElement elChip)
        {
            try {
                elChip.Click();
                
/*                String javascript = "arguments[0].dispatchEvent(new MouseEvent('click', {view: window, bubbles:true, cancelable: true}))";
                IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                executor.ExecuteScript(javascript, elChip);*/
                return true;
            }
            catch(Exception ex)
            {
                Global.log.addLog($"setPlaceChip : {ex.Message}");
            }
            return false;
        }

        public bool PlaceInfo(IWebElement spotId)
        {
            bool result = false;
            try {
                String javascript = "arguments[0].dispatchEvent(new MouseEvent('click', {view: window, bubbles:true, cancelable: true}))";
                IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                executor.ExecuteScript(javascript, spotId);
                result = true;
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Global.log.addLog(ex.StackTrace, Global.LOG_LEVEL_ONLYFILE);
            }

            return result;

        }

        public void CheckContinueButton()
        {
            ReadOnlyCollection<IWebElement> _elsBettingGrid = driver.FindElements(By.CssSelector("[data-role='button-continue']"));
            if (_elsBettingGrid.Count == 0)
            {
                return;
            }
            IWebElement elBetSpot = _elsBettingGrid[0];
            if (elBetSpot.Text == "CONTINUE PLAYING")
                elBetSpot.Click();
        }
        public void CheckUnMuteButton()
        {
            ReadOnlyCollection<IWebElement> _elsBettingGrid = driver.FindElements(By.CssSelector("[data-role='tooltip-label']"));
            if (_elsBettingGrid.Count == 0)
            {
                return ;
            }
            IWebElement elBetSpot = _elsBettingGrid[0];
            if (elBetSpot.Text.Contains("UNMUTE"))
                elBetSpot.Click();

        }
        public void CheckPinCloseButton(bool useDefaultFrame = false)
        {
            ReadOnlyCollection<IWebElement> _elsBettingGrid = driver.FindElements(By.CssSelector("[class='g1-pin-close']"));
            if (_elsBettingGrid.Count == 0)
            {
                return;
            }
            IWebElement elBetSpot = _elsBettingGrid[0];
            if (elBetSpot.Text.Contains("✕"))
            {
                String javascript = "arguments[0].dispatchEvent(new MouseEvent('click', {view: window, bubbles:true, cancelable: true}))";
                IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                executor.ExecuteScript(javascript, elBetSpot);
            }
        }
        public void CheckSessionReminderButton(bool useDefaultFrame = false)
        {
            ReadOnlyCollection<IWebElement> _elsBettingGrid = driver.FindElements(By.CssSelector("[class='custom-button']"));
            if (_elsBettingGrid.Count == 0)
            {
                return;
            }
            foreach(IWebElement elBetSpot in _elsBettingGrid)
            { 
                if (elBetSpot.Text.Contains("CONTINUE PLAYING"))
                {
                    elBetSpot.Click();
                }
            }
        }
        public bool SetFrameForUndo(int nTableIndex)
        {
            if (SwitchTableFrameByIndex(nTableIndex) == false)
            {
                Global.log.addLog("Failed to switch table frame", Global.LOG_LEVEL_ONLYFILE);
                return false;
            }

            return true;
        }

        public IWebElement getUndoElement()
        {
            IWebElement result = null;
            ReadOnlyCollection<IWebElement> _elsUndo = driver.FindElements(By.CssSelector($"[data-role='undo-button']"));
            if (_elsUndo.Count == 0)
            {
                Global.log.addLog("Failed to find chip", Global.LOG_LEVEL_ONLYFILE);
                return result;
            }
            result = _elsUndo[0];
            return result;
        }
        public int UndoInfo(IWebElement elUndo)
        {
            try
            {
                if (elUndo.GetAttribute("class").Contains("buttonStateDisabled"))
                {
                    return 0;
                }
                elUndo.Click();
                return 1;
            }
            catch (Exception ex)
            {
                Global.log.addLog($"UndoInfo : {ex.Message}", Global.LOG_LEVEL_ONLYFILE);
            }

            return -1;
        }
        public void CloseDrive()
        {
            if (driver != null)
            {
                try
                {
                    driver.Close();
                    driver.Quit();
                    driver = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CloseDrive : {ex.Message}");
                }
            }
        }
        
        public bool ClickStatisButton()
        {
            try
            {
                ReadOnlyCollection<IWebElement> _elsStatisticsButtons = driver.FindElements(By.CssSelector("[data-role='statistics-button']"));
                if (_elsStatisticsButtons.Count == 0)
                {
                    Global.log.addLog("Failed to find [data-role='statistics-button']", Global.LOG_LEVEL_ONLYFILE);
                    return false;
                }

                IWebElement elStatisticsButton = _elsStatisticsButtons[0];
                elStatisticsButton.Click();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }

    }
}
