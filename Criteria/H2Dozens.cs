using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class H2Dozens : ICriteria
    {
        private int sequence;
        private int SPINLIMIT = 8;
        private decimal paststake = 0;
        public H2Dozens()
        {
            SetInitSequence();
        }
        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);

            int repeat = 0;
            Global.BET_ITEMS result = new Global.BET_ITEMS();

            SetInitSequence();
            /*
            if (lstSpinAll[0] == Global.BET_SPOT.ZERO)
                return result;
            */
            Global.BET_SPOT first = Global.GetBetDozen(arrStats[0]);

            for (repeat = 0; repeat < lstSpinAll.Count; repeat++)
            {
                if (Global.GetBetDozen(lstSpinAll[repeat]) != first && lstSpinAll[repeat] != Global.BET_SPOT.ZERO)
                    break;
            }

            if (repeat < SPINLIMIT)
            {
                paststake = 0;
                SetInitSequence();
                return new Global.BET_ITEMS();
            }

            sequence = repeat - SPINLIMIT;

            decimal stake = 0;
            if (paststake == 0)
                stake = GlobalData.GetStartStake(startBettingAmount) * (decimal)Math.Pow(3, sequence);
            else
                stake = paststake * 3;
            stake = Math.Round(stake, 1);

            Global.BET_SPOT[] arrDozens = Global.GetAllDozens();
            List<Global.BET_SPOT> lstBetSpots = new List<Global.BET_SPOT>();
            List<decimal> lstBetStaks = new List<decimal>();
            for (int i = 0; i < arrDozens.Count(); i++)
            {
                if (arrDozens[i] == first)
                    continue;
                lstBetSpots.Add(arrDozens[i]);
                lstBetStaks.Add(stake);
            }
            result.placeCount = lstBetSpots.Count;
            result.arrBetSpots = lstBetSpots.ToArray();
            result.betStake = lstBetStaks.ToArray();

            paststake = result.betStake[0];

            return result;
        }

        public string GetCriteriaName()
        {
            //return "H 2 Dozens";
            return "Algorithm H";
        }
        public void SetInitSequence()
        {
            sequence = 0;
        }
    }
}
