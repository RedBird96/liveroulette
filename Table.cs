//#define UNDO_MODE
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LiveRoulette.Global;

namespace LiveRoulette
{
    
    class Table
    {
        public string m_strTableName;
        public List<Global.BET_SPOT> m_lstNumbers;
        public int m_nChangedSpinCount;
        private int LASTSPINARRYCNT = 30;
        private int TIMEOUTLIMIT_SPIN = 50;
        private int TIMEOUTLIMIT_TIME = 25;
        private int TIMEOUTLIMIT_UNDO_SPIN = 15;
        public int m_nTableListIndexUI;
        public List<decimal> m_lstChipArr;
        public int m_nTableIndex;
        public List<Criteria.ICriteria> m_lstCriterias;
        public Global.BET_ITEMS m_SignalResult;
        public Dictionary<Global.BET_SPOT, IWebElement> m_SpotElements;
        public Dictionary<decimal, IWebElement> m_ChipElements;
        public IWebElement m_UndeElement;
        public IWebElement m_SpinElement;
        public IWebElement m_balanceElement;
        public IWebElement m_totalbetElement;
        public IWebElement m_placeInfoTimeElement;
        public IWebElement m_placeInfoTextElement;
        public bool m_OneTimeFlag = false;
        public int m_npreBettingCriteria = -1;

        private decimal m_BettingBalance;
        public Dictionary<string, decimal> m_IsCriteriaBetted;

        public int m_nCheckStaticsOpenCnt;
        public int m_nTimeoutSpin;
        public int m_nTimeUndoSpin;
        public int m_nSignalOccuredTickTime;
        public double m_dPlacedCnt;
        public Table(string strName, int nTableListIndex)
        {
            m_strTableName = strName;
            m_nTableListIndexUI = nTableListIndex;
            m_lstNumbers = new List<Global.BET_SPOT>();
            m_lstChipArr = new List<decimal>();
            m_lstCriterias = new List<Criteria.ICriteria>();
            m_SpotElements = new Dictionary<Global.BET_SPOT, IWebElement>();
            m_ChipElements = new Dictionary<decimal, IWebElement>();
            m_IsCriteriaBetted = new Dictionary<string, decimal>();
            m_nChangedSpinCount = 0;
            m_nTableIndex = 0;
            m_nTimeoutSpin = 0;
            m_nTimeUndoSpin = m_nTimeoutSpin;
            m_nCheckStaticsOpenCnt = 0;
            m_SignalResult.placeCount = 0;
            m_dPlacedCnt = 0;
            m_BettingBalance = 0;
            initCriteria();
        }

        public void initCriteria()
        {
            m_lstCriterias.Add(new Criteria.ANumbers());
            m_lstCriterias.Add(new Criteria.BConsecutiveNumbers());
            m_lstCriterias.Add(new Criteria.CColors());
            m_lstCriterias.Add(new Criteria.DOddEven());
            m_lstCriterias.Add(new Criteria.EHalfs());
            m_lstCriterias.Add(new Criteria.FDozen());
            m_lstCriterias.Add(new Criteria.GColumns());
            m_lstCriterias.Add(new Criteria.H2Dozens());
            m_lstCriterias.Add(new Criteria.I2Columns());
            m_lstCriterias.Add(new Criteria.JConsecutiveRB());
            m_lstCriterias.Add(new Criteria.KConsecutiveColours());
            m_lstCriterias.Add(new Criteria.LConsecutiveOE());
            m_lstCriterias.Add(new Criteria.MConsecutiveFS());
            m_lstCriterias.Add(new Criteria.NConsecutiveDozen());
            m_lstCriterias.Add(new Criteria.OConsecutiveDozenIndividual());
            m_lstCriterias.Add(new Criteria.PConsecutiveColumn());
            m_lstCriterias.Add(new Criteria.QConsecutiveColumnIndividual());

            foreach(Criteria.ICriteria cri in m_lstCriterias)
            {
                m_IsCriteriaBetted.Add(cri.GetCriteriaName(), 0.0m);
            }
        }

        public void setAllSpins(string strSpinAll)
        {
            string[] statusone = strSpinAll.Split(' ');
            m_lstNumbers.Clear();
            foreach (string one in statusone)
            {
                if (one.Length < 1)
                    continue;
                int number_one;
                if (int.TryParse(one, out number_one) == false)
                    continue;
                m_lstNumbers.Add((Global.BET_SPOT)number_one);
            }
            m_nChangedSpinCount = m_lstNumbers.Count;

            Global.log.addLog($"[{m_strTableName}] Loaded All Spins", Global.LOG_LEVEL_LISTFILE);

        }

