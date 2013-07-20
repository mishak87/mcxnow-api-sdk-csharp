#region license
//   Please read and agree to license.md contents before using this SDK.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mcxNOW
{
    public class Info
    {
        /**
         * Amount of base currency in users account
         */
        public decimal base_bal { get; set; }

        /**
         * Amount of info currency in users account
         */
        public decimal cur_bal { get; set; }

        /**
         * Users buy and sell orders
         */
        public List<Order> orders { get; set; }
    }


    public class Order
    {
        /**
         * Id of order
         */
        public string id { get; set; }

        /**
         * buy or sell  
         * false = sell
         * true = buy
         * {ck}
         */
        public bool b { get; set; }

        /**
         * Was executed 1 yes 0 no
         */
        public bool e { get; set; }
        
        /**
         * Time order was placed
         * DateTime not working {ck}
         */
        public int t { get; set; }
        
        /**
         * Volume of the order
         * {ck}
         */
        public decimal a1 { get; set; }

        /**
         * Price of the order
         * {ck}
         */
        public decimal p { get; set; }

        /* Not longer in curent API {ck}
        private OrderInfo orderInfo = null;

        public OrderInfo GetOrderInfo()
        {
            if (orderInfo == null)
            {
                orderInfo = OrderInfo.FromOrder(this);
            }
            return orderInfo;
        }
         */
    }

    /* Not longer in curent API {ck}
 
    public class OrderInfo
    {
        public enum Types {
            Buy,
            Sell
        };

        public Types Type { get; private set; }

        public Currency Currency { get; private set; }

        public decimal Amount { get; private set; }

        public decimal Price { get; private set; }

        
        public string DateTime { get; private set; }

        public static OrderInfo FromOrder(Order order)
        {
            // "Buy (Devcoin) with (0.01)BTC for (0.0000001)BTC each"
            // "Sell (20.0)(MNC) for (0.002928)BTC each"
            Regex buyRegex = new Regex(@"^Buy (?<currency>[A-Za-z]+) with (?<amount>[0-9]+(\.[0-9]+)?)BTC for (?<price>[0-9]+(\.[0-9]+)?)BTC each$", RegexOptions.Singleline);
            Regex sellRegex = new Regex(@"^Sell (?<amount>[0-9]+(\.[0-9]+)?)(?<currency>[A-Za-z]{2,3}) for (?<price>[0-9]+(\.[0-9]+)?)BTC each$", RegexOptions.Singleline);
            Match match = buyRegex.Match(order.a1);
            if (!match.Success)
            {
                match = sellRegex.Match(order.a1);
            }

            if (!match.Success)
            {
                throw new ArgumentException();
            }

            return new OrderInfo()
            {
                Type = match.Groups["type"].Value == "Buy" ? Types.Buy : Types.Sell,
                Currency = Currency.FromString(match.Groups["currency"].Value),
                Amount = Convert.ToDecimal(match.Groups["amount"].Value, System.Globalization.NumberFormatInfo.InvariantInfo),
                Price = Convert.ToDecimal(match.Groups["price"].Value, System.Globalization.NumberFormatInfo.InvariantInfo),
            };
        }
    }

    */
}
