using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class BConsecutiveNumbers : ICriteria
    {
        private int sequence;
        private int SPINLIMIT = 3;
        private decimal paststake = 0;
        public BConsecutiveNumbers()
        {
            SetInitSequence();
        }

        public string GetCriteriaName()
        {
            //return "B Consecutive Numbers";
            return "Algorithm B";
        }

        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);

            return new Global.BET_ITEMS();

            int repeat = 0;
            int same_count = 1;
            Global.BET_SPOT first = Global.BET_SPOT.NUMBER29;

            for (repeat = 0; repeat < 3; repeat++)
            {
                if (lstSpinAll[repeat] != Global.BET_SPOT.NUMBER29)
                { 
                    first = lstSpinAll[repeat];
                    repeat++;
                    break;
                }
            }
            SetInitSequence();

            for (; repeat < lstSpinAll.Count; repeat++)
            {
                if (lstSpinAll[repeat] != first)
                    break;
                same_count++;
            }

            if (same_count < SPINLIMIT || first == Global.BET_SPOT.NUMBER29)
            {
                paststake = 0;
                SetInitSequence();
                return new Global.BET_ITEMS();
            }

            sequence = repeat - SPINLIMIT;
            Console.WriteLine($"B Sequence {repeat}");

            decimal stake = 0;
            if (paststake == 0)
                stake = GlobalData.GetStartStake(startBettingAmount) * (decimal)Math.Pow(35, sequence);
            else
                stake = stake * 35;

            Global.BET_ITEMS result = new Global.BET_ITEMS();

            Random rd = new Random();
            Global.BET_SPOT[] arrNumbers = Global.GetAllNumbers();
            List<Global.BET_SPOT> lstBetSpots = new List<Global.BET_SPOT>();
            List<decimal> lstBetStakes = new List<decimal>();
            for(int i = 0; i < arrNumbers.Count(); i++)
            {
                if (arrNumbers[i] == first || arrNumbers[i] == Global.BET_SPOT.NUMBER29)
                    continue;
                lstBetSpots.Add(arrNumbers[i]);
                lstBetStakes.Add(stake);
            }

            paststake = stake;

            result.placeCount = lstBetSpots.Count;
            result.arrBetSpots = lstBetSpots.ToArray();
            result.betStake = lstBetStakes.ToArray();
            return result;
        }
        public void SetInitSequence()
        {
            sequence = 0;
        }
    }
}
