using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class CColors : ICriteria
    {
        private Dictionary<Global.BET_SPOT, int> sequence;
        private int SPINLIMIT = 11;
        private decimal paststake = 0;
        public CColors()
        {
            SetInitSequence();
        }

        public string GetCriteriaName()
        {
            //return "C Colors";
            return "Algorithm C";
        }

        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            int repeat = 0;
            Global.BET_ITEMS result = new Global.BET_ITEMS();
            Global.BET_SPOT[] arrColors = Global.GetAllColors();
            List<Global.BET_SPOT> lstFoundColors = new List<Global.BET_SPOT>();

            SetInitSequence();
            for (repeat = 0; repeat < lstSpinAll.Count; repeat++)
            {
                Global.BET_SPOT elementColor = Global.GetBetColor(lstSpinAll[repeat]);
                if (!lstFoundColors.Contains(elementColor) && lstSpinAll[repeat] != Global.BET_SPOT.ZERO)
                {
                    lstFoundColors.Add(elementColor);
                    sequence[elementColor] = repeat;
                }
                if (lstFoundColors.Count == arrColors.Length)
                    break;
            }

            List<Global.BET_SPOT> lstBetSpots = new List<Global.BET_SPOT>();
            List<decimal> lstBetStakes = new List<decimal>();
            for (int Index = 0;Index < lstFoundColors.Count; Index++)
            {

                if (sequence[lstFoundColors[Index]] < SPINLIMIT)
                    continue;

                decimal stake = 0;
                if (paststake == 0)
                    stake = GlobalData.GetStartStake(startBettingAmount) * (decimal)Math.Pow(2, sequence[lstFoundColors[Index]] - SPINLIMIT);
                else
                    stake = paststake * 2;
                stake = Math.Round(stake, 1);
                lstBetStakes.Add(stake);
                lstBetSpots.Add(lstFoundColors[Index]);

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
