using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Common;
using System.Windows.Controls;

namespace Client
{
    public partial class Wallet : Form
    {
        IRegistry r;
        public Wallet(User user)
        {
            InitializeComponent();

            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            List<Diginote> diginotes = r.GetUserDiginotes(user);
            
            foreach(Diginote diginote in diginotes)
            {
                ListViewItem item = new ListViewItem(diginote.SerialNumber);
                listView.Items.Add(item);
            }

            Label totalLabel = this.Controls["totalLabel"] as Label;
            totalLabel.Text = diginotes.Count.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
