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
    public partial class SellOrder : Form
    {
        IRegistry r;
        User User;
        int maxQuantity;

        public SellOrder(User user)
        {
            InitializeComponent();
            this.Text = "Sell Diginotes";

            User = user;
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));

            quotelabel.Text = r.GetQuote().ToString();
            maxQuantity = r.GetUserDiginotes(User).Count;

            numericUpDown1.Maximum = maxQuantity;
            numericUpDown1.Minimum = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int qnt = (int) numericUpDown1.Value;
            r.AddOrder(new Order(OrderType.SELL, User, qnt));
            this.Close();
        }
    }
}
