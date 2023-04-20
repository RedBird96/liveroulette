using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class JConsecutiveRB : ICriteria
    {
        private int sequence;
        private int SPINLIMIT = 11;
        private decimal paststake = 0;
        private Global.BET_SPOT betColor;
        public JConsecutiveRB()
        {
            SetInitSequence();
            betColor = new Global.BET_SPOT();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            decimal stake;
            int repeat = 0;
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);
            Global.BET_ITEMS result = new Global.BET_ITEMS();

            SetInitSequence();
            /*if (lstSpinAll[0] == Global.BET_SPOT.ZERO)
                return result;*/
            Global.BET_SPOT front = Global.GetBetColor(arrStats[0]);

            for (repeat = 1; repeat < lstSpinAll.Count; repeat++)
            {
                if (Global.GetBetColor(lstSpinAll[repeat]) == Global.GetBetColor(lstSpinAll[repeat - 1]) && lstSpinAll[repeat] != Global.BET_SPOT.ZERO && lstSpinAll[repeat - 1] != Global.BET_SPOT.ZERO)
                { 
                    break;
                }
            }

            repeat = repeat - 1;

            if (repeat < SPINLIMIT)
            {
                paststake = 0;
                SetInitSequence();
                return new Global.BET_ITEMS();
            }

            sequence = repeat - SPINLIMIT;

            if (paststake == 0)
                stake = GlobalData.GetStartStake(startBettingAmount) * (decimal)Math.Pow(2, sequence);
            else
                stake = paststake * 2;

            stake = Math.Round(stake, 1);

            result.betStake = new decimal[] { stake };
            result.placeCount = 1;
            result.arrBetSpots = new Global.BET_SPOT[] { front };

            paststake = stake;
            return result;
        }

        public string GetCriteriaName()
        {
            //return "J Consecutive RB";
            return "Algorithm J";
        }
        public void SetInitSequence()
        {
            sequence = 0;
        }
    }
}
