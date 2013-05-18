using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcxNOW
{
    public class ChatResponse
    {
        public int id { get; set; }
        public List<c> c { get; set; }
    }

    public class c : ChatMessage { }
    public class ChatMessage
    {
        public string n { get; set; }
        public int i { get; set; }
        public string t { get; set; }
    }
}
