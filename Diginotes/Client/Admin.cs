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
    public partial class Admin : Form
    {
        IRegistry r;
        List<Order> pendingOrders;
        EventIntermediate intermediate;
        
        public Admin()
        {
            InitializeComponent();

            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            intermediate = new EventIntermediate(r);
            intermediate.newEvent += OnChangeEvent;

            //Init Pending Orders View
            pendingOrders = r.getPendingOrders();
            foreach (Order order in pendingOrders)
            {
                ListViewItem item = new ListViewItem(order.Type.ToString());
                item.SubItems.Add(order.Quantity.ToString());
                listView1.Items.Add(item);
            }

            //Set initial quote
            currentQuote.Text = r.GetQuote().ToString();
        }

        public void OnChangeEvent(EventItem item)
        {
            switch (item.Type)
            {
                case EventType.QuoteChanged:
                    OnQuoteChanged(item.Quote);
                    break;

                case EventType.NewOrder:
                    OnNewOrder(item.Order);
                    break;

                case EventType.CompleteOrder:
                    OnCompleteOrder(item.Order);
                    break;

                case EventType.IncompleteOrder:
                    OnIncompleteOrder(item.Order, item.OldOrder);
                    break;

                case EventType.DeleteOrder:
                    OnDeleteOrder(item.Order);
                    break;

                default:
                    break;
            }
        }

        public int GetOrderPos(Order order)
        {
            int i = 0;
            foreach (Order o in pendingOrders)
            {
                int compareDates = DateTime.Compare(o.Timestamp, order.Timestamp);
                if (o.Owner.Username.Equals(order.Owner.Username) && compareDates == 0 && o.Type == order.Type)
                    return i;
                i++;
            }

            return -1;
        }

        public void OnQuoteChanged(float quote)
        {
           
            if (currentQuote.InvokeRequired)
            {
                currentQuote.BeginInvoke((MethodInvoker)delegate () { currentQuote.Text = quote.ToString(); });
            }
            else
            {
                currentQuote.Text = quote.ToString();
            }
        }

        public void OnNewOrder(Order order)
        {
            ListViewItem i = new ListViewItem(order.Type.ToString());
            i.SubItems.Add(order.Quantity.ToString());

            if (listView1.InvokeRequired)
            {
                listView1.BeginInvoke((MethodInvoker)delegate () { listView1.Items.Add(i); pendingOrders.Add(order); });
            }
            else
            {
                listView1.Items.Add(i);
                pendingOrders.Add(order);
            }
        }

        public void OnCompleteOrder(Order order)
        {
            int pos = GetOrderPos(order);
            if (pos != -1)
            {
                if (listView1.InvokeRequired)
                {
                    listView1.BeginInvoke((MethodInvoker)delegate () { listView1.Items[pos].Remove(); pendingOrders.RemoveAt(pos); });
                }
                else
                {
                    listView1.Items[pos].Remove();
                    pendingOrders.RemoveAt(pos);
                }
            }
        }

        public void OnIncompleteOrder(Order order, Order oldOrder)
        {
            int pos = GetOrderPos(oldOrder);
            ListViewItem i = new ListViewItem(order.Type.ToString());
            i.SubItems.Add(order.Quantity.ToString());

            if (pos != -1)
            {
                if (listView1.InvokeRequired)
                {
                    listView1.BeginInvoke((MethodInvoker)delegate () {
                        listView1.Items[pos].Remove();
                        pendingOrders.RemoveAt(pos);
                        listView1.Items.Add(i);
                        pendingOrders.Add(order);
                    });
                }
                else
                {
                    listView1.Items[pos].Remove();
                    pendingOrders.RemoveAt(pos);
                    listView1.Items.Add(i);
                    pendingOrders.Add(order);
                }
            }
        }

        public void OnDeleteOrder(Order order)
        {
            int pos = GetOrderPos(order);
            if (pos != -1)
            {

                if (listView1.InvokeRequired)
                {
                    listView1.BeginInvoke((MethodInvoker)delegate () { pendingOrders.RemoveAt(pos); listView1.Items[pos].Remove(); });
                }
                else
                {
                    pendingOrders.RemoveAt(pos);
                    listView1.Items[pos].Remove();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new CreateDiginote().Show();
        }
    }
}
