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
        List<Order> pendingOrders;

        public Dashboard(User user)
        {
            this.Text = "Diginotes";
            InitializeComponent();

            UserLogged = user;
            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            intermediate = new EventIntermediate(r);
            intermediate.newEvent += OnChangeEvent;

            //Set initial quote value
            quoteLabelValue.Text = r.GetQuote().ToString();

            //Set inital pending orders
            pendingOrders = r.GetUserPendingOrders(UserLogged);
            foreach (Order order in pendingOrders)
            {
                ListViewItem item = new ListViewItem(order.Type.ToString());
                item.SubItems.Add(order.Quantity.ToString());
                pendingView.Items.Add(item);
            }
        }

        void OnChangeEvent(EventItem item)
        {
            switch (item.Type)
            {
                case EventType.QuoteChanged:
                    OnQuoteChanged(item.Quote);
                    break;

                case EventType.NewOrder:
                    if (item.Order.Owner.Username.Equals(UserLogged.Username))
                    {
                        OnNewOrder(item.Order);
                    }
                    break;

                case EventType.CompleteOrder:
                    if (item.Order.Owner.Username.Equals(UserLogged.Username))
                    {
                        OnCompleteOrder(item.Order);
                    }
                    break;

                case EventType.IncompleteOrder:
                    if (item.Order.Owner.Username.Equals(UserLogged.Username))
                    {
                        //TODO: change to handle incomplete events
                        OnCompleteOrder(item.Order);
                        OnNewOrder(item.Order);
                    }
                    break;

                case EventType.NewMessage:
                    break;

                default:
                    break;
            }
        }

        void OnQuoteChanged(int quote)
        {
            //Labels can't be modified in a different thread they were created
            if (quoteLabelValue.InvokeRequired)
            {
                quoteLabelValue.BeginInvoke((MethodInvoker)delegate () { quoteLabelValue.Text = quote.ToString(); });
            }
            else
            {
                quoteLabelValue.Text = quote.ToString();
            }

        }

        void OnNewOrder(Order order)
        {
            ListViewItem i = new ListViewItem(order.Type.ToString());
            i.SubItems.Add(order.Quantity.ToString());

            if (pendingView.InvokeRequired)
            {
                pendingView.BeginInvoke((MethodInvoker)delegate () { pendingView.Items.Add(i); pendingOrders.Add(order); });
            }
            else
            {
                pendingView.Items.Add(i);
                pendingOrders.Add(order);
            }
        }

        void OnCompleteOrder(Order order)
        {
            int pos = GetOrderPos(order);
            if (pos != -1)
            {
                if (pendingView.InvokeRequired)
                {
                    pendingView.BeginInvoke((MethodInvoker)delegate () { pendingView.Items[pos].Remove(); pendingOrders.RemoveAt(pos); });
                }
                else
                {
                    pendingView.Items[pos].Remove();
                    pendingOrders.RemoveAt(pos);
                }
            }                
        }

        public int GetOrderPos(Order order)
        {
            int i = 0;
            foreach (Order o in pendingOrders)
            {
                if (o.Owner.Username.Equals(order.Owner.Username) && o.Timestamp == order.Timestamp && o.Type == order.Type)
                    return i;
                i++;
            }

            return -1;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Wallet(UserLogged).ShowDialog();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            new SellOrder(UserLogged).ShowDialog();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            new PurchaseOrder(UserLogged).ShowDialog();
        }
    }
}