        public bool IsUpdatedSpins(string strSpinUpdate)
        {
            string spinLine = strSpinUpdate;
            string[] statusone = spinLine.Split(' ');
            int diffSpinCnt = getDiffStringArray(spinLine);
            if (diffSpinCnt == 0)
                return false;

            for (int i = diffSpinCnt - 1; i >= 0; i--)
            {
                int statusone_num;
                if (statusone[i].Length < 1)
                    continue;
                if (int.TryParse(statusone[i], out statusone_num) == false)
                    continue;

                m_lstNumbers.Insert(0, (Global.BET_SPOT)statusone_num);
                m_lstNumbers.RemoveAt(m_lstNumbers.Count - 1);
            }
            m_nChangedSpinCount = diffSpinCnt;
            Global.log.addLog($"[{m_strTableName} - {m_nTimeoutSpin}]" + getLastSpinsFromArray(LASTSPINARRYCNT), Global.LOG_LEVEL_ONLYFILE);
            return true;
        }

        private int getDiffStringArray(string spinLine)
        {
            int result = 0, i;
            List<int> spinLinelist, spinArrlist;
            string[] spinLineArr = spinLine.Split(' ');
            spinLinelist = new List<int>();
            spinArrlist = new List<int>();
            if (m_lstNumbers.Count < spinLineArr.Length)
                return 0;
            for (i = 0; i < spinLineArr.Length - 1; i++)
            {
                int lineArr_num;
                if (int.TryParse(spinLineArr[i], out lineArr_num) == false)
                    continue;
                spinLinelist.Add(lineArr_num);

                spinArrlist.Add((int)m_lstNumbers[i]);
            }
            result = 0;
            while (spinLinelist.Count != 0)
            {
                bool containsSameSequence = spinArrlist.Where((item, index) => index <= spinArrlist.Count - spinLinelist.Count).Select((item, index) => spinArrlist.Skip(index).Take(spinLinelist.Count)).Any(part => part.SequenceEqual(spinLinelist));
                if (containsSameSequence)
                    break;
                spinLinelist.RemoveAt(0);
                result++;
            }

            return result;
        }


        private string getLastSpinsFromArray(int lastCount)
        {
            string spintosavefile = "";
            if (m_lstNumbers.Count < lastCount)
                return spintosavefile;
            spintosavefile = "SPIN :";
            for (int i = 0; i < lastCount; i++)
            {
                if (i != 0)
                    spintosavefile += ",";
                spintosavefile = spintosavefile + ((int)m_lstNumbers[i]).ToString();
            }

            return spintosavefile;
        }

