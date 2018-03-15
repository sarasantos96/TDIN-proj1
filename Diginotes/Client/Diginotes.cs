using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Registry;
using Common;
namespace Client
{
    public partial class Diginotes : Form
    {
        Registry.Registry r;
        public Diginotes()
        {
            InitializeComponent();
            r = new Registry.Registry();
            this.Text = "Diginotes";
            TextBox passwordBox = this.Controls["textboxpassword"] as TextBox;
            passwordBox.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Boolean logResult = false;
            TextBox usernameBox = this.Controls["textboxusername"] as TextBox;
            TextBox passwordBox = this.Controls["textboxpassword"] as TextBox;
            if (!usernameBox.Text.Equals("") && !passwordBox.Text.Equals(""))
                logResult = r.CheckLogin(usernameBox.Text, passwordBox.Text);

            if (logResult)
            {
                this.Close();
                new Form1().Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            new SignUp().Show();
        }
    }
}
