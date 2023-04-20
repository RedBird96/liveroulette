using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveRoulette
{
    class Global
    {
        public enum BET_SPOT
        {
            None = -1,
            ZERO = 0,
            NUMBER1,
            NUMBER2,
            NUMBER3,
            NUMBER4,
            NUMBER5,
            NUMBER6,
            NUMBER7,
            NUMBER8,
            NUMBER9,
            NUMBER10,
            NUMBER11,
            NUMBER12,
            NUMBER13,
            NUMBER14,
            NUMBER15,
            NUMBER16,
            NUMBER17,
            NUMBER18,
            NUMBER19,
            NUMBER20,
            NUMBER21,
            NUMBER22,
            NUMBER23,
            NUMBER24,
            NUMBER25,
            NUMBER26,
            NUMBER27,
            NUMBER28,
            NUMBER29,
            NUMBER30,
            NUMBER31,
            NUMBER32,
            NUMBER33,
            NUMBER34,
            NUMBER35,
            NUMBER36,
            COLOR_BLACK = 37,
            COLOR_RED = 38,
            DOZEN_FIRST,
            DOZEN_SECOND,
            DOZEN_THIRD,
            COLUMN_FIRST,
            COLUMN_SECOND,
            COLUMN_THIRD,
            ODDEVEN_ODD,
            ODDEVEN_EVEN,
            HALF_FIRST,
            HALF_SECOND,
        }

        public struct BET_ITEMS
        {
            public int placeCount;
            public decimal[] betStake;
            public BET_SPOT[] arrBetSpots;
        }

        public static string GetBetCssSpotID(BET_SPOT number)
        {
            string spot_id = "";

            if (number >= BET_SPOT.ZERO && number <= BET_SPOT.NUMBER36)
                return ((int)number).ToString();

            switch(number)
            {
                case BET_SPOT.ODDEVEN_EVEN:
                    spot_id = "even";
                    break;
                case BET_SPOT.ODDEVEN_ODD:
                    spot_id = "odd";
                    break;
                case BET_SPOT.COLOR_BLACK:
                    spot_id = "black";
                    break;
                case BET_SPOT.COLOR_RED:
                    spot_id = "red";
                    break;
                case BET_SPOT.DOZEN_FIRST:
                    spot_id = "1st12";
                    break;
                case BET_SPOT.DOZEN_SECOND:
                    spot_id = "2nd12";
                    break;
                case BET_SPOT.DOZEN_THIRD:
                    spot_id = "3rd12";
                    break;
                case BET_SPOT.COLUMN_FIRST:
                    spot_id = "bottom2to1";
                    break;
                case BET_SPOT.COLUMN_SECOND:
                    spot_id = "middle2to1";
                    break;
                case BET_SPOT.COLUMN_THIRD:
                    spot_id = "top2to1";
                    break;
                case BET_SPOT.HALF_FIRST:
                    spot_id = "from1to18";
                    break;
                case BET_SPOT.HALF_SECOND:
                    spot_id = "from19to36";
                    break;
            }
            return spot_id;
        }

        public static string GetBetSpotString(BET_SPOT number)
        {
            string spotStr = "";

            if (number >= BET_SPOT.ZERO && number <= BET_SPOT.NUMBER36)
                return ((int)number).ToString();

            switch (number)
            {
                case BET_SPOT.ODDEVEN_EVEN:
                    spotStr = "Even";
                    break;
                case BET_SPOT.ODDEVEN_ODD:
                    spotStr = "Odd";
                    break;
                case BET_SPOT.COLOR_BLACK:
                    spotStr = "Black";
                    break;
                case BET_SPOT.COLOR_RED:
                    spotStr = "Red";
                    break;
                case BET_SPOT.DOZEN_FIRST:
                    spotStr = "1st12";
                    break;
                case BET_SPOT.DOZEN_SECOND:
                    spotStr = "2nd12";
                    break;
                case BET_SPOT.DOZEN_THIRD:
                    spotStr = "3rd12";
                    break;
                case BET_SPOT.COLUMN_FIRST:
                    spotStr = "Top2to1";
                    break;
                case BET_SPOT.COLUMN_SECOND:
                    spotStr = "Middle2to1";
                    break;
                case BET_SPOT.COLUMN_THIRD:
                    spotStr = "Bottom2to1";
                    break;
                case BET_SPOT.HALF_FIRST:
                    spotStr = "1-18";
                    break;
                case BET_SPOT.HALF_SECOND:
                    spotStr = "19-36";
                    break;
            }
            return spotStr;
        }

        public static BET_SPOT GetBetColor(BET_SPOT number)
        {
            if (number == BET_SPOT.ZERO)
                return BET_SPOT.ZERO;

            BET_SPOT[] arrReds = new BET_SPOT[] {
                BET_SPOT.NUMBER1,
                BET_SPOT.NUMBER3,
                BET_SPOT.NUMBER5,
                BET_SPOT.NUMBER7,
                BET_SPOT.NUMBER9,
                BET_SPOT.NUMBER12,
                BET_SPOT.NUMBER14,
                BET_SPOT.NUMBER16,
                BET_SPOT.NUMBER18,
                BET_SPOT.NUMBER19,
                BET_SPOT.NUMBER21,
                BET_SPOT.NUMBER23,
                BET_SPOT.NUMBER25,
                BET_SPOT.NUMBER27,
                BET_SPOT.NUMBER30,
                BET_SPOT.NUMBER32,
                BET_SPOT.NUMBER34,
                BET_SPOT.NUMBER36,
            };

            if (arrReds.Contains(number))
                return BET_SPOT.COLOR_RED;
            else
                return BET_SPOT.COLOR_BLACK;
        }

        public static BET_SPOT GetBetHalf(BET_SPOT number)
        {
            if (number >= BET_SPOT.NUMBER1 && number <= BET_SPOT.NUMBER18)
                return BET_SPOT.HALF_FIRST;

            return BET_SPOT.HALF_SECOND;
        }

        public static BET_SPOT GetBetDozen(BET_SPOT number)
        {
            if (number == BET_SPOT.ZERO)
                return BET_SPOT.ZERO;

            BET_SPOT[] arrDozenFirst = new BET_SPOT[] {
                BET_SPOT.NUMBER1,
                BET_SPOT.NUMBER2,
                BET_SPOT.NUMBER3,
                BET_SPOT.NUMBER4,
                BET_SPOT.NUMBER5,
                BET_SPOT.NUMBER6,
                BET_SPOT.NUMBER7,
                BET_SPOT.NUMBER8,
                BET_SPOT.NUMBER9,
                BET_SPOT.NUMBER10,
                BET_SPOT.NUMBER11,
                BET_SPOT.NUMBER12
            };
            BET_SPOT[] arrDozenSecond = new BET_SPOT[] {
                BET_SPOT.NUMBER13,
                BET_SPOT.NUMBER14,
                BET_SPOT.NUMBER15,
                BET_SPOT.NUMBER16,
                BET_SPOT.NUMBER17,
                BET_SPOT.NUMBER18,
                BET_SPOT.NUMBER19,
                BET_SPOT.NUMBER20,
                BET_SPOT.NUMBER21,
                BET_SPOT.NUMBER22,
                BET_SPOT.NUMBER23,
                BET_SPOT.NUMBER24
            };

            if (arrDozenFirst.Contains(number))
                return BET_SPOT.DOZEN_FIRST;
            else if (arrDozenSecond.Contains(number))
                return BET_SPOT.DOZEN_SECOND;
            else
                return BET_SPOT.DOZEN_THIRD;
        }


        public static BET_SPOT GetBetColumn(BET_SPOT number)
        {
            if (number == BET_SPOT.ZERO)
                return BET_SPOT.ZERO;

            BET_SPOT[] arrColumnFirst = new BET_SPOT[] {
                BET_SPOT.NUMBER1,
                BET_SPOT.NUMBER4,
                BET_SPOT.NUMBER7,
                BET_SPOT.NUMBER10,
                BET_SPOT.NUMBER13,
                BET_SPOT.NUMBER16,
                BET_SPOT.NUMBER19,
                BET_SPOT.NUMBER22,
                BET_SPOT.NUMBER25,
                BET_SPOT.NUMBER28,
                BET_SPOT.NUMBER31,
                BET_SPOT.NUMBER34
            };
            BET_SPOT[] arrColumnSecond = new BET_SPOT[] {
                BET_SPOT.NUMBER2,
                BET_SPOT.NUMBER5,
                BET_SPOT.NUMBER8,
                BET_SPOT.NUMBER11,
                BET_SPOT.NUMBER14,
                BET_SPOT.NUMBER17,
                BET_SPOT.NUMBER20,
                BET_SPOT.NUMBER23,
                BET_SPOT.NUMBER26,
                BET_SPOT.NUMBER29,
                BET_SPOT.NUMBER32,
                BET_SPOT.NUMBER35
            };

            if (arrColumnFirst.Contains(number))
                return BET_SPOT.COLUMN_FIRST;
            else if (arrColumnSecond.Contains(number))
                return BET_SPOT.COLUMN_SECOND;
            else
                return BET_SPOT.COLUMN_THIRD;
        }

        public static BET_SPOT GetBetOddEven(BET_SPOT number)
        {
            if (number == BET_SPOT.ZERO)
                return BET_SPOT.ZERO;

            BET_SPOT[] arrEven = new BET_SPOT[] {
                BET_SPOT.NUMBER2,
                BET_SPOT.NUMBER4,
                BET_SPOT.NUMBER6,
                BET_SPOT.NUMBER8,
                BET_SPOT.NUMBER10,
                BET_SPOT.NUMBER12,
                BET_SPOT.NUMBER14,
                BET_SPOT.NUMBER16,
                BET_SPOT.NUMBER18,
                BET_SPOT.NUMBER20,
                BET_SPOT.NUMBER22,
                BET_SPOT.NUMBER24,
                BET_SPOT.NUMBER26,
                BET_SPOT.NUMBER28,
                BET_SPOT.NUMBER30,
                BET_SPOT.NUMBER32,
                BET_SPOT.NUMBER34,
                BET_SPOT.NUMBER36
            };
            if (arrEven.Contains(number))
                return BET_SPOT.ODDEVEN_EVEN;
            else
                return BET_SPOT.ODDEVEN_ODD;
        }

        public static BET_SPOT[] GetAllNumbers()
        {
            return new BET_SPOT[] {
                BET_SPOT.ZERO,
                BET_SPOT.NUMBER1,
                BET_SPOT.NUMBER2,
                BET_SPOT.NUMBER3,
                BET_SPOT.NUMBER4,
                BET_SPOT.NUMBER5,
                BET_SPOT.NUMBER6,
                BET_SPOT.NUMBER7,
                BET_SPOT.NUMBER8,
                BET_SPOT.NUMBER9,
                BET_SPOT.NUMBER10,
                BET_SPOT.NUMBER11,
                BET_SPOT.NUMBER12,
                BET_SPOT.NUMBER13,
                BET_SPOT.NUMBER14,
                BET_SPOT.NUMBER15,
                BET_SPOT.NUMBER16,
                BET_SPOT.NUMBER17,
                BET_SPOT.NUMBER18,
                BET_SPOT.NUMBER19,
                BET_SPOT.NUMBER20,
                BET_SPOT.NUMBER21,
                BET_SPOT.NUMBER22,
                BET_SPOT.NUMBER23,
                BET_SPOT.NUMBER24,
                BET_SPOT.NUMBER25,
                BET_SPOT.NUMBER26,
                BET_SPOT.NUMBER27,
                BET_SPOT.NUMBER28,
                BET_SPOT.NUMBER29,
                BET_SPOT.NUMBER30,
                BET_SPOT.NUMBER31,
                BET_SPOT.NUMBER32,
                BET_SPOT.NUMBER33,
                BET_SPOT.NUMBER34,
                BET_SPOT.NUMBER35,
                BET_SPOT.NUMBER36};
        }

        public static string[] gSiteNameArray = 
        {
            "grosvenorcasinos",
            "betvictor",
            "williamhill",
            "betway",
            "betfair",
            "bwin",
            "888casino",
            "32red",
            "marathonbet",
            "leovegas",
            "casumo",
            "unibet",
            "mrgreen1",
            "matchbook"
        };

        public static bool gbDailyReachedStatus;

        public static Log log;

        public static string gSiteFrameName;

        public static WebElementInterface elementInterace;
                
        public static LIVESCENARIO_MODE gbOperationMode;

        public static bool gbIsSimulationMode;

        public static bool gbBalanceFlag;

        public static int gChangedSpinCount = 0;

        public static string gSiteName = "";

        public static AUTOBET_STATUS gAutoBetStatus;

        public static TablesMonitor tbMonitor;

        public static int m_nStartTableTick;

        public static string gStrHWID;

        public static string gAccountMail;

        public static StripeAPI stripe;

        public static CardInfo card;

        public static Global.ACCOUNT_STATUS AccountMode;
        public static BET_SPOT[] GetAllColors()
        {
            return new BET_SPOT[] { BET_SPOT.COLOR_BLACK, BET_SPOT.COLOR_RED };
        }

        public static BET_SPOT[] GetAllOddEven()
        {
            return new BET_SPOT[] { BET_SPOT.ODDEVEN_ODD, BET_SPOT.ODDEVEN_EVEN };
        }

        public static BET_SPOT[] GetAllHalfs()
        {
            return new BET_SPOT[] { BET_SPOT.HALF_FIRST, BET_SPOT.HALF_SECOND };
        }
        public static BET_SPOT[] GetAllDozens()
        {
            return new BET_SPOT[] { BET_SPOT.DOZEN_FIRST, BET_SPOT.DOZEN_SECOND , BET_SPOT.DOZEN_THIRD };
        }
        public static BET_SPOT[] GetAllColumns()
        {
            return new BET_SPOT[] { BET_SPOT.COLUMN_FIRST, BET_SPOT.COLUMN_SECOND, BET_SPOT.COLUMN_THIRD };
        }

        public static int LOG_LEVEL_ONLYFILE = 0;
        public static int LOG_LEVEL_LISTFILE = 1;

        public enum LIVESCENARIO_MODE
        {
            LIVE_MODE,
            SCENARIOTEST_MODE
        }

        public enum AUTOBET_STATUS
        {
            AUTOBET_STARTED,
            AUTOBET_STOPPED
        }

        public enum CONNECTION_STATUS
        {
            CONNECTED,
            DISCONNECTED
        }

        public enum ACCOUNT_STATUS
        {
            NONE,
            TRIAL_MODE,
            PAY_MODE,
        }
    }
}
