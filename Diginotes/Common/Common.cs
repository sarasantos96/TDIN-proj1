using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    [Serializable]
    public class User
    {
        public string Username { get; set; }
        public string Pass { get; set; }
        public string Name { get; set; }

        public User(string username, string pass, string name)
        {
            Username = username;
            Pass = pass;
            Name = name;
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

    }

    [Serializable]
    public class Diginote
    {
        public string SerialNumber { get; set; }
        public int FacialValue { get; set; }
        public User Owner { get; set; }

        public Diginote(string serialNumber, User owner)
        {
            SerialNumber = serialNumber;
            FacialValue = 1;
            Owner = owner;
        }
    }

    public enum OrderType { SELL, PURCHASE };

    [Serializable]
    public class Order
    {
        public OrderType Type;
        public User Owner;
        public int Quantity;

        public Order(OrderType type, User user, int quantity)
        {
            Type = type;
            Owner = user;
            Quantity = quantity;
        }
    }

    public class RemoteNew
    {
        private static Hashtable types = null;

        private static void InitTypeTable()
        {
            types = new Hashtable();
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                types.Add(entry.ObjectType, entry);
        }

        public static object New(Type type)
        {
            if (types == null)
                InitTypeTable();
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)types[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return RemotingServices.Connect(type, entry.ObjectUrl);
        }
    }

    public interface IRegistry
    {
        event QuoteChangedEvent QuoteChanged;
        Boolean AddUser(User user);
        User CheckLogin(string user, string pass);
        int GetQuote();
        void SetQuote(int quote);
        void CreateDiginote(string serialNumber, User owner);
        List<Diginote> GetUserDiginotes(User user);
        void AddOrder(Order order);
        List<Order> GetOrders();
        List<Order> GetUserPendingOrders(User user);
    }

    public delegate void QuoteChangedEvent(int quote);

    public class EventIntermediate : MarshalByRefObject
    {
        public event QuoteChangedEvent newQuote;
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public EventIntermediate(IRegistry server)
        {
            server.QuoteChanged += FireChangedQuote;
        }

        public void FireChangedQuote(int quote)
        {
            newQuote(quote);
        }
    }
}
