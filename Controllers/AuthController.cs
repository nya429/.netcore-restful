
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    // api/auth
    [Route("api/[controller]")]
    [ApiController]
    // ControllerBase provide model and controller
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserToRegisterDto userToResgisterDto)
        {
            // ApiController can handle System.NullReferenceException
            // if(!ModelState.IsValid)
            //     return BadRequest(ModelState);

            userToResgisterDto.Username = userToResgisterDto.Username.ToLower();

            if (await _repo.UserExists(userToResgisterDto.Username))
                return BadRequest("username already exists");
            // Object Initilizer syntax
            var userToCreate = new User
            {
                Username = userToResgisterDto.Username
            };

            var userCreateed = await _repo.Register(userToCreate, userToResgisterDto.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // 1 make a DB call. check if login UN/PW matches the information stored in the DB
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(),
                userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            // 2. Claim-based token information. This case includes => UserId, UserName  
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username),
            };

            // 3. Create a secuirty key with secret
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            // 4. Create a secuirty Credential with key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 5 payload part
            var toeknDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(toeknDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}