using Amazon.S3;
using Amazon.S3.Model;
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
        private readonly IUtilRepository _utilRepository;
        private readonly IAmazonS3 _client;
        public UserRepository(Func<IDbConnection> connection,
            IUtilRepository utilRepository,
            IAmazonS3 client)
        {
            _connection = connection;
            _utilRepository = utilRepository;
            _client = client;
        }

        public async Task<IEnumerable<TypeCNH>> GetAllCNHTypeAsync(bool? valid = null)
        {
            var query = new StringBuilder("SELECT ID, TYPE, DESCRIPTION, VALID FROM CNHTYPE");

            if (valid.HasValue)
                query.Append(" WHERE VALID = ").Append(valid.Value ? "1" : "0");


            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryAsync<TypeCNH>(query.ToString());

            return result;
        }

        public async Task<IEnumerable<UserTypeCNH>> GetAllCNHTypeOfUserIdAsync(Guid idUser)
        {
            var query = "SELECT ID, IDUSERSYSTEM, IDCNHTYPE FROM USERSYSTEMCNHTYPE WHERE IDUSERSYSTEM = @IDUSER";

            using IDbConnection connection = _connection.Invoke();

            return await connection.QueryAsync<UserTypeCNH>(query, new { idUser = idUser });
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

        public async Task UpdaloadCnhImageAsync(Stream stream, string contentType, string path)
        {
            PutObjectRequest request = new ()
            {
                InputStream = stream,
                BucketName = "storage-cnh-images",
                Key = path,
                ContentType = contentType
            };

            await _client.PutObjectAsync(request);
        }

        public async Task UpdateCNHImagePathAsync(Guid idUser, string imagePath) =>
            await _utilRepository.UpdateFieldAsync("USERSYSTEM", "CNHIMAGEPATH", imagePath, "ID", idUser);

        public async Task<bool> IsCNHUniqueAsync(string cnhNumber) =>
            await _utilRepository.IsFieldValueUniqueAsync("USERSYSTEM", nameof(cnhNumber), cnhNumber);

        public async Task<bool> IsCPJUniqueAsync(string cnpj) =>
            await _utilRepository.IsFieldValueUniqueAsync("USERSYSTEM", nameof(cnpj), cnpj);

        public async Task<string> GetLastCNHImagePathOfUserAsync(Guid idUser)
        {
            var query = "SELECT CNHIMAGEPATH FROM USERSYSTEM WHERE ID = @ID";

            DynamicParameters parameters = new();
            parameters.Add("@ID", idUser);

            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);

            return result;
        }
    }
}
