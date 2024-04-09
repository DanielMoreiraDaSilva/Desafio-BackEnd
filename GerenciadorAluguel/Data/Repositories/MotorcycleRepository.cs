using Core;
using Core.Interfaces.Repositories;
using Core.Models;
using Core.Models.Filters;
using Dapper;
using System.Data;
using System.Text;

namespace Data.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly Func<IDbConnection> _connection;
        private readonly IUtilRepository _utilRepository;
        public MotorcycleRepository(Func<IDbConnection> connection, 
            IUtilRepository utilRepository)
        {
            _connection = connection;
            _utilRepository = utilRepository;
        }

        public async Task AddMotorcycleAsync(Motorcycle motorcycle)
        {
            var query = "INSERT INTO MOTORCYCLE (ID, YEAR, MODEL, PLATE) VALUES (@ID, @YEAR, @MODEL, @PLATE);";

            DynamicParameters parameters = new();
            parameters.Add("@ID", motorcycle.Id.Value);
            parameters.Add("@YEAR", motorcycle.Year.Value);
            parameters.Add("@MODEL", motorcycle.Model);
            parameters.Add("@PLATE", motorcycle.Plate.ToUpper());
            

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

        public async Task<IEnumerable<Motorcycle>> GetListMotorcycleAsync(MotorcycleFilter filter)
        {
            var query = new StringBuilder(@"SELECT M.ID, YEAR, MODEL, PLATE FROM MOTORCYCLE M");

            DynamicParameters parameters = new();

            var where = " WHERE ";
            var and = " AND ";
            var whereInsert = false;

            if(filter.Disponible.HasValue)
            {
                query.Append(" LEFT JOIN MOTORCYCLERENTAL MR ON M.ID = MR.IDMOTORCYCLE AND NOW() BETWEEN MR.RENTALSTARTDATE AND MR.RENTALENDDATE");

                if (whereInsert == false) { query.Append(where); whereInsert = true; }
                else query.Append(and);

                query.AppendFormat("MR.ID IS {0} NULL", filter.Disponible.Value ? "" : "NOT");
            }

            if (!string.IsNullOrEmpty(filter.Plate))
            {
                if (whereInsert == false) { query.Append(where); whereInsert = true; }
                else query.Append(and);

                parameters.Add("@PLATE", string.Join("", "%",filter?.Plate,"%"));

                query.Append("PLATE LIKE @PLATE");
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
            parameters.Add("@PLATE", plate.ToUpper());

            using IDbConnection connection = _connection.Invoke();

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task<IEnumerable<RentalPlan>> GetListRentalPlansAsync(Guid? id = null)
        {
            var query = new StringBuilder("SELECT ID, NAME, DURATIONINDAYS, DAILYCOST, FACTORCALCULATECOST, COSTEXTRADAYS FROM RENTALPLAN");

            DynamicParameters parameters = new();

            if(id.HasValue)
            {
                query.Append(" WHERE ID = @ID");

                parameters.Add("@ID", id.Value);
            }

            using IDbConnection connection = _connection.Invoke();

            var result = await connection.QueryAsync<RentalPlan>(query.ToString(), parameters);

            return result;
        }

        public async Task AddMotorcycleRentalAsync(MotorcycleRentalPlan rental)
        {
            var query = @"INSERT INTO MOTORCYCLERENTAL (ID, IDUSER, IDMOTORCYCLE, IDRENTALPLAN, RENTALSTARTDATE, RENTALENDDATE, EXPECTEDRETURNDATE) 
                  VALUES (@ID, @USERID, @MOTORCYCLEID, @RENTALPLANID, @RENTALSTARTDATE, @RENTALENDDATE, @EXPECTEDRETURNDATE);";

            DynamicParameters parameters = new();
            parameters.Add("@ID", rental.Id);
            parameters.Add("@UserId", rental.IdUser);
            parameters.Add("@MotorcycleId", rental.IdMotorcycle);
            parameters.Add("@RentalPlanId", rental.IdRentalPlan);
            parameters.Add("@RentalStartDate", rental.RentalStartDate);
            parameters.Add("@RentalEndDate", rental.RentalEndDate);
            parameters.Add("@ExpectedReturnDate", rental.ExpectedReturnDate.Value);

            using IDbConnection connection = _connection.Invoke();

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task<bool> ThereIsRentalForMotorcycleAsync(Guid idMotorcycle) =>
            await _utilRepository.IsFieldValueUniqueAsync(Constantes.TABLE_NAME_MOTORCYCLERENTAL, nameof(idMotorcycle), idMotorcycle);

        public async Task<bool> IsPlateUniqueAsync(string plate) =>
            await _utilRepository.IsFieldValueUniqueAsync(Constantes.TABLE_NAME_MOTORCYCLE, nameof(plate), plate);
    }
}
