using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepositoy)
        {
            _userRepository = userRepositoy;
        }

        public async Task AddAsync(User user) =>
            await _userRepository.AddAsync(user).ConfigureAwait(false);

        public async Task<IEnumerable<TypeCNH>> GetAllCNHTypeAsync(bool? valid = null) =>
            await _userRepository.GetAllCNHTypeAsync(valid).ConfigureAwait(false);

        public async Task<ValidateReturn> ValidateNewUserAsync(User user)
        {
            ValidateReturn result = new();

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

            if (!await _userRepository.IsCNHUniqueAsync(user.CNHNumber).ConfigureAwait(false))
                result.Errors.Add("CNH já cadastrada para outro usuario");

            if (!await _userRepository.IsCPJUniqueAsync(user.CNPJ).ConfigureAwait(false))
                result.Errors.Add("CNPJ já cadastrado para outro usuario");

            return result;
        }

        public async Task UpdaloadCnhImageAsync(Guid idUser, Stream stream, string contentType)
        {
            var fileName = await _userRepository.GetLastCNHImagePathOfUserAsync(idUser).ConfigureAwait(false);

            if(string.IsNullOrEmpty(fileName))
            {
                fileName = string.Concat(Guid.NewGuid().ToString(), ".", contentType.Split("/")[1]);

                await _userRepository.UpdaloadCnhImageAsync(stream, contentType, fileName).ConfigureAwait(false);

                await _userRepository.UpdateCNHImagePathAsync(idUser, fileName).ConfigureAwait(false);
            }
            else
                await _userRepository.UpdaloadCnhImageAsync(stream, contentType, fileName).ConfigureAwait(false);
        }
    }
}
