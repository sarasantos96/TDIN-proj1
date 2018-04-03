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
    public partial class PurchaseOrder : Form
    {
        IRegistry r;
        User User;
        public PurchaseOrder(User user)
        {
            InitializeComponent();
            this.Text = "Buy Diginotes";

            numericUpDown.Minimum = 1;

            User = user;
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));

            quotelabel1.Text = r.GetQuote().ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int qnt = (int)numericUpDown.Value;
            r.AddOrder(new Order(OrderType.PURCHASE, User, qnt));
            this.Close();
        }
    }
}
