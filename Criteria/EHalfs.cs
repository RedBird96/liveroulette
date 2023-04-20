using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class EHalfs : ICriteria
    {
        private Dictionary<Global.BET_SPOT, int> sequence;
        private int SPINLIMIT = 11;
        private decimal paststake = 0;
        public EHalfs()
        {
            SetInitSequence();
        }

        public string GetCriteriaName()
        {
            //return "E 1-18/19-36";
            return "Algorithm E";
        }

        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            int repeat = 0;
            Global.BET_ITEMS result = new Global.BET_ITEMS();
            Global.BET_SPOT[] arrHalfs = Global.GetAllHalfs();
            List<Global.BET_SPOT> lstFoundHalfs = new List<Global.BET_SPOT>();

            SetInitSequence();
            for (repeat = 0; repeat < lstSpinAll.Count; repeat++)
            {
                Global.BET_SPOT elementHalf = Global.GetBetHalf(lstSpinAll[repeat]);
                if (!lstFoundHalfs.Contains(elementHalf) && lstSpinAll[repeat] != Global.BET_SPOT.ZERO)
                {
                    lstFoundHalfs.Add(elementHalf);
                    sequence[elementHalf] = repeat;
                }
                if (lstFoundHalfs.Count == arrHalfs.Length)
                    break;
            }

            List<Global.BET_SPOT> lstBetSpots = new List<Global.BET_SPOT>();
            List<decimal> lstBetStakes = new List<decimal>();
            for (int Index = 0; Index < lstFoundHalfs.Count; Index++)
            {
                if (sequence[lstFoundHalfs[Index]] < SPINLIMIT)
                    continue;

                decimal stake = 0;
                if (paststake == 0)
                    stake = GlobalData.GetStartStake(startBettingAmount) * (decimal)Math.Pow(2, sequence[lstFoundHalfs[Index]] - SPINLIMIT);
                else
                    stake = paststake * 2;
                stake = Math.Round(stake, 1);
                lstBetStakes.Add(stake);
                lstBetSpots.Add(lstFoundHalfs[Index]);

            }
            result.betStake = lstBetStakes.ToArray();
            result.placeCount = lstBetSpots.Count;
            result.arrBetSpots = lstBetSpots.ToArray();

            if (result.placeCount == 0)
            {
                paststake = 0;
            }
            else
            {
                paststake = result.betStake[0];
            }
            return result;
        }
        public void SetInitSequence()
        {
            sequence = new Dictionary<Global.BET_SPOT, int>();
        }
    }
}
