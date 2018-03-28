using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using Registry;
using Common;
using System.Collections;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("Test.exe.config", false);
            IRegistry r = (IRegistry) RemoteNew.New(typeof(IRegistry));
            /* User user1 = new User("sara", "123","Sara Santos");
             User user2 = new User("joao", "123","João Chaves");

             r.AddUser(user1);
             r.AddUser(user2);*/

            Boolean check1 = r.CheckLogin("sara","123");
            Boolean check2 = r.CheckLogin("joao","123");

            if (check1) Console.WriteLine("Login 1 sucessfull");
            if (check2) Console.WriteLine("Login 2 sucessfull");

            r.SetQuote(2);
            Console.WriteLine("Quote changed...");

            Console.ReadLine();
        }
    }
}
