using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartFeedbackAPI.Models;
using SmartFeedbackAPI.Services;

namespace SmartFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenService _jwtService;

        public AuthController(UserManager<ApplicationUser> userManager, JwtTokenService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (model.Role != "User" )
    return BadRequest("Invalid role. Allowed: User");

await _userManager.AddToRoleAsync(user, model.Role);

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.CreateToken(user, roles);
                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }
    }

    public class RegisterModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    // 👇 Add this line
    public string Role { get; set; } = "User"; // default to "User"
}


    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
