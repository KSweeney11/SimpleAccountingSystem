// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    GLAccount.cs
// Description:	Structure of GL Account
// =============================================
using System;
namespace DataLayer
{
    public class GLAccount : Account
    {
        public bool   CanPstDrct { get; set; }
        public bool   HasDRBal   { get; set; }
        public GLAccount() : base()
        {
            CanPstDrct = true;
            HasDRBal   = true;
        }
        public static implicit operator int(GLAccount v)
        {
            throw new NotImplementedException();
        }
    }
}
