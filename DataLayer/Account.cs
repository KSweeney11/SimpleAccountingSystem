// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    Account.cs
// Description:	Abstract Class for GL and Bank
// =============================================
namespace DataLayer
{
    public abstract class Account
    {
        public int  ActIndx { get; set; }
        public string ActNmbr { get; set; }
        public string ActDscr { get; set; }
        public double ActBal { get; set; }
        public bool   IsActive { get; set; }
        public Account()
        {
            ActIndx  = -1;
            ActBal   =  0;
            IsActive = true;
        }
        public override string ToString()
        {
            return "[" + ActNmbr + "] " + ActDscr;
        }
    }
}
