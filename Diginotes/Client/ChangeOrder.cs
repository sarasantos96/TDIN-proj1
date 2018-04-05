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

        public ChangeOrder(Order order, Boolean message)
        {
            InitializeComponent();

            Order = order;
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            quote = r.GetQuote();

            typeLabel.Text = order.Type.ToString();
            qntlabel.Text = order.Quantity.ToString();
            if(order.Type == OrderType.PURCHASE)
            {
                newpriceupanddown.Minimum = (decimal)(order.Quantity * quote);
                newpriceupanddown.Maximum = decimal.MaxValue;
            }
            else
            {
                newpriceupanddown.Minimum = 1;
                newpriceupanddown.Maximum = (decimal)(order.Quantity * quote);
            }
           
            newpriceupanddown.Value = (decimal) (order.Quantity * quote);

            if (message)
            {

                textBox1.Visible = true;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float newPrice = (int) newpriceupanddown.Value;
            float newquote = (newPrice * quote) / (this.Order.Quantity * quote);
            if(newquote != quote)
                r.SetQuote(newquote);
            this.Close();
        }
    }
}
