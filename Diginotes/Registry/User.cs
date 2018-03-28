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
        public event QuoteChangedEvent QuoteChanged;

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
        }

        public void AddUser(User user)
        {
            Console.WriteLine("addUser() invoked");
            string hashPass = User.GetHashString(user.Pass);

            //DATABASE
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "INSERT [User] (Username, Password, Name) VALUES ('" + user.Username + "', '" + hashPass+ "', '" + user.Name + "');";
            cmd.Connection = con;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public Boolean CheckLogin(string user, string pass)
        {
            Boolean log = false;
            string hashPass = User.GetHashString(pass);

            //DATABASE
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "SELECT Username, Password FROM [User] WHERE Username = '" + user+"' and Password='"+ hashPass +"'";
            cmd.Connection = con;

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                log = true;
            }

            con.Close();
            return log;
        }

        public int GetQuote()
        {
            return quote;
        }

        public void SetQuote(int quote)
        {
            this.quote = quote;
            NotifyClients(quote);
        }

        void NotifyClients(int quote)
        {
            if(QuoteChanged != null)
            {
                Delegate[] delagates = QuoteChanged.GetInvocationList();

                foreach(QuoteChangedEvent del in delagates)
                {
                    new Thread(() => {
                        try
                        {
                            del(quote);
                            Console.WriteLine("Invoking event handler");
                        }
                        catch (Exception)
                        {
                            QuoteChanged -= del;
                            Console.WriteLine("Exception: Removed an event handler");
                        }
                    }).Start();
                }
            }
        }
    }
}
