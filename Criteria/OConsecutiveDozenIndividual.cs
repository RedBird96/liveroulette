using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class OConsecutiveDozenIndividual : ICriteria
    {
        private int SPINLIMIT = 23;
//        private Dictionary<Global.BET_SPOT, int> sequence;
        int sequence;
        public OConsecutiveDozenIndividual()
        {
            SetInitSequence();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            int repeat = 0;
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);
            Global.BET_SPOT firstDozen = Global.GetBetDozen(arrStats[0]);
            Global.BET_ITEMS result = new Global.BET_ITEMS();

            SetInitSequence();
            /*if (lstSpinAll[0] == Global.BET_SPOT.ZERO)
                return result;*/

            int sameDozenIndex = 1;
            for (repeat = 1; repeat < lstSpinAll.Count(); repeat++)
            {
                if (Global.GetBetDozen(lstSpinAll[repeat]) == firstDozen && Global.GetBetDozen(lstSpinAll[repeat - 1]) == firstDozen)
                    break;
                if (Global.GetBetDozen(lstSpinAll[repeat]) == firstDozen)
                    sameDozenIndex++;
            }

            if (sameDozenIndex < SPINLIMIT)
            {
                sequence = 0;
                return new Global.BET_ITEMS();
            }

            for (repeat = 0; repeat < lstSpinAll.Count; repeat++)
            {
                if (lstSpinAll[repeat] != Global.BET_SPOT.ZERO)
                    break;
            }
            sequence = sameDozenIndex - SPINLIMIT + repeat;

            decimal stake = GlobalData.GetStackAmountBySequence(sequence, startBettingAmount);
            stake = Math.Round(stake, 1);
            result.betStake = new decimal[] { stake };
            result.placeCount = 1;
            result.arrBetSpots = new Global.BET_SPOT[] { firstDozen };

            return result;
        }

        public string GetCriteriaName()
        {
            //return "O Consecutive Dozen individual";
            return "Algorithm O";
        }
        public void SetInitSequence()
        {
        }
    }
}
