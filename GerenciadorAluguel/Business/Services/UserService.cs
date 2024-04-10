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
                result.Errors.Add("You must have the CNH types filled");
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
                    result.Errors.Add("You must have at least CNH type A or B");
            }

            if (!await _userRepository.IsCNHUniqueAsync(user.CNHNumber).ConfigureAwait(false))
                result.Errors.Add("CNH already registered for another user");

            if (!await _userRepository.IsCPJUniqueAsync(user.CNPJ).ConfigureAwait(false))
                result.Errors.Add("CNPJ already registered for another user");

            return result;
        }

        public async Task UploadCnhImageAsync(Guid idUser, Stream stream, string contentType)
        {
            var fileName = await _userRepository.GetLastCNHImagePathOfUserAsync(idUser).ConfigureAwait(false);

            if(string.IsNullOrEmpty(fileName))
            {
                fileName = string.Concat(Guid.NewGuid().ToString(), ".", contentType.Split("/")[1]);

                await _userRepository.UploadCnhImageAsync(stream, contentType, fileName).ConfigureAwait(false);

                await _userRepository.UpdateCNHImagePathAsync(idUser, fileName).ConfigureAwait(false);
            }
            else
                await _userRepository.UploadCnhImageAsync(stream, contentType, fileName).ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetListUserNotifiedByIdDeliveryOrder(Guid IdDeliveryOrder) =>
            await _userRepository.GetListUserNotifiedByIdDeliveryOrder(IdDeliveryOrder).ConfigureAwait(false);
    }
}
