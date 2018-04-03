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
        int quote;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public Registry()
        {
            Console.WriteLine("Registry constructor evoked");
            con = new System.Data.SqlClient.SqlConnection(Properties.Settings.Default.Database1);
            quote = 1;
            orders = new List<Order>();
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
            }catch(Exception e)
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
            catch(Exception e)
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

        public int GetQuote()
        {
            return quote;
        }

        public void SetQuote(int quote)
        {
            this.quote = quote;
            EventItem item = new EventItem(EventType.QuoteChanged, quote);
            NotifyClients(item);
        }

        void NotifyClients(EventItem item)
        {
            if(ChangedEvent != null)
            {
                Delegate[] delagates = ChangedEvent.GetInvocationList();

                foreach(ChangeEventHandler del in delagates)
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
                if(userId == -1)
                {
                    Console.WriteLine("[CreateDiginote] User does not exist");
                    return;
                }
                Console.WriteLine("[CreateDiginote] Adding Diginote to User id " + userId);

                con.Open();
                cmd.CommandText = "INSERT [Diginote] (SerialNumber, FacialValue, Owner) VALUES ('" + diginote.SerialNumber + "', '" + diginote.FacialValue + "', '" + userId + "');";
                cmd.ExecuteNonQuery();
                con.Close();
            }catch(Exception e)
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

            if(userId == -1)
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
            catch(Exception e)
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                Console.WriteLine("[GetUserDiginotes] Exeception Caught: " + e.Message);
            }

            Console.WriteLine("GetUser diginotes of " + user.Username + " total: " + diginotes.Count());

            return diginotes;
        }

        public List<Order> GetOrders()
        {
            return orders;
        }

        public void AddOrder(Order order)
        {
            Boolean FoundTransaction = false;
            if (!orders.Any())
            {
                orders.Add(order);
                NotifyNewOrder(order);
            }
            else
            {
                int i = 0;
                foreach(Order el in orders)
                {
                    if(el.Type != order.Type && !el.Owner.Username.Equals(order.Owner.Username))
                    {
                        FoundTransaction = true;
                        int aux = order.Quantity;
                        order.Quantity = order.Quantity - el.Quantity;
                        int temp = el.Quantity - aux;
                        orders.RemoveAt(i);
                        if (temp > 0)
                        {                            
                            el.Quantity = temp;
                            orders.Add(el);
                            NotifyIncompleteOrder(el);
                        }
                        else
                        {
                            NotifyCompleteOrder(el);
                        }
                        //TODO fazer a transação
                        break;
                    }
                    i++;
                    if (order.Quantity <= 0)
                        break;
                }

                if (!FoundTransaction)
                {
                    orders.Add(order);
                    NotifyNewOrder(order);
                }
                else
                {
                    //orders.RemoveAll(o => o.Quantity <= 0); NECESSÁRIO??
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
        }

        public List<Order> GetUserPendingOrders(User user)
        {
            List<Order> pending = new List<Order>();

            foreach(Order order in this.orders)
            {
                if (order.Owner.Username.Equals(user.Username))
                    pending.Add(order);
            }

            return pending;
        }

        public void NotifyIncompleteOrder(Order order)
        {
            EventItem item = new EventItem(EventType.IncompleteOrder, order);
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
    }
}
