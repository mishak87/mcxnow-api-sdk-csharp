#region license
//   Please read and agree to license.md contents before using this SDK.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcxNOW
{
    public class Fund
    {
        public Currency Currency { get; set; }

        public decimal Balance { get; set; }

        public decimal Incoming { get; set; }

        public string DepositAddress { get; set; }

        public decimal MinimumDeposit { get; set; }

        public int DepositConfirmations { get; set; }

        public decimal WithdrawFee { get; set; }
    }
}
