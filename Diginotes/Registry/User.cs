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

        public Registry()
        {
            Console.WriteLine("Registry constructor evoked");
            loadUsers();
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

            string u = user.Name + ";" + user.Pass + ";" + user.Quote+"\r\n";
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var dir = System.IO.Path.GetDirectoryName(path) + "\\userlog.txt";
            using (StreamWriter sw = File.AppendText(dir))
            {
                sw.Write(u);
                sw.Close();
            }
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

        public void loadUsers()
        {
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var dir = System.IO.Path.GetDirectoryName(path) + "\\userlog.txt";
            string line;

            if (!File.Exists(dir))
            {
                File.Create(dir).Dispose();
            }
            else
            {
                System.IO.StreamReader file = new StreamReader(dir);

                if (file != null)
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] args = line.Split(';');
                        User user = new User(args[0], args[1], Int32.Parse(args[2]));
                        users.Add(user);
                    }
                }
                file.Close();
            }
        }

    }
}
