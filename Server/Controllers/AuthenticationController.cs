using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using ServerLibrary.Reository.Contract;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUserAccount AccountInterface) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if (user == null) return BadRequest("Model is empty");
            var rsult= await AccountInterface.CreateAsync(user);
            return Ok(rsult);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if (user == null) return BadRequest("User is empty");
            var result=await AccountInterface.SigingInAsync(user);
            return Ok(result);
        }
        [HttpPost("Refrsh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RereshToken token)
        {
            if (token == null) return BadRequest("Model is empty");
            var result=await AccountInterface.RefreshTokenAsync(token);
            return Ok(result);
        }
    }
}
