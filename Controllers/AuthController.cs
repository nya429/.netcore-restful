
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    // api/auth
    [Route("api/[controller]")]
    [ApiController]
    // ControllerBase provide model and controller
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserToRegisterDto userToResgisterDto)
        {   
            userToResgisterDto.Username = userToResgisterDto.Username.ToLower();
        
            if(await _repo.UserExists(userToResgisterDto.Username)) 
                return BadRequest("username already exists");
            // Object Initilizer syntax
            var userToCreate = new User
            {
                Username = userToResgisterDto.Username
            };

            userToCreate = await _repo.Register(userToCreate, userToResgisterDto.Password);
            return StatusCode(201);
        }
    }
}