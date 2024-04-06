using Core.Interfaces.Repositories;
using Core.Models;
using Dapper;
using System.Data;
using System.Text;

namespace Data.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly Func<IDbConnection> _connection;
        public MotorcycleRepository(Func<IDbConnection> connection)
        {
            _connection = connection;
        }

        public async Task AddAsync(Motorcycle motorcycle)
        {
            var query = "INSERT INTO MOTORCYCLE (ID, YEAR, MODEL, PLATE) VALUES (@ID, @YEAR, @MODEL, @PLATE);";

            DynamicParameters parameters = new ();
            parameters.Add("@ID", motorcycle.Id);
            parameters.Add("@YEAR", motorcycle.Year);
            parameters.Add("@MODEL", motorcycle.Model);
            parameters.Add("@PLATE", motorcycle.Plate);
            

            using IDbConnection connection = _connection.Invoke();

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task DeleteAsync(Guid idMotorcycle)
        {
            var query = "DELETE FROM MOTORCYCLE WHERE ID=@ID;";

            DynamicParameters parameters = new();
            parameters.Add("@ID", idMotorcycle);

            using IDbConnection connection = _connection.Invoke();

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task<IEnumerable<Motorcycle>> GetAsync(MotorcycleFilter? filter = null)
        {
            var query = new StringBuilder(@"SELECT ID, YEAR, MODEL, PLATE FROM MOTORCYCLE");

            DynamicParameters parameters = new();

            if(!string.IsNullOrEmpty(filter?.Plate))
            {
                parameters.Add("@PLATE", string.Join("", "%",filter?.Plate,"%"));

                query.Append(" WHERE PLATE LIKE @PLATE");
            }

            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryAsync<Motorcycle>(query.ToString(), parameters);

            return result;
        }

        public async Task UpdatePlateAsync(Guid idMotorcycle, string plate)
        {
            var query = "UPDATE MOTORCYCLE SET PLATE=@PLATE WHERE ID=@ID;";

            DynamicParameters parameters = new();
            parameters.Add("@ID", idMotorcycle);
            parameters.Add("@PLATE", plate);

            using IDbConnection connection = _connection.Invoke();

            await connection.ExecuteAsync(query, parameters);
        }
    }
}