        public void setBetSignal(bool checkSignal = true)
        {
            try
            {
                if (Global.gAutoBetStatus == Global.AUTOBET_STATUS.AUTOBET_STARTED && !Global.gbDailyReachedStatus)
                { 
                    if (checkSignal)
                        m_SignalResult = checkBetCondition();

                    if (m_SignalResult.placeCount != 0)
                    {
                        if (checkSignal)
                            m_nSignalOccuredTickTime = Environment.TickCount & Int32.MaxValue;

                        if (!PreparePlace())
                        {
                            Global.log.addLog($"{m_strTableName} no chip data so signal skipped ");
                            return;
                        }

                        if (m_ChipElements[m_lstChipArr[0]].Displayed)
                        {
                            setPlace(m_SignalResult);
//#if (UNDO_MODE)
//                            Thread.Sleep(1000);
//                            undoPlace();
//#endif
                            m_SignalResult.placeCount = 0;
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Global.log.addLog($"setBetSignal : {ex.Message}");
            }
        }
        private bool PreparePlace()
        {
            if (m_lstChipArr.Count != 0)
                return true;

            string strChipsData = Global.elementInterace.GetChipsFromStatus();
            if (strChipsData.Length == 0)
                return false;
            setChips(strChipsData);
            return true;

        }
        private void setPlace(Global.BET_ITEMS placeParam)
        {
            int i, j;
            m_lstChipArr.Sort();
            for (i = m_lstChipArr.Count - 1; i >= 0 ; i--)
            {
                if (placeParam.betStake.Max() < m_lstChipArr[i])
                    continue;

                Global.elementInterace.setPlaceChip(m_ChipElements[m_lstChipArr[i]]);
                for (j = 0; j < placeParam.placeCount;j ++)
                {
                    string strSpotId = Global.GetBetCssSpotID(placeParam.arrBetSpots[j]);
                    decimal totalBetStake = placeParam.betStake[j];
                    while(totalBetStake > 0m)
                    {
                        totalBetStake -= m_lstChipArr[i];
                        if (totalBetStake < 0m)
                            break;
                        placeParam.betStake[j] = totalBetStake;

                        if (Global.gbIsSimulationMode)
                        {
                            Global.log.addLog($"[{m_strTableName}] Simulation Spot : {strSpotId}, Stake : {m_lstChipArr[i]}", Global.LOG_LEVEL_ONLYFILE);
                        }
                        else 
                        {
                            Global.log.addLog($"[{m_strTableName}] Spot : {strSpotId}, Stake : {m_lstChipArr[i]}", Global.LOG_LEVEL_ONLYFILE);
                            if (m_SpotElements.Count == 0 || m_SpotElements[placeParam.arrBetSpots[j]] == null)
                            {
                                Global.log.addLog($"Spot Element Count : {m_SpotElements.Count} or Spot Element is null before PlaceInfo");
                            }
                            if (!Global.elementInterace.PlaceInfo(m_SpotElements[placeParam.arrBetSpots[j]]))
                            {
                                Global.log.addLog($"[{m_strTableName}] {strSpotId} Place operation is failed", Global.LOG_LEVEL_LISTFILE);
                            }
                            else
                            {
                                m_nStartTableTick = Environment.TickCount & Int32.MaxValue;
                                m_nTimeoutSpin = 0;
                                m_nTimeUndoSpin = m_nTimeoutSpin;
                                m_dPlacedCnt = 1;
                            }
                                
                        }
                    } 
                }
            }

        }
        private void undoPlace(bool forceUndo = false)
        {
            if (Global.gbIsSimulationMode && !forceUndo)
                return;
            
            int unRes;
            do
            {
                unRes = Global.elementInterace.UndoInfo(m_UndeElement);
                if (unRes == -1)
                    Global.log.addLog($"[{m_strTableName}] Undo operation is failed", Global.LOG_LEVEL_ONLYFILE);
                else if (unRes == 0)
                    Global.log.addLog($"[{m_strTableName}] Undo Disabled", Global.LOG_LEVEL_ONLYFILE);
                else if (unRes == 1)
                    Global.log.addLog($"[{m_strTableName}] Undo Clicked", Global.LOG_LEVEL_ONLYFILE);

            } while (unRes == 1);
        }

        private Global.BET_ITEMS checkBetCondition()
        {
            decimal currentBalance;
            Global.BET_ITEMS result = new Global.BET_ITEMS();
            if (m_lstNumbers.Count == 0)
                return result;

            currentBalance = GlobalData.GetCurrentBalance();
            if (currentBalance == 0)
                return result;

            /*if (GlobalData.GetDailyTarget() <= currentBalance - GlobalData.GetStartBalance() && Global.gbIsSimulationMode == false)
            {
                Global.gbDailyReachedStatus = true;
                return result;
            }*/

            for (int nAlgoIndex = 0; nAlgoIndex < m_lstCriterias.Count; nAlgoIndex++)
            {
                decimal startbettingAmount = m_IsCriteriaBetted[m_lstCriterias[nAlgoIndex].GetCriteriaName()];
                if (m_IsCriteriaBetted[m_lstCriterias[nAlgoIndex].GetCriteriaName()] == 0.0m)
                    startbettingAmount = GlobalData.GetCurrentBalance();

                Global.BET_ITEMS algo_result = m_lstCriterias[nAlgoIndex].CheckBetPlaces(m_lstNumbers, startbettingAmount);
                if (algo_result.placeCount == 0)
                {
                    m_IsCriteriaBetted[m_lstCriterias[nAlgoIndex].GetCriteriaName()] = 0.0m;
                    continue;
                }

                m_IsCriteriaBetted[m_lstCriterias[nAlgoIndex].GetCriteriaName()] = GlobalData.GetCurrentBalance();

                string betSpotsname = "";
                for (int j = 0; j < algo_result.arrBetSpots.Count(); j++)
                {
                    if (j != 0)
                        betSpotsname += ",";
                    betSpotsname += Global.GetBetSpotString(algo_result.arrBetSpots[j]);
                }
                Global.log.addLog($"[{m_strTableName}] {m_lstCriterias[nAlgoIndex].GetCriteriaName()}, SpotCount {algo_result.placeCount} ({betSpotsname}), TotalStake {algo_result.betStake.Sum()}", Global.LOG_LEVEL_LISTFILE);


                result.placeCount += algo_result.placeCount;

                var listStakeMerge = new List<decimal>();
                var listSpotMerge = new List<Global.BET_SPOT>();
                if (result.betStake != null)
                    listStakeMerge.AddRange(result.betStake);
                listStakeMerge.AddRange(algo_result.betStake);

                if (result.arrBetSpots != null)
                    listSpotMerge.AddRange(result.arrBetSpots);
                listSpotMerge.AddRange(algo_result.arrBetSpots);
                result.betStake = listStakeMerge.ToArray();
                result.arrBetSpots = listSpotMerge.ToArray();
            }

            if (result.betStake != null && currentBalance < result.betStake.Sum() && Global.gbIsSimulationMode == false)
            {
                if(result.arrBetSpots.Length > 1 || currentBalance == 0)
                { 
                    result = new Global.BET_ITEMS();
                    if (Global.gbBalanceFlag)
                        Global.log.addLog("Balance is not enough so stopped auto betting", Global.LOG_LEVEL_LISTFILE);
                    Global.gbBalanceFlag = false;
                }
                else
                {
                    result.betStake[0] = currentBalance;
                }
            }
                
            Global.gbBalanceFlag = true;

            return result;
        }

        public void setChips(string strChipArray)
        {
            int i;
            if (strChipArray.Length == 0)
            {
                if (strChipArray.Length == 0)
                    Global.log.addLog($"Get Chip Array failed {m_strTableName}");
                return;
            }

            string[] chipsdata = strChipArray.Split(' ');
            for (i = 0; i < chipsdata.Length; i++)
            {
                if (chipsdata[i].Length < 1)
                    continue;
                decimal chipsdata_dec;
                if (decimal.TryParse(chipsdata[i], out chipsdata_dec) == false)
                    continue;
                m_lstChipArr.Add(chipsdata_dec);
            }

            for (i = 0; i < m_lstChipArr.Count; i++)
            {
                string strStake_Spin;
                if (m_lstChipArr[i] < 1m)
                    strStake_Spin = m_lstChipArr[i].ToString("N1");
                else
                    strStake_Spin = Convert.ToInt32(m_lstChipArr[i]).ToString();
                IWebElement element =  Global.elementInterace.getPlaceChip(strStake_Spin);
                if (element == null)
                {
                    m_lstChipArr.Clear();
                    Global.log.addLog($"Get Chip failed {m_strTableName}");
                    return;
                }
                m_ChipElements.Add(m_lstChipArr[i], element);
            }

            m_UndeElement = Global.elementInterace.getUndoElement();
        }

        public bool checkTimeoutTable()
        {
            int nCurrentTickTime = Environment.TickCount & Int32.MaxValue; 
            if (m_nTimeoutSpin >= TIMEOUTLIMIT_SPIN || nCurrentTickTime - m_nStartTableTick > 1000 * 60 * TIMEOUTLIMIT_TIME)
            {
                if (m_nTimeoutSpin >= TIMEOUTLIMIT_SPIN)
                {
                    Global.log.addLog($"{m_strTableName} table closed because spin count limited");
                }
                if (nCurrentTickTime - m_nStartTableTick > 1000 * 60 * TIMEOUTLIMIT_TIME)
                {
                    Global.log.addLog($"{m_strTableName} table closed because session expiration time");
                }

                if (!Global.elementInterace.CloseTable(m_strTableName))
                {
                    Global.log.addLog($"{m_strTableName} table closed failed");
                    return false;
                }
                    
                Global.log.addLog($"{m_strTableName} table closed successful");
                m_nStartTableTick = nCurrentTickTime;
                m_nTimeoutSpin = 0;
                m_nTimeUndoSpin = m_nTimeoutSpin;
                return true;
            }
            return false;
        }

        public bool checkTimeoutUndo()
        {
            if (m_nTimeoutSpin - m_nTimeUndoSpin> TIMEOUTLIMIT_UNDO_SPIN)
            {
                if (!PreparePlace())
                {
                    Global.log.addLog($"{m_strTableName} no chip data so signal skipped ");
                    return false;
                }

                Global.elementInterace.setPlaceChip(m_ChipElements.ElementAt(0).Value);
                if (Global.elementInterace.PlaceInfo(m_SpotElements[Global.BET_SPOT.COLOR_RED]))
                    undoPlace(true);

                m_nTimeUndoSpin = m_nTimeoutSpin;
            }
            return true;
        }
    }
}
