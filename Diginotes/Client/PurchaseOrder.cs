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
        float quote;
        public PurchaseOrder(User user)
        {
            InitializeComponent();
            this.Text = "Buy Diginotes";

            numericUpDown.Minimum = 1;
            numericUpDown.Maximum = decimal.MaxValue;

            User = user;
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));

            quote = r.GetQuote();
            quotelabel1.Text = quote.ToString();

            pricelabel4.Text = ((float)numericUpDown.Value * quote).ToString() + " €";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int qnt = (int)numericUpDown.Value;
            r.AddOrder(new Order(OrderType.PURCHASE, User, qnt));
            this.Close();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            pricelabel4.Text = (quote * (float)numericUpDown.Value).ToString() + " €";
        }
    }
}
