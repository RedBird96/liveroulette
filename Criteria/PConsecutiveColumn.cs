using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class PConsecutiveColumn : ICriteria
    {
        private int SPINLIMIT = 21;
        private int sequence;
        public PConsecutiveColumn()
        {
            SetInitSequence();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            int repeat = 0;
            bool isFound = false;
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);
            Global.BET_ITEMS result = new Global.BET_ITEMS();

            SetInitSequence();
            /*if (lstSpinAll[0] == Global.BET_SPOT.ZERO)
                return result;*/

            Global.BET_SPOT front = Global.GetBetColumn(arrStats[0]);

            for (repeat = 1; repeat < lstSpinAll.Count; repeat++)
            {
                if (Global.GetBetColumn(lstSpinAll[repeat]) == Global.GetBetColumn(lstSpinAll[repeat - 1]) && lstSpinAll[repeat] != Global.BET_SPOT.ZERO && lstSpinAll[repeat - 1] != Global.BET_SPOT.ZERO)
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
            //return "P Consecutive Column";
            return "Algorithm P";
        }
        public void SetInitSequence()
        {
            sequence = 0;
        }
    }
}
