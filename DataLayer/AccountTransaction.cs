// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    AccountTransaction.cs
// Description:	Abstraction for Header/Dist.
// =============================================
namespace DataLayer
{
    public abstract class AccountTransaction
    {
        public int  TrxID   { get; set; }
        public string TrxDscr { get; set; }
    }
}
