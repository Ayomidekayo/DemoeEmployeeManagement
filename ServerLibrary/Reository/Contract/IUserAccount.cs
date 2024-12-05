using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace ServerLibrary.Reository.Contract
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SigingInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RereshToken token);
    }
}
