using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Common;

namespace Registry
{
    public class Client
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class DatabaseContext : DbContext
    {
        public DbSet<Client> Users { get; set; }
    }
}
