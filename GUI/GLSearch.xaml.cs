// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    GLSearch.xaml.cs
// Description:	Search through GL Accounts
// =============================================
using System.Collections.Generic;
using System.Windows;
using DataLayer;
namespace GUI
{
    public partial class GLSearch : Window
    {
        public GLAccount myGL;
        public GLSearch()
        {
            InitializeComponent();
            List<GLAccount> GLAccounts = new DataLayer.Repostiory().GetAll();
            dgGL.ItemsSource = GLAccounts;
        }
        private void BtOK_Click(object sender, RoutedEventArgs e)
        {
            var s = dgGL.SelectedItem;
            GLAccount gl = s as GLAccount;
            if (gl != null)
                myGL = gl;
            this.Close();
        }
    }
}
