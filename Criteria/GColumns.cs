using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class GColumns : ICriteria
    {
        private int SPINLIMIT = 23;
        private Dictionary<Global.BET_SPOT, int> sequence;
        List<Global.BET_SPOT> foundColumns = new List<Global.BET_SPOT>();
        public GColumns()
        {
            SetInitSequence();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            Global.BET_SPOT[] arrColumns = Global.GetAllColumns();
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);
            int repeat = 0;
            Global.BET_ITEMS result = new Global.BET_ITEMS();

            SetInitSequence();
            /*if (lstSpinAll[0] == Global.BET_SPOT.ZERO)
                return result;*/
            
            foundColumns.Clear();
            for (repeat = 0; repeat < arrStats.Length; repeat++)
            {
                Global.BET_SPOT elemenetColumn = Global.GetBetColumn(arrStats[repeat]);
                if (!foundColumns.Contains(elemenetColumn) && lstSpinAll[repeat] != Global.BET_SPOT.ZERO)
                { 
                    foundColumns.Add(elemenetColumn); 
                    sequence[elemenetColumn] = repeat;
                }
                if (foundColumns.Count == arrColumns.Length)
                {
                    break;
                }
            }

            List<Global.BET_SPOT> lstBetSpots = new List<Global.BET_SPOT>();
            List<decimal> lstBetStakes = new List<decimal>();
            for (int Index = 0; Index < foundColumns.Count; Index++)
            {
                if (sequence[foundColumns[Index]] < SPINLIMIT)
                    continue;

                decimal stake = GlobalData.GetStackAmountBySequence(sequence[foundColumns[Index]] - SPINLIMIT, startBettingAmount);
                stake = Math.Round(stake, 1);
                lstBetStakes.Add(stake);
                lstBetSpots.Add(foundColumns[Index]);
            }
            result.betStake = lstBetStakes.ToArray();
            result.placeCount = lstBetSpots.Count;
            result.arrBetSpots = lstBetSpots.ToArray();

            return result;
        }

        bool checkNonExist(Global.BET_SPOT param)
        {
            if (!foundColumns.Contains(param))
                return true;
            return false;
        }
        public string GetCriteriaName()
        {
            //return "G 1 Columns";
            return "Algorithm G";
        }
        public void SetInitSequence()
        {
            sequence = new Dictionary<Global.BET_SPOT, int>();
        }
    }
}
