// =============================================
// Author:		Sweeney, Kieran
// Create date: 16 Dec 2018
// Filename:    Repository.cs
// Description:	Handles SQL-C# Interface
// =============================================
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DataLayer
{
    public class Repostiory
    {
#pragma warning disable IDE0044 // Add readonly modifier
        private IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DataLayer.Properties.Settings.AccountingDBConnectionString"].ConnectionString);
#pragma warning restore IDE0044 // Add readonly modifier  
        public GLAccount Find(int ActIndx)
        {
            return db.Query<GLAccount>($"SELECT * FROM GLAccounts WHERE ActIndx = @id", new { id = ActIndx }).SingleOrDefault();
        }
        public GLAccount Find(string ActNmbr)
        {
            return db.Query<GLAccount>($"SELECT * FROM GLAccounts WHERE ActNmbr = @id", new { id = ActNmbr}).SingleOrDefault();
        }
        public List<GLAccount> GetAll()
        {
            return db.Query<GLAccount>($"SELECT * FROM GLAccounts ORDER BY ActNmbr").ToList();
        }
        public int Remove(int i)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ActIndx", value: i, direction: ParameterDirection.InputOutput);
            db.Execute("GLAccountDelete", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@ActIndx");
        }
        public GLAccount Save(GLAccount gl)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ActIndx", value: gl.ActIndx, direction: ParameterDirection.InputOutput);
            parameters.Add("ActNmbr"    , value: gl.ActNmbr);
            parameters.Add("@ActDscr"   , value: gl.ActDscr);
            parameters.Add("@ActBal"    , value: gl.ActBal, dbType: DbType.Decimal);
            parameters.Add("@CanPstDrct", value: gl.CanPstDrct);
            parameters.Add("@HasDRBal"  , value: gl.HasDRBal);
            parameters.Add("@IsActive"  , value: gl.IsActive);
            db.Execute("GLAccountSave", parameters, commandType: CommandType.StoredProcedure);
            if(gl.ActIndx < 0)
                gl.ActIndx = parameters.Get<int>("@ActIndx");
            return gl; //Return GLAccount (passes back the index for new GLAccount).
        }
        public void AddGLTransaction(GLTransaction t)
        {
            if(t.Distributions == null)
            {
                Console.WriteLine("AddGLTransactions - Empty Distribution List");
                return;
            }
            if (!t.DistributionVariance().Equals(0))
            {
                Console.WriteLine("AddGLTransactions - Unbalanced Distributions");
                return;
            }
            var parameters = new DynamicParameters();
            parameters.Add("@TrxID", value: t.TrxID, direction: ParameterDirection.InputOutput);
            parameters.Add("@PstDate", value: t.PstDate);
            parameters.Add("@TrxDscr", value: t.TrxDscr);
            db.Execute("GLTransactionHdr_Insert", parameters, commandType: CommandType.StoredProcedure);
            t.TrxID = parameters.Get<int>("@TrxID");
            foreach (GLDistribution d in t.Distributions)
            {
                parameters = new DynamicParameters();
                parameters.Add("@TrxID", value: t.TrxID);
                parameters.Add("@DTrxID", value: d.TrxID, direction: ParameterDirection.InputOutput);
                parameters.Add("@ActIndx", value: d.ActIndx);
                parameters.Add("@DR", value: d.DR, dbType: DbType.Decimal);
                parameters.Add("@CR", value: d.CR, dbType: DbType.Decimal);
                parameters.Add("@TrxDscr", value: d.TrxDscr);
                db.Execute("GLTransactionDst_Insert", parameters, commandType: CommandType.StoredProcedure);
                d.TrxID = parameters.Get<int>("@DTrxID");
            }
        }
        public List<GLTransaction> populateDistributions(List<GLTransaction> transactions)
        {
            foreach (GLTransaction t in transactions)
            {
                t.Distributions = db.Query<GLDistribution>("GLDistributionByHdr", new { TrxID = t.TrxID }, commandType: CommandType.StoredProcedure).ToList();
                foreach (GLDistribution d in t.Distributions)
                {
                    if (d.ActIndx == -1)
                    {
                        if (string.IsNullOrEmpty(d.ActNmbr))
                            d.GL = new GLAccount();
                        else
                            d.GL = Find(d.ActNmbr);
                    }
                    else
                        d.GL = Find(d.ActIndx);
                    d.ActIndx = d.GL.ActIndx;
                    d.ActNmbr = d.GL.ActNmbr;
                }
            }
            return transactions;
        }
        public List<GLTransaction> GetAllTrx()
        {
            List<GLTransaction> transactions = db.Query<GLTransaction>($"SELECT * FROM GLTransactionsHdr").ToList();
            return populateDistributions(transactions);
        }
        public DataSet GetTrxByDate(DateTime dtStart, DateTime dtEnd)
        {
            var sqlAdapter = new SqlConnection(ConfigurationManager.ConnectionStrings["DataLayer.Properties.Settings.AccountingDBConnectionString"].ConnectionString);
            var dataAdapter = new SqlDataAdapter($"GLTransactionsByRange '{dtStart}', '{dtEnd}'", sqlAdapter);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            return ds;
        }
        public DataSet GetTrialBalance(DateTime dtStart, DateTime dtEnd)
        {
            var sqlAdapter = new SqlConnection(ConfigurationManager.ConnectionStrings["DataLayer.Properties.Settings.AccountingDBConnectionString"].ConnectionString);
            var dataAdapter = new SqlDataAdapter($"GLTrialBalance '{dtStart}', '{dtEnd}'", sqlAdapter);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            return ds;
        }
    }
}