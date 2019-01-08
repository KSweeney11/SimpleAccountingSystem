// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    GLSetup.xaml.cs
// Description:	GL Setup Events, Triggers, Calcs
// =============================================

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DataLayer;
namespace GUI
{
    public partial class GLSetup : Window
    {
        static DataLayer.Repostiory repo = new DataLayer.Repostiory();
        static IList<GLAccount> GLAccounts = repo.GetAll();
        public GLAccount gl;
        int index = 0;
        int count = GLAccounts.Count()-1;
        public GLSetup()
        {
            InitializeComponent();
        }
        public GLSetup(GLAccount selectedGL)
        {
            InitializeComponent();
            gl = selectedGL;
            DataContext = gl;
        }
        private void BtFirst_Click(object sender, RoutedEventArgs e)
        {
            index = 0;
            gl = GLAccounts[index];
            DataContext = gl;
        }
        private void BtPrevious_Click(object sender, RoutedEventArgs e)
        {
            gl = GLAccounts[index > 0 ? --index : 0];
            DataContext = gl;
        }
        private void BtNext_Click(object sender, RoutedEventArgs e)
        {
            gl = GLAccounts[index < count ? ++index : count];
            DataContext = gl;
        }
        private void BtLast_Click(object sender, RoutedEventArgs e)
        {
            index = count;
            gl = GLAccounts[index];
            DataContext = gl;
        }
        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {
            GLSearch w = new GLSearch();
            w.ShowDialog();
            if (w.myGL != null)
                DataContext = w.myGL;
        }
        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            GLAccount tempGL;
            GLAccounts = repo.GetAll();
            GLAccount findGL = GLAccounts.FirstOrDefault(i => i.ActNmbr == tbActNmbr.Text);
            bool isNew = false;
            if (tbActDscr.Text.Trim()=="" || tbActNmbr.Text.Trim() == "")
            {
                MessageBox.Show("Account Number and Account Descriptions are required fields.");
                return;
            }
            if (findGL == null)
            {
                tempGL = new GLAccount();
                isNew = true;
            }
            else
                tempGL = findGL;
            tempGL.ActNmbr    = tbActNmbr.Text;
            tempGL.ActDscr    = tbActDscr.Text;
            tempGL.IsActive   = (bool) ckIsActive.IsChecked;
            tempGL.CanPstDrct = (bool) ckCanDirPst.IsChecked;
            tempGL.HasDRBal   = (bool)ckHasDRBal.IsChecked;
            tempGL            =  repo.Save(tempGL);
            if (isNew)
            {
                GLAccounts.Add(repo.Save(tempGL));
                count++;
            }
            index = GLAccounts.IndexOf(tempGL);
            MessageBox.Show("Account Saved.");
            DataContext = repo.Find(tempGL.ActNmbr);
        }
        private void BtNew_Click(object sender, RoutedEventArgs e)
        {
            DataContext = null;
        }
        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
            GLAccount tempGL = GLAccounts.FirstOrDefault(i => i.ActNmbr == tbActNmbr.Text);
            bool isDeleted = SimpleAccountingSystem.App.DeleteGLAccount(tempGL);
            if (isDeleted)
            {
                GLAccounts.Remove(tempGL);
                count--;
                DataContext = null;
            }
        }
    }
}