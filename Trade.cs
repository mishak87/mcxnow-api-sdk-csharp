#region license
//   Please read and agree to license.md contents before using this SDK.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcxNOW
{
    public class Trade
    {
        public static Trade Buy(Currency currency, decimal amount, decimal price)
        {
            return new Trade()
            {
                Type = Types.BUY,
                Currency = currency,
                Amount = amount,
                Price = price
            };
        }

        public static Trade Sell(Currency currency, decimal amount, decimal price)
        {
            return new Trade()
            {
                Type = Types.SELL,
                Currency = currency,
                Amount = amount,
                Price = price
            };
        }

        public enum Types
        {
            SELL,
            BUY
        }

        public Types Type { get; set; }

        private decimal amount = 0M;

        public decimal Amount
        {
            get
            {
                return amount;
            }
            set
            {
                if (Type == Types.SELL && value < 0.01M)
                {
                    throw new ArgumentException("Sell amount must be greater then 0.01");
                }
                else if (Type == Types.BUY && value < 0.02M)
                {
                    throw new ArgumentException("Buy amount must be greater then 0.02");
                }
                else
                {
                    amount = value;
                }
            }
        }

        private decimal price = 0M;

        public decimal Price
        {
            get
            {
                return price;
            }
            set
            {
                if (value < 0.00000001M)
                {
                    throw new ArgumentException("Price cannot be bellow 0.00000001");
                }
                else
                {
                    price = value;
                }
            }
        }

        private Currency currency = null;

        public Currency Currency {
            get
            {
                return currency;
            }
            set
            {
                if (value == Currency.BTC)
                {
                    throw new ArgumentException("You cannot trade in BTC");
                }
                else if (value != currency)
                {
                    currency = value;
                }
            }                    
        }

        public bool Execute { get; set; }

        private Trade()
        {
            Execute = false;
        }
    }}
