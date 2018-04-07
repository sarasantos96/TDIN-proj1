using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Registry
{
    public class Registry : MarshalByRefObject, IRegistry
    {
        public event ChangeEventHandler ChangedEvent;
        public List<Order> orders;

        System.Data.SqlClient.SqlConnection con;
        float quote;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public Registry()
        {
            Console.WriteLine("Registry constructor evoked");
            con = new System.Data.SqlClient.SqlConnection(Properties.Settings.Default.Database1);
            Console.WriteLine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            LoadQuote();
            LoadOrders();
        }

        public void LoadQuote()
        {
            Console.WriteLine("loadQuote() invoked");
            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT Value FROM [Quote] WHERE Id=1";
                cmd.Connection = con;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    con.Close();
                }
                else
                {
                    reader.Read();
                    quote = reader.GetFloat(0);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[loadQuote] Exeception Caught: " + e.Message);
                return;
            }
        }

        public void LoadOrders()
        {
            Console.WriteLine("loadOrders() invoked");
            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT * FROM [Order] ORDER BY TimeStamp ASC";
                cmd.Connection = con;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                orders = new List<Order>();

                if (!reader.HasRows)
                {
                    con.Close();
                }
                else
                {
                    while (reader.Read())
                    {
                        User u = new User(reader.GetString(1), null, null);
                        OrderType type;

                        if (reader.GetString(2) == "SELL")
                        {
                            type = OrderType.SELL;
                        }
                        else
                        {
                            type = OrderType.PURCHASE;
                        }

                        Order o = new Order(type, u, reader.GetInt32(3), reader.GetDateTime(4));

                        orders.Add(o);
                        NotifyNewOrder(o);
                    }
                }

                reader.Close();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[loadOrders] Exeception Caught: " + e.Message);
                return;
            }


        }

        public Boolean AddUser(User user)
        {
            Console.WriteLine("addUser() invoked");
            string hashPass = User.GetHashString(user.Pass);

            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT [User] (Username, Password, Name) VALUES ('" + user.Username + "', '" + hashPass + "', '" + user.Name + "');";
                cmd.Connection = con;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[AddUser] Exeception Caught: " + e.Message);
                return false;
            }
            return true;
        }

        public User CheckLogin(string user, string pass)
        {
            User log = null;
            string hashPass = User.GetHashString(pass);

            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT Username, Password, Name FROM [User] WHERE Username = '" + user + "' and Password='" + hashPass + "'";
                cmd.Connection = con;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    con.Close();
                    return null;
                }

                reader.Read();
                log = new User(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                reader.Close();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[CheckLogin] Exeception Caught: " + e.Message);
            }
            return log;
        }

        public int GetUserId(User user)
        {
            int id = -1;

            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT Id FROM [User] WHERE Username = '" + user.Username + "';";
                cmd.Connection = con;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    con.Close();
                    return -1;
                }

                reader.Read();
                id = reader.GetInt32(0);
                reader.Close();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[GetUserId] Exeception Caught: " + e.Message);
            }
            return id;
        }

        public float GetQuote()
        {
            return quote;
        }

        public void SetQuote(float quote)
        {
            this.quote = quote;

            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE [Quote] SET Value=" + quote + " WHERE Id=1";
                cmd.Connection = con;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[GetUserId] Exeception Caught: " + e.Message);
            }

            EventItem item = new EventItem(EventType.QuoteChanged, quote);
            NotifyClients(item);
        }

        void NotifyClients(EventItem item)
        {
            if (ChangedEvent != null)
            {
                Delegate[] delagates = ChangedEvent.GetInvocationList();

                foreach (ChangeEventHandler del in delagates)
                {
                    new Thread(() => {
                        try
                        {
                            del(item);
                            Console.WriteLine("Invoking event handler");
                        }
                        catch (Exception e)
                        {
                            ChangedEvent -= del;
                            Console.WriteLine("Exception: Removed an event handler");
                            Console.WriteLine("Exception: " + e.Message);
                        }
                    }).Start();
                }
            }
        }

        public void CreateDiginote(string serialNumber, User owner)
        {
            Diginote diginote = new Diginote(serialNumber, owner);
            int userId;
            Console.WriteLine("CreateDiginote() invoked");

            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;

                userId = GetUserId(owner);
                if (userId == -1)
                {
                    Console.WriteLine("[CreateDiginote] User does not exist");
                    return;
                }
                Console.WriteLine("[CreateDiginote] Adding Diginote to User id " + userId);

                con.Open();
                cmd.CommandText = "INSERT [Diginote] (SerialNumber, FacialValue, Owner) VALUES ('" + diginote.SerialNumber + "', '" + diginote.FacialValue + "', '" + userId + "');";
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[CreateDiginote] Exeception Caught: " + e.Message);
            }

        }

        public List<Diginote> GetUserDiginotes(User user)
        {
            List<Diginote> diginotes = new List<Diginote>();
            int userId = GetUserId(user);
            Diginote diginote;

            if (userId == -1)
            {
                Console.WriteLine("[GetUserDiginotes] User does not exist");
                return diginotes;
            }

            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT SerialNumber, Owner FROM [Diginote] WHERE Owner = '" + userId + "';";
                cmd.Connection = con;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        diginote = new Diginote(reader.GetString(0), user);
                        diginotes.Add(diginote);
                    }
                    reader.Close();
                }
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[GetUserDiginotes] Exeception Caught: " + e.Message);
            }

            Console.WriteLine("GetUser diginotes of " + user.Username + " total: " + diginotes.Count());

            return diginotes;
        }

        public Boolean ChangeDiginoteOwner(Diginote diginote, int newId)
        {
            Boolean success = false;

            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE Diginote SET Owner = " + newId.ToString() + " WHERE SerialNumber = '" + diginote.SerialNumber + "'; ";
                cmd.Connection = con;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                success = true;
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[ChangeDiginoteOwner] Exeception Caught: " + e.Message);
            }


            return success;
        }

        public List<Order> GetOrders()
        {
            return orders;
        }

        public void AddOrder(Order order)
        {
            Boolean FoundTransaction = false;
            int i = 0;
            foreach (Order el in orders)
            {
                if (el.Type != order.Type && !el.Owner.Username.Equals(order.Owner.Username) && el.Available)
                {
                    FoundTransaction = true;
                    int aux = order.Quantity;
                    Boolean transaction = DoTransaction(order, el);
                    order.Quantity = order.Quantity - el.Quantity;
                    int temp = el.Quantity - aux;
                    el.Timestamp = orders.ElementAt(i).Timestamp;
                    orders.RemoveAt(i);
                    if (temp > 0)
                    {
                        Order old = new Order(el.Type, el.Owner, el.Quantity);
                        old.Timestamp = el.Timestamp;
                        el.Quantity = temp;
                        NotifyIncompleteOrder(el, old);
                        UpdateOrderInDatabase(el);
                        el.Timestamp = DateTime.Now;
                        orders.Add(el);
                    }
                    else
                    {
                        NotifyCompleteOrder(el);
                        RemoveOrderFromDatabase(el);
                    }
                    break;
                }
                i++;
                if (order.Quantity <= 0)
                    break;
            }

            if (!FoundTransaction)
            {
                orders.Add(order);
                AddOrderToDatabase(order);
                NotifyNewOrder(order);
            }
            else
            {
                if (order.Quantity > 0)
                {
                    AddOrder(order);
                }
                else
                {
                    NotifyCompleteOrder(order);
                }
            }
        }

        public void AddOrderToDatabase(Order order)
        {
            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;

                string type;

                if (order.Type == OrderType.PURCHASE)
                {
                    type = "PURCHASE";
                }
                else
                {
                    type = "SELL";
                }

                int available = 1;
                if (!order.Available)
                    available = 0;

                con.Open();
                cmd.CommandText = "INSERT [Order] (Owner, Type, Quantity, TimeStamp, Available) VALUES ('" + order.Owner.Username + "', '" + type + "', " + order.Quantity + ", '" + order.Timestamp.ToString() + "', " + available + ");";
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[AddOrderToDatabase] Exeception Caught: " + e.Message);
            }
        }

        public void UpdateOrderInDatabase(Order order)
        {
            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;

                string type;
                if (order.Type == OrderType.PURCHASE)
                {
                    type = "PURCHASE";
                }
                else
                {
                    type = "SELL";
                }

                con.Open();
                cmd.CommandText = "UPDATE [Order] SET Quantity= " + order.Quantity + ", TimeStamp='" + DateTime.Now + "' WHERE Owner='" + order.Owner.Username + "' AND Type='" + type + "' AND TimeStamp='" + order.Timestamp.ToString() + "';";
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[UpdateOrderInDatabase] Exeception Caught: " + e.Message);
            }
        }

        public void RemoveOrderFromDatabase(Order order)
        {
            try
            {
                //DATABASE
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;

                string type;
                if (order.Type == OrderType.PURCHASE)
                {
                    type = "PURCHASE";
                }
                else
                {
                    type = "SELL";
                }

                con.Open();
                cmd.CommandText = "DELETE FROM [Order] WHERE Owner='" + order.Owner.Username + "' AND TimeStamp='" + order.Timestamp.ToString() + "'AND Type='" + type + "';";
                Console.WriteLine(cmd.CommandText);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[RemoveOrderFromDatabase] Exeception Caught: " + e.Message);
            }
        }

        public List<Order> GetUserPendingOrders(User user)
        {
            List<Order> pending = new List<Order>();

            foreach (Order order in this.orders)
            {
                if (order.Owner.Username.Equals(user.Username))
                    pending.Add(order);
            }

            return pending;
        }

        public void NotifyIncompleteOrder(Order order, Order newOrder)
        {
            EventItem item = new EventItem(EventType.IncompleteOrder, order, newOrder);
            NotifyClients(item);
        }

        public void NotifyCompleteOrder(Order order)
        {
            EventItem item = new EventItem(EventType.CompleteOrder, order);
            NotifyClients(item);
        }

        public void NotifyNewOrder(Order order)
        {
            EventItem item = new EventItem(EventType.NewOrder, order);
            NotifyClients(item);
        }

        public Boolean DoTransaction(Order order1, Order order2)
        {
            int newId = -1;
            int quantity = Math.Min(order1.Quantity, order2.Quantity);
            List<Diginote> diginotes = new List<Diginote>();
            string from, to, date;

            if (order1.Timestamp > order2.Timestamp)
                date = order1.Timestamp.ToString();
            else
                date = order2.Timestamp.ToString();

            if (order1.Type == OrderType.PURCHASE)
            {
                newId = GetUserId(order1.Owner);
                diginotes = GetUserDiginotes(order2.Owner);
                to = order1.Owner.Username;
                from = order2.Owner.Username;
            }
            else
            {
                newId = GetUserId(order2.Owner);
                diginotes = GetUserDiginotes(order1.Owner);
                from = order1.Owner.Username;
                to = order2.Owner.Username;
            }

            if (quantity > diginotes.Count)
                return false;

            Boolean success = false;
            for (int i = 0; i < quantity; i++)
            {
                success = ChangeDiginoteOwner(diginotes[i], newId);
            }

            string log = "Transaction: FROM " + from + " TO " + to + " QUANTITY " + quantity + " AT " + date;

            string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\log.txt";

            if (!File.Exists(path))
                File.Create(path);

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine(log);
                w.Close();
            }
            //AddTransactionLog(log);

            return success;
        }

        public void AddTransactionLog(string log)
        {
            string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\log.txt";

            if (!File.Exists(path))
                File.Create(path);

            using (StreamWriter w = File.AppendText(path))
            {
                w.WriteLine(log);
                w.Close();
            }
        }

        public void cancelPendingOrder(Order order)
        {
            int i = 0;
            foreach (Order o in new List<Order>(orders))
            {
                if (o.Owner.Username.Equals(order.Owner.Username) && o.Timestamp == order.Timestamp)
                    orders.RemoveAt(i);
                i++;
            }
        }

        public void NotifyDeleteOrder(Order order)
        {
            EventItem item = new EventItem(EventType.DeleteOrder, order);
            NotifyClients(item);
        }

        public void ChangeAvailabilityOrders(User user, Boolean availability)
        {
            foreach (Order o in orders)
            {
                if (o.Owner.Username.Equals(user.Username))
                    o.Available = availability;
            }
        }
    }
}
