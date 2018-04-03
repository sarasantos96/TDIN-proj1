using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ChangeOrder : Form
    {
        Order Order;
        float quote;
        IRegistry r;

        public ChangeOrder(Order order)
        {
            InitializeComponent();

            Order = order;
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            quote = r.GetQuote();

            typeLabel.Text = order.Type.ToString();
            qntlabel.Text = order.Quantity.ToString();
            newpriceupanddown.Minimum = 0;
            newpriceupanddown.Maximum = decimal.MaxValue;
            newpriceupanddown.Value = (decimal) (order.Quantity * quote);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float newPrice = (int) newpriceupanddown.Value;
            float newquote = (newPrice * quote) / (this.Order.Quantity * quote);
            r.SetQuote(newquote);
            this.Close();
        }
    }
}
