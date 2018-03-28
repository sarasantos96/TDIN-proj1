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
    public partial class Form1 : Form
    {
        IRegistry r;
        EventIntermediate intermediate;
        Label quoteLabel;

        public Form1()
        {
            InitializeComponent();

            r = (IRegistry)RemoteNew.New(typeof(IRegistry));
            intermediate = new EventIntermediate(r);
            intermediate.newQuote += OnQuoteChanged;

            //Set initial quote value
            quoteLabel = (Label) this.Controls["quoteValueLabel"] as Label;
            quoteLabel.Text = r.GetQuote().ToString();
        }

        void OnQuoteChanged(int quote)
        {
            if (this.quoteLabel.InvokeRequired)
            {
                this.quoteLabel.BeginInvoke((MethodInvoker)delegate () { this.quoteLabel.Text = quote.ToString(); ; });
            }
            else
            {
                this.quoteLabel.Text = quote.ToString();
            }
            
        }

    }
}
