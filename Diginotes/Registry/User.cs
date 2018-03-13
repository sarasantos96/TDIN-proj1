using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
