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
    public partial class ConfirmOrders : Form
    {
        User user;
        List<Order> pendingOrders;
        float quote;
        IRegistry r;
        private static System.Timers.Timer aTimer;

        public ConfirmOrders(User user)
        {
            InitializeComponent();
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 60000;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;

            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            this.user = user;
            quote = r.GetQuote();
            pendingOrders = r.GetUserPendingOrders(user);

            foreach (Order order in pendingOrders)
            {
                ListViewItem item = new ListViewItem(order.Type.ToString());
                item.SubItems.Add(order.Quantity.ToString());
                item.SubItems.Add((order.Quantity * quote).ToString());
                confirmView.Items.Add(item);
            }

            //Start Timer
            aTimer.Enabled = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (confirmView.SelectedItems.Count > 0)
            {

                int index = confirmView.Items.IndexOf(confirmView.SelectedItems[0]);
                confirmView.Items.Remove(confirmView.SelectedItems[0]);
                Order o = pendingOrders[index];
                pendingOrders.RemoveAt(index);
                r.NotifyDeleteOrder(o);
            }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate () { this.Close(); });
            }
            else
            {
                this.Close();
            }
        }
    }
}
