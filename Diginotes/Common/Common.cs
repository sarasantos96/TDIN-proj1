using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class User
    {
        public string Name { get; set; }
        public string Pass { get; set; }
        public int Quote { get; set; }

        public User(string name, string pass, int quote)
        {
            Name = name;
            Pass = pass;
            Quote = quote;
        }
    }
}
