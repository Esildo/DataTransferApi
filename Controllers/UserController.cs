using DataTransferApi.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DataTransferApi.Model;
using DataTransferApi.Services;
using DataTransferApi.Dtos;

namespace DataTransferApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public UserController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response  = await _authenticationService.Login(request); 

            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authenticationService.Register(request);

            return Ok(response);
        }

        //[HttpGet("login/{username}")]
        //public IActionResult Get(string username)
        //{
        //    var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };

        //    var jwt = new JwtSecurityToken(
        //        issuer: AuthOptions.ISSUER,
        //        audience: AuthOptions.AUDIENCE,
        //        claims: claims,
        //        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
        //        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        //    var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        //    return Ok(token);

        //}
        //[HttpGet("login")]
        //public IActionResult Login([FromBody] Login request)
        //{

        //}


        [Authorize]
        [HttpGet("data")]
        public IActionResult GetData() 
        { 
            return Ok(new {message = "hello from authorize sicret path"});
        }


        //private Account AuthenticateUser(string email, string password)
        //{
        //    return Accounts.SingleOrDefault(u => u.Email == email && u.Password == password);
        //}
    }

    
}
