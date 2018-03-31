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
        IRegistry r;
        public Diginotes()
        {
            this.Text = "Diginotes - Login";
            InitializeComponent();
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            this.Text = "Diginotes";
            TextBox passwordBox = this.Controls["textboxpassword"] as TextBox;
            passwordBox.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            User logResult = null;
            TextBox usernameBox = this.Controls["textboxusername"] as TextBox;
            TextBox passwordBox = this.Controls["textboxpassword"] as TextBox;
            if (!usernameBox.Text.Equals("") && !passwordBox.Text.Equals(""))
                logResult = r.CheckLogin(usernameBox.Text, passwordBox.Text);

            if (logResult != null)
            {
                this.Close();
                new Dashboard(logResult).Show();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            new SignUp().Show();
        }
    }
}
