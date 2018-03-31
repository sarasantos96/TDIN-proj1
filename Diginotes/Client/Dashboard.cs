using System.Collections.Generic;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class Dashboard : Form
    {
        IRegistry r;
        User UserLogged;
        EventIntermediate intermediate;
        Label QuoteLabel;
        public Dashboard(User user)
        {
            this.Text = "Diginotes";
            InitializeComponent();

            UserLogged = user;
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            intermediate = new EventIntermediate(r);
            intermediate.newQuote += OnQuoteChanged;

            //Set initial quote value
            QuoteLabel = (Label)this.Controls["quoteLabelValue"] as Label;
            QuoteLabel.Text = r.GetQuote().ToString();

            //Set inital pending orders
            List<Order> pendingOrders = r.GetUserPendingOrders(UserLogged);
            foreach (Order order in pendingOrders)
            {
                ListViewItem item = new ListViewItem(order.Type.ToString());
                item.SubItems.Add(order.Quantity.ToString());
                pendingView.Items.Add(item);
            }
        }

        void OnQuoteChanged(int quote)
        {
            //Labels can't be modified in a different thread they were created
            if (this.QuoteLabel.InvokeRequired)
            {
                this.QuoteLabel.BeginInvoke((MethodInvoker)delegate () { this.QuoteLabel.Text = quote.ToString(); });
            }
            else
            {
                this.QuoteLabel.Text = quote.ToString();
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Wallet(UserLogged).Show();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            new SellOrder(UserLogged).Show();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            new PurchaseOrder(UserLogged).Show();
        }
    }
}
