using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class QConsecutiveColumnIndividual : ICriteria
    {
        private int SPINLIMIT = 23;
        //        private Dictionary<Global.BET_SPOT, int> sequence;
        int sequence;
        public QConsecutiveColumnIndividual()
        {
            SetInitSequence();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            int repeat = 0;
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);
            Global.BET_SPOT frontColumn = Global.GetBetColumn(arrStats[0]);
            int sameColumnIndex = 1;

            SetInitSequence();
            for (repeat = 1; repeat < lstSpinAll.Count(); repeat++)
            {
                if (Global.GetBetColumn(lstSpinAll[repeat]) == frontColumn && Global.GetBetColumn(lstSpinAll[repeat - 1]) == frontColumn)
                    break;
                if (Global.GetBetColumn(lstSpinAll[repeat]) == frontColumn)
                    sameColumnIndex++;
            }

            if (sameColumnIndex < SPINLIMIT)
            {
                sequence = 0;
                return new Global.BET_ITEMS();
            }

            for (repeat = 0; repeat < lstSpinAll.Count; repeat++)
            {
                if (lstSpinAll[repeat] != Global.BET_SPOT.ZERO)
                    break;
            }
            sequence = sameColumnIndex - SPINLIMIT + repeat;
            decimal stake = GlobalData.GetStackAmountBySequence(sequence, startBettingAmount);
            stake = Math.Round(stake, 1);
            Global.BET_ITEMS result = new Global.BET_ITEMS();
            result.betStake = new decimal[] { stake };
            result.placeCount = 1;
            result.arrBetSpots = new Global.BET_SPOT[] { frontColumn };


            return result;
        }

        public string GetCriteriaName()
        {
            //return "Q Consecutive Column individual";
            return "Algorithm Q";
        }
        public void SetInitSequence()
        {
        }
    }
}
