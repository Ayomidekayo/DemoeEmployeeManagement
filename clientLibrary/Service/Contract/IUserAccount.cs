

using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace clientLibrary.Service.Contract
{
    public interface IUserAccount
    {

        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> LogInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RereshToken token);
        Task<WeatherForecast[]> WeatherForecastAsync();

    }
}
