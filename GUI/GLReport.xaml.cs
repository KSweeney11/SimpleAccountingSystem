// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    GLReport.xaml.cs
// Description:	Display GL Transactions by SP
// =============================================
using System;
using System.Windows;

namespace GUI
{
    public partial class GLReport : Window
    {
        
        public GLReport(DateTime dtStart, DateTime dtEnd)
        {
            InitializeComponent();
            lbDateRange.Content = dtStart.ToString("MM/dd/yyyy") + " to " + dtEnd.ToString("MM/dd/yyyy");
            DataLayer.Repostiory repo = new DataLayer.Repostiory();
            DataContext = repo.GetTrxByDate(dtStart, dtEnd).Tables[0];
            dgGLTransactions.IsReadOnly = true;
        }
    }
}
