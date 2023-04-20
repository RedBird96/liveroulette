using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveRoulette
{
    public class CardInfo
    {
        public string cardNo { get; set; }
        public long expMonth { get; set; }
        public long expYear { get; set; }
        public string cvc { get; set; }
        public string Id { get; set; }

    }

    public class ProductInfo
    {
        public double price { get; set; }
        public int intervalDay { get; set; }
        public string intervalPeriod { get; set; }
        public int metaDataTrialDay { get; set; }
        public string currencyUnit { get; set; }
    }
}
