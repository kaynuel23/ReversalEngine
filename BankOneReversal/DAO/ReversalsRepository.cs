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
        public Task<IEnumerable<Reversals>> GetPendingReversals()
        {
            using (IDbConnection conn = Connection)
            {
                return SqlMapper.QueryAsync<Reversals>(conn, $"SELECT * FROM {_tableName} where REVERSALSTATUS = @REVERSALSTATUS", new { REVERSALSTATUS = ReversalStatus.Pending }); 
            }
        }

        public async new Task<int> Update(Reversals entity)
        {
            using (IDbConnection conn = Connection)
            {
                string updateQuery = $@"UPDATE {_tableName} SET REVERSALSTATUS = @REVERSALSTATUS WHERE ID=@ID";
                //conn.Open();
                return await SqlMapper.ExecuteAsync(conn, updateQuery, new
                {
                    REVERSALSTATUS = entity.ReversalStatus,
                    ID = entity.ID
                });
            }
        }
        public async Task<int> Insert(string MFBCode,  string uniqueIdentifier)
        {
            using (IDbConnection conn = Connection)
            {
                string insertQuery = @"INSERT INTO [Reversals] ([MFBCode],[UniqueIdentifier])
                                        OUTPUT inserted.ID 
                                        VALUES (@MFBCode,@UniqueIdentifier)";
                //conn.Open();
                IEnumerable<int> result =  await SqlMapper.QueryAsync<int>(conn, insertQuery, new
                {
                    MFBCode = MFBCode,
                    UniqueIdentifier = uniqueIdentifier
                });
                return result.SingleOrDefault();
            }
        }
    }
}
