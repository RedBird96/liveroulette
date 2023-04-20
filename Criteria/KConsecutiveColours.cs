using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class KConsecutiveColours : ICriteria
    {
        private int SPINLIMIT = 11;
        int sequence;
        private decimal paststake = 0;
        public KConsecutiveColours()
        {
            SetInitSequence();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            int samecolorCount = 0;
            int repeat = 0;
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);
            Global.BET_ITEMS result = new Global.BET_ITEMS();

            SetInitSequence();

            if (lstSpinAll[0] != Global.BET_SPOT.ZERO)
                samecolorCount = 1;

            Global.BET_SPOT front = Global.GetBetColor(arrStats[0]);

            for (repeat = 1; repeat < lstSpinAll.Count(); repeat++)
            {
                if (Global.GetBetColor(lstSpinAll[repeat]) == front && Global.GetBetColor(lstSpinAll[repeat - 1]) == front /*&& lstSpinAll[repeat] != Global.BET_SPOT.ZERO && lstSpinAll[repeat] != Global.BET_SPOT.ZERO*/)
                    break;
                if (Global.GetBetColor(lstSpinAll[repeat]) == front)
                    samecolorCount++;
            }

            if (samecolorCount < SPINLIMIT)
            {
                paststake = 0;
                sequence = 0;
                return new Global.BET_ITEMS();
            }

            sequence = samecolorCount - SPINLIMIT;
            samecolorCount = 0;

            for (repeat = repeat - 1; repeat >= 0; repeat--)
            {
                if (Global.GetBetColor(lstSpinAll[repeat]) == front)
                    samecolorCount++;
                if (samecolorCount >= SPINLIMIT && lstSpinAll[repeat] == Global.BET_SPOT.ZERO)
                    sequence++;
            }

            decimal stake = 0;
            if (paststake == 0)
                stake = GlobalData.GetStartStake(startBettingAmount) * (decimal)Math.Pow(2, sequence);
            else
                stake = paststake * 2;

            stake = Math.Round(stake, 1);

            result.betStake = new decimal[]{ stake };
            result.placeCount = 1;
            result.arrBetSpots = new Global.BET_SPOT[] { front };

            paststake = stake;

            return result;
        }

        public string GetCriteriaName()
        {
            //return "K Consecutive colours";
            return "Algorithm K";
        }
        public void SetInitSequence()
        {
            sequence = 0;
        }
    }
}
