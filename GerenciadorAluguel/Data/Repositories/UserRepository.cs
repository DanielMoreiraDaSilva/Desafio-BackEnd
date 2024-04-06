using Core.Interfaces.Repositories;
using Core.Models;
using Dapper;
using System.Data;
using System.Text;

namespace Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Func<IDbConnection> _connection;
        public UserRepository(Func<IDbConnection> connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<UserTypeCNH>> GetAllCNHTypeAsync(bool? valid = null)
        {
            var query = new StringBuilder("SELECT ID, TYPE, DESCRIPTION, VALID FROM CNHTYPE");

            if (valid.HasValue)
                query.Append(" WHERE VALID = ").Append(valid.Value ? "1" : "0");


            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryAsync<UserTypeCNH>(query.ToString());

            return result;
        }

        public async Task AddAsync(User user)
        {
            var query = "INSERT INTO USERSYSTEM (ID, NAME, CNPJ, BIRTHDAY, CNHNUMBER) VALUES (@ID, @NAME, @CNPJ, @BIRTHDAY, @CNHNUMBER);";

            DynamicParameters parameters = new();
            parameters.Add("@ID", user.Id);
            parameters.Add("@NAME", user.Name);
            parameters.Add("@CNPJ", user.CNPJ);
            parameters.Add("@BIRTHDAY", user.BirthDay);
            parameters.Add("@CNHNUMBER", user.CNHNumber);

            using IDbConnection connection = _connection.Invoke();

            await connection.ExecuteAsync(query, parameters);
        }

    }
}
