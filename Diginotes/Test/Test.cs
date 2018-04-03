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
             User user1 = new User("sara", "123","Sara Santos");
             User user2 = new User("joao", "123","João Chaves");

            /* r.AddUser(user1);
             r.AddUser(user2);*/

            User check1 = r.CheckLogin("sara","123");
            User check2 = r.CheckLogin("joao","123");

            if (check1 != null) Console.WriteLine("Login 1 sucessfull");
            if (check2 != null) Console.WriteLine("Login 2 sucessfull");

            r.SetQuote(2);
            Console.WriteLine("Quote changed...");

            /*Console.WriteLine("Creating Diginotes...");
            r.CreateDiginote("AS123567", user1);
            r.CreateDiginote("AS123568", user1);
            r.CreateDiginote("AS123569", user1);

            Console.WriteLine("User1 Diginotes:");
            List<Diginote> diginotes = r.GetUserDiginotes(user1);
            foreach(Diginote d in diginotes)
            {
                Console.WriteLine("Diginote " + d.SerialNumber);
            }*/

            Order order1 = new Order(OrderType.PURCHASE, user1, 20);
            Order order2 = new Order(OrderType.PURCHASE, user1, 10);
            Order order3 = new Order(OrderType.PURCHASE, user1, 10);
            Order order4 = new Order(OrderType.PURCHASE, user1, 10);
            Order order5 = new Order(OrderType.SELL, user2, 10);
            Order order6 = new Order(OrderType.SELL, user2, 60);
            r.AddOrder(order1);
           /* r.AddOrder(order2);
            r.AddOrder(order3);
            r.AddOrder(order4);*/
            r.AddOrder(order2);

           /* Console.WriteLine("Orders:");
            foreach (Order o in r.GetOrders())
            {
                Console.WriteLine("User " + o.Owner.Name + ", quantity=" + o.Quantity);
            }

            Console.ReadLine();

            r.AddOrder(order6);

            foreach (Order o in r.GetOrders())
            {
                Console.WriteLine("User " + o.Owner.Name + ", quantity=" + o.Quantity);
            }
            */
            Console.ReadLine();
        }
    }
}
