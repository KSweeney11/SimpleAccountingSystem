// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    GLTransaction.cs
// Description:	Structure of GL Header Transaction
// =============================================
using System;
using System.Collections.Generic;
namespace DataLayer
{
    public class GLTransaction : AccountTransaction
    {
        public DateTime PstDate { get; set; }
        public IList<GLDistribution> Distributions { get; set; }
        public GLTransaction()
        {
            PstDate = System.DateTime.Today;
            Distributions = new List<GLDistribution>();
        }
        public double DistributionVariance()
        {
            double variance = 0;
            foreach (GLDistribution d in Distributions)
                variance += d.DR - d.CR;
            return (double) variance;
        }
        public override string ToString()
        {
            string s = string.Format("Date: {0:d}\tJE: {1}\tJournal Description: {2}",PstDate, TrxID, TrxDscr);
            foreach (GLDistribution d in Distributions)
                s += "\n\t" + d.ToString();
            s += "\n";
            return s;
        }
    }
}
