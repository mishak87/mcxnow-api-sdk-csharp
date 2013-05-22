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
    public sealed class Currency
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        private int Value { get; set; }

        public static readonly Currency MNC = new Currency(1, "MNC", "MinCoin");
        public static readonly Currency LTC = new Currency(2, "LTC", "Litecoin");
        public static readonly Currency SC = new Currency(3, "SC", "SolidCoin");
        public static readonly Currency DVC = new Currency(4, "DVC", "Devcoin");
        public static readonly Currency BTC = new Currency(5, "BTC", "Bitcoin");

        /**
         * Return list of all currencies except BTC (it is exchange base currency)
         */
        public static List<Currency> GetAllTraded()
        {
            return new List<Currency>
            {
                MNC,
                LTC,
                SC,
                DVC
            };
        }

        public static List<Currency> GetAll()
        {
            return new List<Currency>
            {
                BTC,
                MNC,
                LTC,
                SC,
                DVC
            };
        }

        private Currency(int value, String code, String name)
        {
            Code = code;
            Name = name;
            Value = value;
        }

        public override String ToString()
        {
            return Code;
        }

        public static Currency FromString(string value)
        {
            foreach (Currency currency in GetAll())
            {
                if (currency.Code == value || currency.Name == value)
                {
                    return currency;
                }
            }
            throw new Exception("Unknown currency");
        }
    }

    
}
