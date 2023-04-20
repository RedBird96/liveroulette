using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette.Criteria
{
    class ANumbers : ICriteria
    {
        private decimal sequence;
        private int SPINLIMIT = 330;
        private string strStakeArray = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,6,6,6,6,6,7,7,7,7,7,7,8,8,8,8,9,9,9,9,10,10,10,11,11,11,12,12,12,13,13,13,14,14,15,15,15,16,16,17,17,18,18,19,20,20,21,21,22,23,23,24,25,26,26,27,28,28,30,30,31,32,33,34,35,36,37,38,39,40,42,43,44,45,47,49,50,51,52,54,56,58,60,62,64,66,68,70,72,74,76,78,80,82,84,86,90,93,96,99,100,104,108,110,113,116,120,124,128,132,136,140,145,150,155,160,165,170,175,180,185,190,195";
        private List<decimal> lstStakes = new List<decimal>();  // 331~530
        private Dictionary<Global.BET_SPOT, int> lstFoundIndex;
        public ANumbers()
        {
            lstFoundIndex = new Dictionary<Global.BET_SPOT, int>();
            string[] splits = strStakeArray.Split(',');

            SetInitSequence();
            foreach (string split in splits)
            {
                decimal stake;
                if (decimal.TryParse(split, out stake) == false)
                    continue;

                lstStakes.Add(stake);
            }
        }

        public string GetCriteriaName()
        {
            //return "A Numbers";
            return "Algorithm A";
        }

        public Global.BET_ITEMS CheckBetPlaces(List<Global.BET_SPOT> lstSpinAll, decimal startBettingAmount)
        {
            Global.BET_SPOT[] arrStats = GlobalData.GetStatsNumbersExceptZero(lstSpinAll);
            Global.BET_SPOT[] arrNumbers = Global.GetAllNumbers();

            SetInitSequence();
            for (int i = 0; i < arrNumbers.Count(); i++)
            {
                lstFoundIndex[arrNumbers[i]] = lstSpinAll.Count();
                int nIndexFound = lstSpinAll.IndexOf(arrNumbers[i]);
                if (nIndexFound != -1)
                {
                    lstFoundIndex[arrNumbers[i]] = nIndexFound;
                }
            }

            Global.BET_ITEMS result = new Global.BET_ITEMS();
            List<decimal> lstBetStakes = new List<decimal>();
            List<Global.BET_SPOT> lstBetSpots = new List<Global.BET_SPOT>();
            for (int i = 0;i < lstFoundIndex.Count; i++)
            {
                if (lstFoundIndex[arrNumbers[i]] < SPINLIMIT)
                    continue;

                sequence = lstStakes[lstFoundIndex[arrNumbers[i]] % 330];
                lstBetStakes.Add(sequence);
                lstBetSpots.Add(arrNumbers[i]);
            }
            result.placeCount = lstBetStakes.Count;
            result.betStake = lstBetStakes.ToArray();
            result.arrBetSpots = lstBetSpots.ToArray();
            return result;
        }
        public void SetInitSequence()
        {
            sequence = 0m;
        }
    }
}
