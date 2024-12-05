
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Reository.Contract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Constants = ServerLibrary.Helper.Constants;

namespace ServerLibrary.Reository.Implementation
{
    public class UserAccountReository(IOptions<JwtSection> config, AppDbContext appDbContext) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user is null) return new GeneralResponse(false, "Model is empty");
            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser != null) return new GeneralResponse(false, "User registered already");
            //Save user
            var applicationUser = await AddToDatabase(new ApplicationUser()
            {
                FullName = user.Email,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });
            // check Create asign role
            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(u => u.Name!.Equals(Constants.Admin));
            if (checkAdminRole is null)
            {
                var CreateAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
                await AddToDatabase(new UserRole() { RoleId = CreateAdminRole.Id, UserId = applicationUser.Id });
                return new GeneralResponse(true, "Account created!");

            }
            //Add user Role
            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(u => u.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
                response = await AddToDatabase(new SystemRole() { Name = Constants.User });
                await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
            }
            return new GeneralResponse(true, "Account created");

        }
        private async Task<UserRole> FindUserRole(int userId) => await appDbContext.UserRoles.FirstOrDefaultAsync(u => u.UserId == userId);
        private async Task<SystemRole> FindRoleName(int roleId) => await appDbContext.SystemRoles.FirstOrDefaultAsync(u => u.Id == roleId);
    
    public async Task<LoginResponse> SigingInAsync(Login user)
        {
            if (user is null) return new LoginResponse(false, "Model is empty");
            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null) return new LoginResponse(false, "User not found");
            //Verify password
            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
                return new LoginResponse(false, "Email/Password not valid");
            //Vetify UserrRole
            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null) return new LoginResponse(false, "User role not found");

            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName is null) return new LoginResponse(false, "User role not found");

            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name!);
            string rereshToken = GenerateRefreshToken();
            //save rfreshtoken to the dataase
            var findUser = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(u => u.UserId == applicationUser.Id);
            if (findUser is not null)
            {
                findUser!.Token = rereshToken;
                await appDbContext.SaveChangesAsync();
            }else
            {
                await AddToDatabase(new RefreshTokenInfo { Token = rereshToken ,UserId=applicationUser.Id});
            }
            return new LoginResponse(true, "LOgin successfully", jwtToken, rereshToken);
        }
        private string GenerateToken(ApplicationUser user, String role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.FullName!),
                new Claim(ClaimTypes.Email,user.Email!),
                new Claim(ClaimTypes.Role,role!)
            };
            var token = new JwtSecurityToken(
                issuer: config.Value.Issuer,
                audience: config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        private async Task<ApplicationUser> FindUserByEmail(string email) =>
            await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email!.ToLower()!.Equals(email!.ToLower()));


        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = appDbContext.Add(model!);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }

        public async Task<LoginResponse> RefreshTokenAsync(RereshToken token)
        {
            if (token is null) return new LoginResponse(false, "Model is empty");
            var findToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(u => u.Token!.Equals(token.Token));
            if (findToken is null) return new LoginResponse(false, "Refresh token is required");
            //Get user credentials
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == findToken.UserId);
            if (user is null) return new LoginResponse(false, "Refresh token could not be generated because user ot found");
             var userRole=await FindUserRole(user.Id);
            var roleName=await FindRoleName(userRole.RoleId);
            string jwtToken =  GenerateToken(user, roleName.Name!);
            string refreshToken = GenerateRefreshToken();
            var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(u => u.UserId==user.Id);
            if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token cloud not be generated because user is not signed in");
            updateRefreshToken.Token = refreshToken;
            await appDbContext.SaveChangesAsync();
            return new LoginResponse(true,"Tokem refreshed successfully",jwtToken,refreshToken);
        }
    }
}
