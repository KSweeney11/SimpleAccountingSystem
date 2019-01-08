// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    GLDistribution.cs
// Description:	Structure of GL Dist Transaction
// =============================================
namespace DataLayer
{
    public class GLDistribution: AccountTransaction
    {
        public GLAccount GL      { get; set; }
        public int     ActIndx { get; set; }
        public string    ActNmbr { get; set; }
        public double    DR      { get; set; }
        public double    CR      { get; set; }
        public GLDistribution()
        {
            DR = CR = 0;
            ActIndx = -1;
            ActNmbr = "";
            GL = new GLAccount();
        }
        public override string ToString()
        {
            return string.Format("Account: {1:d} DR: {2:C2}\tCR: {3:C2}\t{4}", TrxID, GL.ActNmbr, DR, CR, TrxDscr);
        }
    }
}