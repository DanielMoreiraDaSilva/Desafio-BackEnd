using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUtilRepository _utilRepository;
        public UserService(IUserRepository userRepositoy, 
            IUtilRepository utilRepository)
        {
            _userRepository = userRepositoy;
            _utilRepository = utilRepository;

        }

        public async Task AddAsync(User user) =>
            await _userRepository.AddAsync(user).ConfigureAwait(false);

        public async Task<IEnumerable<UserTypeCNH>> GetAllCNHTypeAsync(bool? valid = null) =>
            await _userRepository.GetAllCNHTypeAsync(valid).ConfigureAwait(false);

        public async Task<UserValidateReturn> ValidateNewUserAsync(User user)
        {
            UserValidateReturn result = new();

            var listValidCNHType = (await _userRepository.GetAllCNHTypeAsync(true).ConfigureAwait(false)).Select(x => x.Type);
            var cnhIsValid = false;

            if(!user.InsertCNHType.Any())
                result.Errors.Add("Você deve possuir os tipors de CNH preenchidos");
            else
            {
                foreach (var type in user.InsertCNHType)
                {
                    if (listValidCNHType.Where(x => x == type).Any())
                    {
                        cnhIsValid = true;
                        break;
                    }
                }

                if(!cnhIsValid)
                    result.Errors.Add("Você deve possuir ao menos CNH do tipo A ou B");
            }

            if (!await _utilRepository.IsFieldValueUniqueAsync("USERSYSTEM", nameof(user.CNHNumber), user.CNHNumber))
                result.Errors.Add("CNH já cadastrada para outro usuario");

            if (!await _utilRepository.IsFieldValueUniqueAsync("USERSYSTEM", nameof(user.CNPJ), user.CNPJ))
                result.Errors.Add("CNPJ já cadastrado para outro usuario");

            return result;
        }
    }
}
