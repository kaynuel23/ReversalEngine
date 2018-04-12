using BankOne.ReversalEngine.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankOne.ReversalEngine.Data
{
    public class ReversalsRepository : DapperRepository<Reversals>
    {
        private string _tableName;
        public ReversalsRepository(string tableName) : base(tableName)
        {
            _tableName = tableName;
        }
        public IEnumerable<Reversals> GetPendingReversals()
        {
            using (IDbConnection conn = Connection)
            {
                return SqlMapper.Query<Reversals>(conn, $"SELECT * FROM {_tableName} where REVERSALSTATUS = @REVERSALSTATUS", new { REVERSALSTATUS = ReversalStatus.Pending }); 
            }
        }

        public override void Update(Reversals entity)
        {
            using (IDbConnection conn = Connection)
            {
                string updateQuery = $@"UPDATE {_tableName} SET REVERSALSTATUS = @REVERSALSTATUS WHERE ID=@ID";
                //conn.Open();
                SqlMapper.Execute(conn, updateQuery, new
                {
                    REVERSALSTATUS = entity.ReversalStatus,
                    ID = entity.ID
                });
            }
        }
        public new long Insert(string MFBCode,  string uniqueIdentifier)
        {
            using (IDbConnection conn = Connection)
            {
                string insertQuery = @"INSERT INTO [Reversals] ([MFBCode],[UniqueIdentifier])
                                        OUTPUT inserted.ID 
                                        VALUES (@MFBCode,@UniqueIdentifier)";
                //conn.Open();
                return SqlMapper.Query<int>(conn, insertQuery, new
                {
                    MFBCode = MFBCode,
                    UniqueIdentifier = uniqueIdentifier
                }).Single();
            }
        }
    }
}
