// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    App.xaml.cs
// Description:	Common functions accross project
// =============================================
using System.Windows;
using DataLayer;
namespace SimpleAccountingSystem
{
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            new GUI.Dashboard().Show();
        }
        static public bool DeleteGLAccount(GLAccount tempGL)
        {
            if (tempGL == null)
                return false;
            if (MessageBox.Show("Delete Account " + tempGL.ToString() + "?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (new DataLayer.Repostiory().Remove(tempGL.ActIndx) == -1)
                {
                    MessageBox.Show(tempGL.ToString() + " Deleted.");
                    return true;
                }
                else
                    MessageBox.Show("Deletion failed. Check GL Transactions for any activity against this account.\n\nAccounts used in GL Transactions cannot be deleted.");
            }
            return false;
        }
    }
}