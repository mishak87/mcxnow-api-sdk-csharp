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
         * Was executed 1 yes 0 no
         */
        public bool e { get; set; }
        
        /**
         * Time order was placed
         */
        public DateTime t { get; set; }
        
        /**
         * Textual info about order
         */
        public string i { get; set; }

        private OrderInfo orderInfo = null;

        public OrderInfo GetOrderInfo()
        {
            if (orderInfo == null)
            {
                orderInfo = OrderInfo.FromOrder(this);
            }
            return orderInfo;
        }
    }

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

        public DateTime DateTime { get; private set; }

        public static OrderInfo FromOrder(Order order)
        {
            // "Buy (Devcoin) with (0.01)BTC for (0.0000001)BTC each"
            // "Sell (20.0)(MNC) for (0.002928)BTC each"
            Regex buyRegex = new Regex(@"^Buy (?<currency>[A-Za-z]+) with (?<amount>[0-9]+(\.[0-9]+)?)BTC for (?<price>[0-9]+(\.[0-9]+)?)BTC each$", RegexOptions.Singleline);
            Regex sellRegex = new Regex(@"^Sell (?<amount>[0-9]+(\.[0-9]+)?)(?<currency>[A-Za-z]{2,3}) for (?<price>[0-9]+(\.[0-9]+)?)BTC each$", RegexOptions.Singleline);
            Match match = buyRegex.Match(order.i);
            if (!match.Success)
            {
                match = sellRegex.Match(order.i);
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
}
