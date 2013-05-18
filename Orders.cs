using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcxNOW
{
    public class Orders
    {
        public string id { get; set; }
        public List<GroupedOrder> buy { get; set; }
        public List<GroupedOrder> sell { get; set; }
        public List<History> history { get; set; }
        public List<Volume> vol { get; set; }
        public int users { get; set; }
        public decimal curvol { get; set; }
        public decimal basevol { get; set; }
        public decimal lprice { get; set; }
        public decimal pricel { get; set; }
        public decimal priceh { get; set; }
    }

    public class GroupedOrder
    {
        /**
         * Last one is total text therefore string
         */
        public string p { get; set; }
        public decimal c1 { get; set; }
        public decimal c2 { get; set; }
    }

    public class Volume
    {
        public decimal p { get; set; }
        public decimal a { get; set; }
    }

    public class History
    {
        public DateTime t { get; set; }
        public bool b { get; set; }
        public decimal p { get; set; }
        public decimal c1 { get; set; }
        public decimal c2 { get; set; }
    }
}
