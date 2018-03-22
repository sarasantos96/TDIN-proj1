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
            Registry.Registry r = new Registry.Registry();
            /*User user1 = new User("sara", "123",0);
            User user2 = new User("joao", "123",0);
            User user3 = new User("ines", "123",0);
            User user4 = new User("nuno", "123",0);

            r.AddUser(user1);
            r.AddUser(user2);
            r.AddUser(user3);
            r.AddUser(user4);

            ArrayList l = r.getUsers();
            foreach (User u in l)
                Console.WriteLine("" + u.Name + " " + u.Pass);*/

            //Boolean log = r.CheckLogin("sara", "123");
            //Boolean log2 = r.CheckLogin("joao", "123");
            /*
            if (log)
            {
                Console.WriteLine("Login successful");
            }
            else
            {
                Console.WriteLine("User does not exist");
            }

            if (log2)
            {
                Console.WriteLine("Login successful");
            }
            else
            {
                Console.WriteLine("User does not exist");
            }
            */
            User user1 = new User("sara", "123", 0);
            r.AddUser(user1);


            Console.ReadLine();
        }
    }
}
