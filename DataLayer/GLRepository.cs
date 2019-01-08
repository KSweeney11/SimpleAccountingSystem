using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAccountingSystem;
using Dapper;

namespace DataLayer.Dapper
{
    public class GLRepository
    {
#pragma warning disable IDE0044 // Add readonly modifier
        private IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SimpleAccountingSystem.Properties.Settings.AcctDBConnection"].ConnectionString);
#pragma warning restore IDE0044 // Add readonly modifier
        public GLAccount Add(GLAccount gl)
        {
            var sql = "INSERT INTO GLAccounts @ActNmbr, @ActDscr, @ActBal, @CanPstDrct, @HasDRBal, @IsActive;"
                    + "SELECT CAST(SCOPE_IDENTITY() AS INT)";
            var indx = this.db.Query<GLAccount>(sql, gl).Single();
            gl.ActIndx = indx;
            return gl;
        }
        public GLAccount Find(int ActIndx)
        {
            return this.db.Query<GLAccount>($"SELECT * FROM GLAccounts WHERE ActIndx = @id", new { id = ActIndx }).SingleOrDefault();
        }
        public List<GLAccount> GetAll()
        {
            return this.db.Query<GLAccount>($"SELECT * FROM GLAccounts").ToList();
        }
        public List<GLAccount> ListAcctByCriterion(string s)
        {
            string sql = "SELECT ActIndx AS 'Index', ActNmbr AS 'Account', ActDscr AS 'Description'" +
                         "FROM GLAccounts" +
                         "WHERE IsActive = 1 AND " +
                         $"(ActNmbr LIKE '%{ s }%' OR ActDscr LIKE '%{ s }%'";
            return this.db.Query<GLAccount>(sql).ToList();
        }
        public void Remove(int ActIndx)
        {
            //this.db.Execute("DELETE FROM GLAccounts WHERE ActIndx = @id", new {id = ActIndx});
            this.db.Execute("GLAccountDelete @id", new { id = ActIndx }, commandType: CommandType.StoredProcedure);
        }

        public GLAccount Update(GLAccount gl)
        {
            var sql = "UPDATE GLAccounts" +
                      "SET ActNumbr   = @ActNmbr, " +
                      "    ActDscr    = @ActDscr, " +
                      "    CanPstDrct = @CanPstDrct, " +
                      "    HasDRBal   = @HasDRBal, " +
                      "    IsActive   = @IsActive " +
                      "WHERE ActIndx  = @ActIndx";
            this.db.Execute(sql, gl);
            return gl;
        }
        public void Save(GLAccount gl)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ActIndx"   , value: gl.ActIndx, direction: ParameterDirection.InputOutput);
            parameters.Add("ActNmbr"    , value: gl.ActNmbr);
            parameters.Add("@ActDscr"   , value: gl.ActDscr);
            parameters.Add("@ActBal"    , value: gl.ActBal , dbType: DbType.Decimal);
            parameters.Add("@CanPstDrct", value: gl.CanPstDrct);
            parameters.Add("@HasDRBal"  , value: gl.HasDRBal);
            parameters.Add("@IsActive"  , value: gl.IsActive);
            this.db.Execute("SaveGLAccount", parameters, commandType: CommandType.StoredProcedure);
            gl.ActIndx = parameters.Get<int>("@ActIndx");
        }
        public int AddGJHeader(DateTime d, string s)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TrxID", value:0, direction: ParameterDirection.InputOutput);
            parameters.Add("@PstDate", value: d);
            parameters.Add("@TrxDscr", value: s);
            this.db.Execute("GLTransactionHdr_Insert", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@TrxID");
        }
        //Two experimental SP for transaction distribution insertion and list of GL trx by date range.
        public void AddDst(GLTransactionsDst d)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TrxID", value: d.TrxID);
            parameters.Add("@ActIndx", value: d.ActIndx);
            parameters.Add("@DR", value: d.DR);
            parameters.Add("@CR", value: d.CR);
            parameters.Add("@TrxDscr", value: d.TrxDscr);
            this.db.Execute("GLTransactionDst_Insert", parameters, commandType: CommandType.StoredProcedure);
        }
        public List<GLTransactionsDst> GetTransactionsByDate(DateTime dtStart, DateTime dtEnd)
        {
            string sql = "SELECT * FROM [dbo].[GLTransactions] " +
                        $"WHERE[Posting Date] BETWEEN '{ dtStart }' AND IIF('{ dtEnd }' = '1900-01-01', '{ dtStart }', '{ dtEnd }') " +
                         "ORDER BY[Journal Entry No]";
            return this.db.Query< GLTransactionsDst>(sql).ToList();
        }
    }
}