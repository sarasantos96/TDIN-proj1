using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Registry;
using Common;
using System.Runtime.Remoting;

namespace Client
{
    public partial class SignUp : Form
    {
        IRegistry r;
        public SignUp()
        {
            InitializeComponent();
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            this.Text = "Sign Up";
            TextBox passwordBox = this.Controls["passwordTextBox"] as TextBox;
            passwordBox.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextBox usernameBox = this.Controls["usernameTextBox"] as TextBox;
            TextBox passwordBox = this.Controls["passwordTextBox"] as TextBox;
            TextBox nameBox = this.Controls["nameTextBox"] as TextBox;

            if(!usernameBox.Text.Equals("") && !passwordBox.Text.Equals("") && !nameBox.Text.Equals(""))
            {
                Boolean success = r.AddUser(new User(usernameBox.Text, passwordBox.Text, nameBox.Text));
                if (success)
                {
                    this.Close();
                    new Diginotes().Show();
                }              
            }
        }
    }
}
