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
        public async Task<int> ReversalExists(string mfbCode, string uniqueIdentifier)
        {
            return await WithConnection(async c => {
                var results = await c.QueryAsync<int>($@"SELECT count(*) FROM {_tableName} 
                                where MFBCODE = @MFBCODE and UNIQUEIDENTIFIER = @UNIQUEIDENTIFIER",
                                new { MFBCODE = mfbCode, UNIQUEIDENTIFIER = uniqueIdentifier });
                return results.FirstOrDefault();
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
        public async Task<int> BulkUpdate(List<Reversals> entities)
        {
            string updateQuery = $@"UPDATE {_tableName} SET REVERSALSTATUS = 1 WHERE ID=@ID";
            return await WithConnection(async c => {
                return await c.ExecuteAsync(updateQuery, entities);
            });
        }
        public async Task<int> Insert(string MFBCode,  string uniqueIdentifier)
        {            
            string insertQuery = @"INSERT INTO [Reversals] ([MFBCode],[UniqueIdentifier],[DateLogged],[ReversalStatus],[RetryCount])
                                        OUTPUT inserted.ID 
                                        VALUES (@MFBCode,@UniqueIdentifier,@DateLogged,@ReversalStatus,@RetryCount)";
            return await WithConnection(async c => {
                IEnumerable<int> result = await c.QueryAsync<int>(insertQuery, new
                {
                    MFBCode = MFBCode,
                    UniqueIdentifier = uniqueIdentifier,
                    DateLogged = DateTime.Now,
                    ReversalStatus = ReversalStatus.Pending,
                    RetryCount = 0
                });
                return result.SingleOrDefault();
            });
        }
    }
}
