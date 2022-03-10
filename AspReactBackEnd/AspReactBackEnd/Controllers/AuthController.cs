using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JWTAuth.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly ContextDB _context;

        public LoginController(IConfiguration config, ContextDB context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Login _user)
        {
            if (_user.Email != null && _user.Password != null)
            {
                var user = await GetUser(_user.Email, _user.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("Name", user.Name),
                        new Claim("Email", user.Email)
                    };
                    var expires_time = DateTime.UtcNow.AddMinutes(10);
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: expires_time,
                        signingCredentials: signIn);

                    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                    Response.Cookies.Append("jwt", jwt, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = expires_time
                    });
                    return Ok(new
                    {
                        access_token = jwt,
                        token_type = "Bearer",
                        expires_time,
                        message = "Authenticated"
                    });
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        
        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new
            {
                message = "success"
            });
        }

        private async Task<User> GetUser(string email, string password)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if(verified)
            {
                return user;
            }

            return null;


        }

    }
    
    public class Login
    {
        [Required(ErrorMessage = "Email field is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        public string Password { get; set; }
    }
}