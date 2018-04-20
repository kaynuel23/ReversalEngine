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
        public async Task<IEnumerable<Reversals>> GetPendingReversals()
        {
            return await WithConnection(async c => {
                var results = await c.QueryAsync<Reversals>($"SELECT top 20 * FROM {_tableName} where REVERSALSTATUS = @REVERSALSTATUS", new { REVERSALSTATUS = ReversalStatus.Pending });
                return results;
            });
        }

        public async Task<int> Update(Reversals entity)
        {
            string updateQuery = $@"UPDATE {_tableName} SET REVERSALSTATUS = @REVERSALSTATUS WHERE ID=@ID";
            return await WithConnection(async c => {
                return await c.ExecuteAsync(updateQuery, new
                {
                    REVERSALSTATUS = entity.ReversalStatus,
                    ID = entity.ID
                });
            });
        }
        public async Task<int> Insert(string MFBCode,  string uniqueIdentifier)
        {            
            string insertQuery = @"INSERT INTO [Reversals] ([MFBCode],[UniqueIdentifier])
                                        OUTPUT inserted.ID 
                                        VALUES (@MFBCode,@UniqueIdentifier)";
            return await WithConnection(async c => {
                IEnumerable<int> result = await c.QueryAsync<int>(insertQuery, new
                {
                    MFBCode = MFBCode,
                    UniqueIdentifier = uniqueIdentifier
                });
                return result.SingleOrDefault();
            });
        }
    }
}
