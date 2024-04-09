using Amazon.S3;
using Amazon.S3.Model;
using Core;
using Core.Interfaces.Repositories;
using Core.Models;
using Core.Models.Configurations;
using Dapper;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text;

namespace Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Func<IDbConnection> _connection;
        private readonly IUtilRepository _utilRepository;
        private readonly IAmazonS3 _client;
        private readonly AwsConfig _awsConfig;
        public UserRepository(Func<IDbConnection> connection,
            IUtilRepository utilRepository,
            IAmazonS3 client,
            IOptions<AwsConfig> awsConfig)
        {
            _connection = connection;
            _utilRepository = utilRepository;
            _client = client;
            _awsConfig = awsConfig.Value;
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

        public async Task UploadCnhImageAsync(Stream stream, string contentType, string path)
        {
            PutObjectRequest request = new ()
            {
                InputStream = stream,
                BucketName = _awsConfig.BucketName,
                Key = path,
                ContentType = contentType
            };

            await _client.PutObjectAsync(request).ConfigureAwait(false);
        }

        public async Task UpdateCNHImagePathAsync(Guid id, string cnhImagePath) =>
            await _utilRepository.UpdateFieldAsync(Constantes.TABLE_NAME_USERSYSTEM, nameof(cnhImagePath), cnhImagePath, nameof(id), id).ConfigureAwait(false);

        public async Task<bool> IsCNHUniqueAsync(string cnhNumber) =>
            await _utilRepository.IsFieldValueUniqueAsync(Constantes.TABLE_NAME_USERSYSTEM, nameof(cnhNumber), cnhNumber).ConfigureAwait(false);

        public async Task<bool> IsCPJUniqueAsync(string cnpj) =>
            await _utilRepository.IsFieldValueUniqueAsync(Constantes.TABLE_NAME_USERSYSTEM, nameof(cnpj), cnpj).ConfigureAwait(false);

        public async Task<string> GetLastCNHImagePathOfUserAsync(Guid idUser)
        {
            var query = "SELECT CNHIMAGEPATH FROM USERSYSTEM WHERE ID = @ID";

            DynamicParameters parameters = new();
            parameters.Add("@ID", idUser);

            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);

            return result;
        }

        public async Task<IEnumerable<User>> GetListUserNotifiedByIdDeliveryOrder(Guid IdDeliveryOrder)
        {
            var query = @"SELECT U.ID, NAME, CNPJ, BIRTHDAY, CNHNUMBER, CNHIMAGEPATH
                        FROM USERSYSTEM U
                        LEFT JOIN USERNOTIFYHISTORY UN
                        ON UN.IDUSER = U.ID
                        WHERE UN.IDDELIVERYORDER = @IDDELIVERYORDER";

            DynamicParameters parameters = new();
            parameters.Add("@IDDELIVERYORDER", IdDeliveryOrder);

            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryAsync<User>(query, parameters);

            return result;
        }
    }
}
