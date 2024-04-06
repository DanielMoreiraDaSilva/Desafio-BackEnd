using Core.Interfaces.Repositories;
using Dapper;
using System.Data;
using System.Text;

namespace Data.Repositories
{
    public class UtilRepository : IUtilRepository
    {
        private readonly Func<IDbConnection> _connection;
        public UtilRepository(Func<IDbConnection> connection)
        {
            _connection = connection;
        }

        public async Task<bool> IsFieldValueUniqueAsync<T>(string table, string field, T value)
        {
            var query = new StringBuilder().AppendFormat("SELECT COUNT(*) FROM {0} WHERE {1} = @{1}", table, field);

            DynamicParameters parameters = new();
            parameters.Add(string.Concat("@", field), value);

            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryFirstOrDefaultAsync<int>(query.ToString(), parameters);

            return result == 0;
        }
    }
}
