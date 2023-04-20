using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveRoulette
{
    class TablesMonitor
    {
        public Dictionary<string, Table> m_lsTable;
        public List<Table> m_lsRemovedTable;
        public List<string> m_strTimeOutTableName;
        private static List<string> m_lsTableNames;
        private Dictionary<string, List<Global.BET_SPOT>> m_dicReopenedTableNumber;
        private Dictionary<string, List<Global.BET_SPOT>> m_dicClosedTableNumber;
        public int nPastTime = 0;
        public string m_strBackupBalance;
        public int nErrorCount = 0;
        public int nInitFrameStart = 0;

        public TablesMonitor()
        {
            m_lsTable = new Dictionary<string, Table>();
            m_dicClosedTableNumber = new Dictionary<string, List<Global.BET_SPOT>>();
            m_dicReopenedTableNumber = new Dictionary<string, List<Global.BET_SPOT>>();
            m_lsRemovedTable = new List<Table>();
            m_lsTableNames = new List<string>();
            m_strTimeOutTableName = new List<string>();
            Global.gbBalanceFlag = true;
            nPastTime = Environment.TickCount & Int32.MaxValue;
            m_strBackupBalance = "0";
        }
        public void Start()
        {
            Thread thTableMonitor = new Thread(ConnectTables);
            thTableMonitor.IsBackground = true;
            thTableMonitor.Start();
        }

        private void ConnectTables()
        {
            Global.m_nStartTableTick = Environment.TickCount & Int32.MaxValue;
            nInitFrameStart = 0;
            if (!Global.elementInterace.InitParentFrame())
            {
                Global.gAutoBetStatus = Global.AUTOBET_STATUS.AUTOBET_STOPPED;
                Global.log.addLog("Please Select Table", Global.LOG_LEVEL_LISTFILE);
                return;
            }

            while (Global.elementInterace.driver != null)
            {
                GetTablesFromWeb();
                GetTablesStatusFromWeb();
                Thread.Sleep(50);
            }
            m_lsRemovedTable.Clear();
            m_lsTable.Clear();
            m_lsTableNames.Clear();
        }

        private void CheckTableExist()
        {
            lock (m_lsTable)
            {
                lock (m_lsTableNames)
                {
                    string[] strKeysArr = m_lsTable.Keys.ToArray();
                    string[] strNonExistTableNames = Array.FindAll(strKeysArr, CheckNonExistTableName).ToArray();
                    foreach (string removeTableKey in strNonExistTableNames)
                    {
                        m_lsRemovedTable.Add(m_lsTable[removeTableKey]);
                        m_lsTable.Remove(removeTableKey);
                        m_lsTableNames.Remove(removeTableKey);
                    }
                }
            }
        }

        private void GetTablesFromWeb()
        {
            try
            {
                lock (m_lsTable)
                {
                    lock (m_lsTableNames)
                    {
                        m_lsTableNames.Clear();

                        if (!Global.elementInterace.CheckFrameContainerChange())
                            return;

                        if (m_strTimeOutTableName.Count != 0)
                        {
                            int index = 0;
                            for (index = 0; index < m_strTimeOutTableName.Count; index++)
                            {
                                string timeoutName = m_strTimeOutTableName.ElementAt(index);
                                if (Global.elementInterace.SetActiveTable(timeoutName))
                                {
                                    Global.log.addLog($"{timeoutName} table reopened successful");
                                    m_strTimeOutTableName.Remove(timeoutName);
                                    m_dicReopenedTableNumber[timeoutName] = m_dicClosedTableNumber[timeoutName];
                                    m_dicClosedTableNumber.Remove(timeoutName);

                                    index--;
                                }
                            }
                        }
                        m_lsTableNames = Global.elementInterace.GetTableNames();

                        string table_names = "Table List:";
                        int iCount;
                        for (iCount = 0; iCount < m_lsTableNames.Count; iCount++)
                        {
                            table_names = table_names + m_lsTableNames.ElementAt(iCount) + ",";
                            if (m_lsTableNames.ElementAt(iCount).Length < 1)
                                continue;
                            if (!m_lsTable.ContainsKey(m_lsTableNames[iCount]))
                            {
                                Table newTable;
                                if ((newTable = ReadAllSpinsFromTable(m_lsTableNames[iCount])) != null)
                                {
                                    if (ReadAllSpotsFromTable(newTable))
                                        m_lsTable.Add(m_lsTableNames[iCount], newTable);
                                }
                            }
                        }

                        int nCurrentTick = Environment.TickCount & Int32.MaxValue; 
                        if (nCurrentTick - nInitFrameStart > 1000 * 30)
                        {
                            Global.elementInterace.InitParentFrame();
                            nInitFrameStart = nCurrentTick;
                        }
                        //                        Global.log.addLog(table_names);
                        nErrorCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                if (nErrorCount == 0)
                    Global.log.addLog($"GetTablesFromWeb : {ex.Message}");

                nErrorCount++;
            }
        }

        private void GetTablesStatusFromWeb()
        {
            int iCount;
            CheckTableExist();
            DateTime startT = DateTime.Now;
            //            for (iCount = 0; iCount < m_lsTable.Count; iCount++)
            try {
                for (iCount = 0; iCount < m_lsTable.Count; iCount++)
                {
                    Table table = m_lsTable.ElementAt(iCount).Value;
                    int nNameOrderIndex = m_lsTableNames.FindIndex(table.m_strTableName.StartsWith);
                    if (Global.elementInterace.SetFrameForPrepare(nNameOrderIndex))
                    {
                        //                    if (Global.elementInterace.CheckPlaceInfoText(table.m_placeInfoTextElement) ||
                        //                        Global.elementInterace.CheckPlaceInfoTime(table.m_placeInfoTimeElement))
                        {
                            string updateSpins = Global.elementInterace.GetUpdateSpinsFromStatus(table.m_nCheckStaticsOpenCnt, table.m_SpinElement);
                            
                            if (table.IsUpdatedSpins(updateSpins))
                            {
                                // 
                                string strBalance = Global.elementInterace.GetBalanceFromStatus(table.m_balanceElement);
                                if (strBalance != "")
                                    GetBalanceFromWeb(strBalance);
                                if (strBalance != "")
                                {
                                    table.setBetSignal();
                                    table.m_nTimeoutSpin++;
                                }
                                if (m_strBackupBalance != strBalance)
                                {
                                    Global.log.addLog($"Balance : {m_strBackupBalance} -> {strBalance}");
                                    m_strBackupBalance = strBalance;
                                }
                                table.m_dPlacedCnt = 0;

                            }
                            else if (updateSpins != "")
                            {
                                int nCurrentTickTime = Environment.TickCount & Int32.MaxValue;
                                if (nCurrentTickTime - table.m_nSignalOccuredTickTime < 1000 * 20)
                                    table.setBetSignal(false);
                                else
                                {
                                    if (table.m_SignalResult.placeCount != 0)
                                    {
                                        Global.log.addLog($"{table.m_strTableName} signal closed");
                                    }
                                    table.m_SignalResult.placeCount = 0;
                                }

                                /*
                                string strTotalBet = Global.elementInterace.GetBalanceFromStatus(table.m_totalbetElement);
                                if (strTotalBet != "")
                                { 
                                    string totalValue = strTotalBet.Split(' ').Last();
                                    table.m_dPlacedCnt = double.Parse(totalValue);
                                }*/
                            }
                            else
                            {
                                m_lsTable.Remove(table.m_strTableName);
                                m_lsTableNames.Remove(table.m_strTableName);
                                CheckTableExist();
                                iCount--;
                            }

                            //Global.elementInterace.ReadPlaceTimerElement(out table.m_placeInfoTimeElement);
                        }
                        table.checkTimeoutUndo();
                        if (!ExistPlacedTable())
                        { 
                            if (table.checkTimeoutTable())
                            {
                                Global.log.addLog($"{table.m_strTableName} table closed");
                                if (!m_strTimeOutTableName.Exists(x => x == table.m_strTableName))
                                    m_strTimeOutTableName.Add(table.m_strTableName);
                                m_dicClosedTableNumber[table.m_strTableName] = table.m_lstNumbers;
                                return;
                            }
                        }

                        Global.elementInterace.GotoParentFrame();

                    }
                    table.m_nCheckStaticsOpenCnt++;
                }
            }
            catch(Exception ex)
            {

            }
        }

        public bool ExistPlacedTable()
        {
            int iCount;
            double sumPlacedCnt = 0;
            for (iCount = 0; iCount < m_lsTable.Count; iCount++)
            {
                Table table = m_lsTable.ElementAt(iCount).Value;
                sumPlacedCnt += table.m_dPlacedCnt;
            }

            if (sumPlacedCnt > 0)
                return true;

            return false;
        }
        private Table ReadAllSpinsFromTable(string strTableName)
        {
            int nNameOrderIndex = m_lsTableNames.FindIndex(strTableName.StartsWith);
            int nIndex = GetTableUIIndex();
            if (nIndex == -1)
                return null;
            Table newTable = new Table(strTableName, nIndex);

            string strResponse;
            int nRetry = 0;

            if (m_dicReopenedTableNumber.ContainsKey(strTableName))
            {
                newTable.m_lstNumbers = m_dicReopenedTableNumber[strTableName];
                newTable.m_nChangedSpinCount = newTable.m_lstNumbers.Count;
                //      Global.log.addLog($"[{newTable.m_strTableName}] Loaded Closed Table Spins", Global.LOG_LEVEL_LISTFILE);
                m_dicReopenedTableNumber.Remove(strTableName);
                return newTable;

            }
            Global.log.addLog($"[{newTable.m_strTableName}] Loading All Spins", Global.LOG_LEVEL_LISTFILE);
            while (++nRetry < 3)
            {
                char c = ' ';
                IWebElement spinElement;
                IWebElement balanceElement;
                IWebElement totalBetElement;
                IWebElement placeTextElement;
                strResponse = Global.elementInterace.ReadStatistics(nNameOrderIndex, 1, out spinElement, out balanceElement, out totalBetElement);
                if (Global.elementInterace.ReadPlaceTextElement(nNameOrderIndex, out placeTextElement))
                    newTable.m_placeInfoTextElement = placeTextElement;
                if (strResponse.Count(f => (f == c)) > 200)
                {
                    newTable.setAllSpins(strResponse);
                    newTable.m_SpinElement = spinElement;
                    newTable.m_balanceElement = balanceElement;
                    newTable.m_totalbetElement = totalBetElement;
                    return newTable;
                }
            }

            Global.log.addLog($"[{newTable.m_strTableName}] Loading Spins Failed", Global.LOG_LEVEL_LISTFILE);
            return null;
        }

        private bool ReadAllSpotsFromTable(Table table)
        {
            int nNameOrderIndex = m_lsTableNames.FindIndex(table.m_strTableName.StartsWith);
            List<Global.BET_SPOT> lst_all_spots = new List<Global.BET_SPOT>();
            lst_all_spots.AddRange(Global.GetAllNumbers());
            lst_all_spots.AddRange(Global.GetAllColors());
            lst_all_spots.AddRange(Global.GetAllColumns());
            lst_all_spots.AddRange(Global.GetAllDozens());
            lst_all_spots.AddRange(Global.GetAllHalfs());
            lst_all_spots.AddRange(Global.GetAllOddEven());

            table.m_SpotElements.Clear();
            if (Global.elementInterace.SetFrameForPrepare(nNameOrderIndex))
            {
                for (int i = 0; i < lst_all_spots.Count; i++)
                {
                    string spot_name = Global.GetBetCssSpotID(lst_all_spots[i]);
                    IWebElement element = Global.elementInterace.getPlaceSpot(nNameOrderIndex, spot_name);
                    if (element == null)
                    {
                        Global.elementInterace.GotoParentFrame();
                        Global.log.addLog("Get Place Spot is null");
                        return false;
                    }

                    table.m_SpotElements.Add(lst_all_spots[i], element);
                }
                Global.elementInterace.GotoParentFrame();
            }
            Global.log.addLog($"table={table.m_strTableName}, Index={nNameOrderIndex}, m_SpotElements={table.m_SpotElements.Count}");
            return true;
        }

        private bool CheckNonExistTableName(string tableName)
        {
            if (!m_lsTableNames.Contains(tableName))
                return true;
            return false;
        }

        private int GetTableUIIndex()
        {
            int nResult = 0;
            int[] nZeroArr = new int[4] { 0, 0, 0, 0 };
            for (int iIndex = 0; iIndex < m_lsTable.Count; iIndex++)
            {
                Table tb = m_lsTable.ElementAt(iIndex).Value;
                nZeroArr[tb.m_nTableListIndexUI] = 1;
            }

            nResult = Array.IndexOf(nZeroArr, 0);
            return nResult;
        }
        private void GetBalanceFromWeb(string startBalance)
        {
            decimal currentBalance;
            if (decimal.TryParse(startBalance, out currentBalance) == false)
            {
                Global.log.addLog("Get Balance Failed", Global.LOG_LEVEL_LISTFILE);
                return;
            }

            GlobalData.SetCurrentBalance(currentBalance);
            GlobalData.SetStartBalance(currentBalance);
        }

        public void TestFunc(int mode)
        {
            if (mode == 1)
            {
                Global.elementInterace.SetFrameForPrepare(0);
                Global.elementInterace.PlaceInfo(m_lsTable.ElementAt(0).Value.m_SpotElements.ElementAt(1).Value);
                Global.elementInterace.GotoParentFrame();
            }
            else if (mode == 2)
            {
                IWebElement ele = null;
                Global.elementInterace.SetFrameForPrepare(0);
                Global.elementInterace.PlaceInfo(ele);
                Global.elementInterace.GotoParentFrame();
            }
            else if (mode == 3)
            {
                Global.elementInterace.SetFrameForPrepare(0);
                Global.elementInterace.PlaceInfo(m_lsTable.ElementAt(1).Value.m_SpotElements.ElementAt(1).Value);
                Global.elementInterace.GotoParentFrame();
            }
        }
    }
}
