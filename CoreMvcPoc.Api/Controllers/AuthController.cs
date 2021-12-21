using CoreMvcPoc.BLL;
using CoreMvcPoc.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreMvcPoc.Api
{
    [Route("api/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly IUserService userService;     
        private IConfiguration _config;
        public AuthController(IUserService userService,IConfiguration config)
        {
            this.userService = userService;            
            _config = config;
        }

        // POST api/auth/login
        [HttpPost]
        [ActionName("login")]
        public IActionResult Login([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            IActionResult response = Unauthorized();

            var user = Authenticate(credentials);
            if(user==null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }
            if (user != null)
            {
                var tokenString = BuildToken(user);
                user.Token = tokenString;
                if(!SaveToken(user))
                {
                    return BadRequest(Errors.AddErrorToModelState("login_failure", "Error in Login", ModelState));
                }
                response = Ok(new { token = tokenString, role = user.Role });
            }

            return response;
        }

        private bool SaveToken(User user)
        {
            return userService.UpdateUser(user);
        }

        private string BuildToken(User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),               
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
               };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(20),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
           
        }

        private User Authenticate(CredentialsViewModel credentials)
        {
            return userService.Login(credentials.UserName, credentials.Password);
        }

        [HttpPost]
        [ActionName("logout")]
        [ServiceFilter(typeof(CustomAuth))]
        public IActionResult Logout()
        {
            IActionResult response = Unauthorized();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x=>x.Type=="Id");
            if(userIdClaim==null)
            {
                return BadRequest("Unable to logout");
            }
            var result = userService.Logout(Convert.ToInt32(userIdClaim.Value));
            if(result)
            {
                 response = Ok(new { message = "User logged out successfuly" });
            }
            return response;
        }

    }
} 