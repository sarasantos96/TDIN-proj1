using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace Client
{   
    public partial class CreateDiginote : Form
    {
        public class Item
        {
            public string Username { get; set; }

            public Item(string username)
            {
                Username = username;
            }

            public override string ToString()
            {
                return Username;
            }
        }

        IRegistry r;
        List<User> users;
        Guid guid;
        public CreateDiginote()
        {
            InitializeComponent();

            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            users = new List<User>(r.GetAllUsernames());
            guid = new Guid();

            numericUpDown1.Minimum = 1;

            foreach(User user in users)
            {
                comboBox1.Items.Add(user.Username);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Display the Value property
            Item itm = (Item)comboBox1.SelectedItem;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string item = (string) comboBox1.SelectedItem;
                int qnt = (int) numericUpDown1.Value;
                for(int i = 0; i < qnt; i++)
                {
                    guid = Guid.NewGuid();
                    r.CreateDiginote(guid.ToString(), new User(item, "", ""));
                }              
            }

            this.Close();
        }
    }
}
