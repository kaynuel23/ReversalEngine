using BankOne.ReversalEngine.Model;
using Dapper;
using RepoWrapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankOne.ReversalEngine.Data
{
    public class DapperRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly string _tableName;
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public IDbConnection Connection
        {
            get
            {
                var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                var conn = factory.CreateConnection();
                conn.ConnectionString = connectionString;
                conn.OpenAsync();
                return conn;
            }
        }
        public DapperRepository(string tableName)
        {
            _tableName = tableName;
        }
        internal virtual dynamic Mapping(T item)
        {
            return item;
        }

        public virtual void Delete(T entity)
        {
            //using (IDbConnection conn = Connection)
            {
                //conn.Open();
                SqlMapper.Execute(Connection, "DELETE FROM " + _tableName + " WHERE ID=@ID", new { ID = entity.ID });
            }
        }

        public virtual void DeleteById(long id)
        {
            T entity = GetById(id);
            Delete(entity);
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IEnumerable<T> items = null;

            // extract the dynamic sql query and parameters from predicate
            if (filter != null)
            {
                QueryResult result = DynamicQuery.GetDynamicQuery(_tableName, filter);

                //using (IDbConnection cn = Connection)
                {
                    //cn.Open();
                    items = SqlMapper.Query<T>(Connection, result.Sql, (object)result.Param);
                }
            }
            else
            {
                //using (IDbConnection conn = Connection)
                {
                    //conn.Open();
                    items = SqlMapper.Query<T>(Connection, "SELECT * FROM " + _tableName).ToList();
                }
            }

            return items;
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {

            // extract the dynamic sql query and parameters from predicate
            if (filter != null)
            {
                QueryResult result = DynamicQuery.GetDynamicQuery(_tableName, filter);

                //using (IDbConnection conn = Connection)
                {
                    //conn.Open();
                    var results = await SqlMapper.QueryAsync<T>(Connection, result.Sql, (object)result.Param);
                    return results;
                }
            }
            else
            {
                //using (IDbConnection conn = Connection)
                {
                    //conn.Open();
                    var results = await SqlMapper.QueryAsync<T>(Connection, "SELECT * FROM " + _tableName);
                    return results;
                }
            }


        }

        public virtual T GetById(long id)
        {
            T item = default(T);
            //using (IDbConnection cn = Connection)
            {
                //cn.Open();
                item = SqlMapper.QueryFirstOrDefault<T>(Connection, "SELECT * FROM " + _tableName + " WHERE ID=@ID", new { ID = id });
            }

            return item;
        }

        public async virtual Task<T> GetByIdAsync(long id)
        {
            Task<T> item = default(Task<T>);
            //using (IDbConnection cn = Connection)
            {
                //cn.Open();
                item = SqlMapper.QueryFirstOrDefaultAsync<T>(Connection, "SELECT * FROM " + _tableName + " WHERE ID=@ID", new { ID = id });
            }

            return await item;
        }

        public virtual void Insert(T entity)
        {
            //using (IDbConnection conn = Connection)
            {
                var parameters = (object)Mapping(entity);
                string insertQuery = DynamicQuery.GetInsertQuery(_tableName, parameters);
                //conn.Open();
                entity.ID = SqlMapper.Query<int>(Connection, insertQuery).Single();
            }
        }
        private static string BuildInsertValues(T entity)
        {
            StringBuilder sb = new StringBuilder();
            var props = typeof(T).GetProperties().ToArray();
            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (property.Name.ToUpper() == "ID")
                    continue;
                //sb.AppendFormat($"@{property.Name}");
                sb.AppendFormat("@{0}", property.Name);
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public virtual void Update(T entity)
        {
            //using (IDbConnection conn = Connection)
            {
                var parameters = (object)Mapping(entity);

                //conn.Open();
                string updateQuery = DynamicQuery.GetUpdateQuery(_tableName, parameters);
                SqlMapper.Execute(Connection, updateQuery, entity);
            }
        }
    }
}
