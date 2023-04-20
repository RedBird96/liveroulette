using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class NConsecutiveDozen : ICriteria
    {
        private int sequence;
        private int SPINLIMIT = 23;
        public NConsecutiveDozen()
        {
            SetInitSequence();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            int repeat = 0;
            bool isFound = false;
            Global.BET_ITEMS result = new Global.BET_ITEMS();
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);
            Global.BET_SPOT front = Global.GetBetDozen(arrStats[0]);

            SetInitSequence();
            /*if (lstSpinAll[0] == Global.BET_SPOT.ZERO)
                return result;*/

            for (repeat = 1; repeat < lstSpinAll.Count; repeat++)
            {
                if (Global.GetBetDozen(lstSpinAll[repeat]) == Global.GetBetDozen(lstSpinAll[repeat - 1]) && lstSpinAll[repeat] != Global.BET_SPOT.ZERO && lstSpinAll[repeat - 1] != Global.BET_SPOT.ZERO)
                    break;
            }

            repeat = repeat - 1;
            if (repeat < SPINLIMIT)
            {
                SetInitSequence();
                return new Global.BET_ITEMS();
            }

            sequence = repeat - SPINLIMIT;
            decimal stake = GlobalData.GetStackAmountBySequence(sequence, startBettingAmount);
            stake = Math.Round(stake, 1);

            result.betStake = new decimal[] { stake };
            result.placeCount = 1;
            result.arrBetSpots = new Global.BET_SPOT[] { front };
            sequence++;
            return result;
        }

        public string GetCriteriaName()
        {
            //return "N Consecutive Dozen";
            return "Algorithm N";
        }
        public void SetInitSequence()
        {
            sequence = 0;
        }
    }
}
