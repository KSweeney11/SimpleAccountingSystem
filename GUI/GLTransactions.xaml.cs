// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    GLTransactions.xaml.cs
// Description:	GL JE Bindings, Validation
// =============================================
using System;
using System.Linq;
using System.Windows;
using DataLayer;
namespace GUI
{
    public partial class GLTransactions : Window
    {
        DateTime PstDate = DateTime.Today;
        string TrxDscr;
        static DataLayer.Repostiory repo = new DataLayer.Repostiory();
        public static GLTransaction Trx = new GLTransaction();
        public GLTransactions()
        {
            InitializeComponent();
            dpPstDate.SelectedDate = PstDate;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Trx = new GLTransaction();
            for (int i = 0; i < 2; i++)
                Trx.Distributions.Add(new GLDistribution());
            dgTransaction.ItemsSource = Trx.Distributions;
        }
        private void BtSubmit_Click (object sender, RoutedEventArgs e)
        {
            TrxDscr = tbTrxDscr.Text;
            if (dpPstDate.SelectedDate == null)
            {
                MessageBox.Show("Posting Date cannot be empty.");
                return;
            }
            if (string.IsNullOrEmpty(TrxDscr))
            {
                MessageBox.Show("Journal Header Description cannot be empty.");
                return;
            }
            Trx.PstDate = (DateTime)dpPstDate.SelectedDate;
            Trx.TrxDscr = tbTrxDscr.Text;
            if (CheckDistributions())
            {
                repo.AddGLTransaction(Trx);
                MessageBox.Show("Posted! " + Trx.ToString());
                Close();
            }
            else
                MessageBox.Show("One or more distributions have an error.");
        }
        private bool CheckDistributions()
        {
            double sumDR = 0;
            //Validating content of each GL Transaction
            foreach (GLDistribution d in Trx.Distributions)
            {
                if(d.GL == null)
                {
                    MessageBox.Show("Missing Account:\n" + d.ToString());
                    return false;
                }
                if (d.DR != 0 && d.CR != 0)
                {
                    MessageBox.Show("Only a debit or credit should be entered for each distribution:\n" + d.ToString());
                    return false;
                }
                if(d.DR < 0 || d.CR < 0)
                {
                    MessageBox.Show("Debits and Credits should be no less than zero:\n" + d.ToString());
                    return false;
                }
                if (!d.GL.IsActive)
                {
                    MessageBox.Show("Inactive Account Selected: " + d.GL.ToString());
                    return false;
                }
                if(!d.GL.CanPstDrct)
                {
                    MessageBox.Show("Direct Posting Disabled: " + d.GL.ToString());
                    return false;
                }
                d.ActIndx = d.GL.ActIndx;
                if (string.IsNullOrEmpty(d.TrxDscr))
                {
                    MessageBox.Show("Missing GJ Distribution Description:\n" + d.ToString());
                    return false;
                }
                sumDR += d.DR;
                d.GL = repo.Find(d.ActNmbr);
            }
            if(sumDR <= 0)
            {
                MessageBox.Show("Distributions must positive have amounts.");
                return false;
            }
            double var = Trx.DistributionVariance();
            if (var == 0)
                return true;
            MessageBox.Show(String.Format("Sum of Debits must equal sum of Credits\n[Variance = {0:C}].",var));
            return false;
        }
        private void BtSelectGL_Click(object sender, RoutedEventArgs e)
        {
            GLSearch w = new GLSearch();
            w.Owner = this;
            w.ShowDialog();            
            GLDistribution dist = dgTransaction.SelectedItems.OfType<GLDistribution>().First();
            if(w.myGL != null)
            {
                dist.GL = w.myGL;
                dist.ActNmbr = dist.GL.ActNmbr;
                dist.ActIndx = dist.GL.ActIndx;
                dgTransaction.Items.Refresh();
            }
        }
        private void BtAddRows_Click(object sender, RoutedEventArgs e)
        {
            Trx.Distributions.Add(new GLDistribution());
            dgTransaction.Items.Refresh();
        }
    }
}