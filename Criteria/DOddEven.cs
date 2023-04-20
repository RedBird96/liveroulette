using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class DOddEven : ICriteria
    {
        private Dictionary<Global.BET_SPOT, int> sequence;
        private int SPINLIMIT = 11;
        private decimal paststake = 0;

        public DOddEven()
        {
            SetInitSequence(); 
        }

        public string GetCriteriaName()
        {
            //return "D Odd/Even";
            return "Algorithm D";
        }

        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            int repeat = 0;
            Global.BET_ITEMS result = new Global.BET_ITEMS();
            Global.BET_SPOT[] arrOddEvens = Global.GetAllOddEven();
            List<Global.BET_SPOT> lstFoundOddEvens = new List<Global.BET_SPOT>();

            SetInitSequence();
            for (repeat = 0; repeat < lstSpinAll.Count; repeat++)
            {
                Global.BET_SPOT elementOddEven = Global.GetBetOddEven(lstSpinAll[repeat]);
                if (!lstFoundOddEvens.Contains(elementOddEven) && lstSpinAll[repeat] != Global.BET_SPOT.ZERO)
                {
                    lstFoundOddEvens.Add(elementOddEven);
                    sequence[elementOddEven] = repeat;
                }
                if (lstFoundOddEvens.Count == arrOddEvens.Length)
                    break;
            }

            List<Global.BET_SPOT> lstBetSpots = new List<Global.BET_SPOT>();
            List<decimal> lstBetStakes = new List<decimal>();
            for (int Index = 0; Index < lstFoundOddEvens.Count; Index++)
            {
                if (sequence[lstFoundOddEvens[Index]] < SPINLIMIT)
                    continue;

                decimal stake = 0;
                if (paststake == 0)
                    stake = GlobalData.GetStartStake(startBettingAmount) * (decimal)Math.Pow(2, sequence[lstFoundOddEvens[Index]] - SPINLIMIT);
                else
                    stake = paststake * 2;
                stake = Math.Round(stake, 1);
                lstBetStakes.Add(stake);
                lstBetSpots.Add(lstFoundOddEvens[Index]);

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
