using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class FDozen : ICriteria
    {
        private int SPINLIMIT = 23;
        private Dictionary<Global.BET_SPOT, int> sequence;
        List<Global.BET_SPOT> foundDozens = new List<Global.BET_SPOT>();
        public FDozen()
        {
            SetInitSequence();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            Global.BET_SPOT[] arrDozens = Global.GetAllDozens();
            int repeat = 0;
            Global.BET_ITEMS result = new Global.BET_ITEMS();
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);

            SetInitSequence();
            /*if (lstSpinAll[0] == Global.BET_SPOT.ZERO)
                return result;*/
            
            foundDozens.Clear();
            for (repeat = 0; repeat < lstSpinAll.Count; repeat++)
            {
                Global.BET_SPOT elemenetDozen = Global.GetBetDozen(lstSpinAll[repeat]);
                if (!foundDozens.Contains(elemenetDozen) && lstSpinAll[repeat] != Global.BET_SPOT.ZERO)
                { 
                    foundDozens.Add(elemenetDozen);
                    sequence[elemenetDozen] = repeat;
                }
                if (foundDozens.Count == arrDozens.Length)
                {
                    break;
                }
            }

            List<Global.BET_SPOT> lstBetSpots = new List<Global.BET_SPOT>();
            List<decimal> lstBetStakes = new List<decimal>();
            for (int Index = 0; Index < foundDozens.Count; Index ++)
            {
                if (sequence[foundDozens[Index]] < SPINLIMIT)
                    continue;

                decimal stake = GlobalData.GetStackAmountBySequence(sequence[foundDozens[Index]] - SPINLIMIT, startBettingAmount);
                stake = Math.Round(stake, 1);
                lstBetStakes.Add(stake);
                lstBetSpots.Add(foundDozens[Index]);
            }
            
            result.betStake = lstBetStakes.ToArray();
            result.placeCount = lstBetStakes.Count;
            result.arrBetSpots = lstBetSpots.ToArray();

            return result;
        }

        bool checkNonExist(Global.BET_SPOT param)
        {
            if (!foundDozens.Contains(param))
                return true;
            return false;
        }
        public string GetCriteriaName()
        {
            //return "F 1 Dozen";
            return "Algorithm F";
        }
        public void SetInitSequence()
        {
            sequence = new Dictionary<Global.BET_SPOT, int>();
        }
    }
}
