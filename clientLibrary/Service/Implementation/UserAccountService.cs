using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using clientLibrary.Helpers;
using clientLibrary.Service.Contract;
using System.Net.Http.Json;

namespace clientLibrary.Service.Implementation
{
    public class UserAccountService(GetHttpClient getHttpClient) : IUserAccount
    {
        public const string Authurl = "api/authentication";
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var httpClient=getHttpClient.GetPublicHttpclient();
            var result = await httpClient.PostAsJsonAsync($"{Authurl}/register", user);
            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");
            return await result.Content.ReadFromJsonAsync<GeneralResponse>()!;
        }

        public async Task<LoginResponse> LogInAsync(Login user)
        {
            var httpClient = getHttpClient.GetPublicHttpclient();
            var result = await httpClient.PostAsJsonAsync($"{Authurl}/login", user);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");
            return await result.Content.ReadFromJsonAsync<LoginResponse>()!;
        }

        public Task<LoginResponse> RefreshTokenAsync(RereshToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<WeatherForecast[]> WeatherForecastAsync()
        {
           var httpClient=await getHttpClient.GetPrivateHttpclient();
            var result = await httpClient.GetFromJsonAsync<WeatherForecast[]>("api/weatherforecast");
            return result!;
        }
    }
}
