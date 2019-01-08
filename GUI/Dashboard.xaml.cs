// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    Dashboard.xaml.cs
// Description:	Main window events/bindings
// =============================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataLayer;
namespace GUI
{
    public partial class Dashboard : Window
    {
        IList<GLAccount> GLAccounts;
        DateTime startDate, endDate;
        public Dashboard()
        {
            InitializeComponent();
            //Using Prior month for default start/end dates on reports
            DateTime dt = DateTime.Today.AddMonths(-1);
            int year = dt.Year;
            int month = dt.Month;
            startDate = new DateTime(year, month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            dtStart.SelectedDate = startDate;
            dtEnd.SelectedDate = endDate;
        }
        private void DtStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtStart.SelectedDate == null || dtEnd.SelectedDate == null)
                return;
            //To prevent reversed start/end dates, update End to match Start.
            if (dtStart.SelectedDate > dtEnd.SelectedDate)
            {
                dtEnd.SelectedDate = dtStart.SelectedDate;
                endDate = (DateTime)dtEnd.SelectedDate;
            }
            startDate = (DateTime)dtStart.SelectedDate;
        }
        private void DtEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtStart.SelectedDate == null || dtEnd.SelectedDate == null)
                return;
            //To prevent reversed start/end dates, update Start to match End.
            if (dtStart.SelectedDate > dtEnd.SelectedDate)
            {
                dtStart.SelectedDate = dtEnd.SelectedDate;
                startDate = (DateTime)dtStart.SelectedDate;
            }
            endDate = (DateTime)dtEnd.SelectedDate;
        }
        private void BtCreate_Click(object sender, RoutedEventArgs e)
        {
            new GLSetup().ShowDialog();
            GLAccounts = new Repostiory().GetAll();
            dgGL.ItemsSource = GLAccounts;

        }
        private void BtUpdate_Click(object sender, RoutedEventArgs e)
        {
            var gl = dgGL.SelectedItem;
            new GLSetup(gl as GLAccount).ShowDialog();
            dgGL.Items.Refresh();
        }
        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
            IList<GLAccount> selection = dgGL.SelectedItems.Cast<GLAccount>().ToList();
            bool isDeleted;
            foreach (GLAccount tempGL in selection)
                isDeleted = SimpleAccountingSystem.App.DeleteGLAccount(tempGL);
            GLAccounts = new Repostiory().GetAll();
            dgGL.ItemsSource = GLAccounts;
        }
        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            GLAccounts = new Repostiory().GetAll();
            dgGL.ItemsSource = GLAccounts;
        }
        private void BtSubmit_Click(object sender, RoutedEventArgs e)
        {
            //Prevent null dates
            if (dtStart.SelectedDate == null)
            {
                MessageBox.Show("Missing Start Date.");
                return;
            }
            if (dtEnd.SelectedDate == null)
            {
                MessageBox.Show("Missing End Date.");
                return;
            }
            if (dtStart.SelectedDate > dtEnd.SelectedDate)
            {
                MessageBox.Show("End Date cannot precede Start Date.");
                return;
            }
            if(cbReport.SelectedIndex == 0)
                new GLTrialBalance(startDate, endDate).ShowDialog(); 
            else
                new GLReport(startDate, endDate).ShowDialog();
        }
        private void BtGLJournal_Click(object sender, RoutedEventArgs e)
        {
            new GLTransactions().ShowDialog();
            GLAccounts = new Repostiory().GetAll();
            dgGL.ItemsSource = GLAccounts;
        }
    }
}