using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Registry
{
    public class Registry : MarshalByRefObject
    {
        ArrayList users = new ArrayList();
        System.Data.SqlClient.SqlConnection con;

        public Registry()
        {
            Console.WriteLine("Registry constructor evoked");
            con = new System.Data.SqlClient.SqlConnection(Properties.Settings.Default.Database1);
        }

        public ArrayList getUsers()
        {
            Console.WriteLine("getUsers() invoked");
            return users;
        }

        public void AddUser(User user)
        {
            Console.WriteLine("addUser() invoked");
            users.Add(user);

            //DATABASE
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "INSERT [User] (Username, Password) VALUES ('" + user.Name + "', '" + user.Pass + "');";
            cmd.Connection = con;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public Boolean CheckLogin(string user, string pass)
        {
            foreach(User u in users)
            {
                if (user.Equals(u.Name) && pass.Equals(u.Pass))
                    return true;
            }

            return false;
        }
    }
}
