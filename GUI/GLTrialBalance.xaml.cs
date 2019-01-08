// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    GLTransactions.xaml.cs
// Description:	Display Trial Balance through SP
// =============================================
using System;
using System.Windows;
namespace GUI
{
    public partial class GLTrialBalance : Window
    {
        public GLTrialBalance(DateTime dtStart, DateTime dtEnd)
        {
            InitializeComponent();
            lbDateRange.Content = dtStart.ToString("MM/dd/yyyy") + " to " + dtEnd.ToString("MM/dd/yyyy");
            DataLayer.Repostiory repo = new DataLayer.Repostiory();
            dgGLTrialBalance.DataContext = repo.GetTrialBalance(dtStart, dtEnd).Tables[0];
            dgGLTrialBalance.IsReadOnly = true;
        }
    }
}
