using Core.Interfaces.Repositories;
using Core.Models;
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
            var query = new StringBuilder()
                .AppendFormat("SELECT COUNT(*) FROM {0} WHERE {1} = @{1}", table, field);

            DynamicParameters parameters = new();
            parameters.Add(string.Concat("@", field), value);

            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryFirstOrDefaultAsync<int>(query.ToString(), parameters);

            return result == 0;
        }

        public async Task UpdateFieldAsync<T>(string table, string fieldToUpdate, T valueToUpdate, string fieldToMatch, object valueToMatch, ControlConnection controlConnection = null)
        {
            var query = new StringBuilder()
                .AppendFormat("UPDATE {0} SET {1}=@{1} WHERE {2}=@{2};", table, fieldToUpdate, fieldToMatch);

            DynamicParameters parameters = new();
            parameters.Add(string.Concat("@", fieldToUpdate), valueToUpdate);
            parameters.Add(string.Concat("@", fieldToMatch), valueToMatch);

            using IDbConnection connection = _connection.Invoke();

            if(controlConnection != null)
                await controlConnection.Connection.ExecuteAsync(query.ToString(), parameters, controlConnection.Transaction);
            else
                await connection.ExecuteAsync(query.ToString(), parameters);
        }

    }
}
