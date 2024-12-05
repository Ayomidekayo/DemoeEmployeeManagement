
using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace clientLibrary.Helpers
{
    public class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var stringToken = await localStorageService.GetToken();

            if (String.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(anonymous));

            var deserializeToken = Serilizations.DeserilizeJsonString<UserSession>(stringToken);
            if (deserializeToken == null) return await Task.FromResult(new AuthenticationState(anonymous));

            var getUserClaims = DecrptToken(deserializeToken.Token!);
            if (getUserClaims == null) return await Task.FromResult(new AuthenticationState(anonymous));

            var claimPrincipal = SetClaimPrincipal(getUserClaims);
            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        }
        public static ClaimsPrincipal SetClaimPrincipal(CusomerUserClaim claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                { new(ClaimTypes.NameIdentifier,claims.id!),
                    new (ClaimTypes.Name,claims.Name!),
                    new (ClaimTypes.Email,claims.Email!),
                    new (ClaimTypes.Role,claims.Role!),
                }, "JwtAuth"));
        }
        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            var claimPrincipal = new ClaimsPrincipal();
            if (userSession.Token != null || userSession.RefreshToken != null)
            { var serializeSession = Serilizations.SerializeObj(userSession);
                await localStorageService.SetToken(serializeSession);
                 var getUserClaim=DecrptToken(userSession.Token!);
                claimPrincipal = SetClaimPrincipal(getUserClaim);
            }else
            {
                await localStorageService.RemoveToken();
            }    
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimPrincipal))); 
        }
        private CusomerUserClaim DecrptToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken)) return new CusomerUserClaim();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var userId = token.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            var name = token.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name);
            var email = token.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email);
            var role = token.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Role);
            return new CusomerUserClaim(userId!.Value!, name!.Value!, email!.Value!, role!.Value!);
        }
    }
}