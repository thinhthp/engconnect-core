using EngConnect.Entities.Entities;
using EngConnect.Services.DTOs.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EngConnect.Api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Services.DTOs.Account.RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Create user
            ApplicationUser user = new ApplicationUser
            {
                UserName = request.Name,
                Email = request.Email,
                PhoneNumber = request.Phone,
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password!);

            if (result.Succeeded)
            {
                //Add Role
                await _userManager.AddToRoleAsync(user, "Student");

                //Mapping to DTO
                UserResponse response = new UserResponse
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    IsActive = user.IsActive,
                    CreateDate = user.CreatedAt,
                    UpdateDate = user.UpdateDate,
                    CreateBy = user.CreateBy,
                    UpdateBy = user.UpdateBy
                };
                return StatusCode(StatusCodes.Status201Created, new { response, message = "User registered successfully!" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check user
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email!);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password!))
                return Unauthorized();

            //Get role
            //IList<string> roles = await _userManager.GetRolesAsync(user);
            //var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

            //JWT
            string tokenString = await GenerateToken(user);

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, request.Password!, false, false);

            if (result.Succeeded)
            {
                //Mapping to DTO
                UserResponse response = new UserResponse
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    IsActive = user.IsActive,
                    CreateDate = user.CreatedAt,
                    UpdateDate = user.UpdateDate,
                    CreateBy = user.CreateBy,
                    UpdateBy = user.UpdateBy
                };

                return Ok(new { response, tokenString });
            }

            return BadRequest(new { message = "Invalid email or password" });
        }

        [HttpGet("google-signin")]
        public IActionResult GoogleSignIn()
        {
            string? redirectUrl = Url.Action("GoogleResponse", "Auth");
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        //Callback endpoint that processes the external login info from Google.
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            //Retrieve external login info from the temporary external cookie.
            ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("Error loading external login information.");
            }

            //Retrieve the user's email from the external login claims.
            string? email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email claim not received from Google.");
            }

            //Check if the user already exists; if not, create a new user.
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };
                IdentityResult createResult = await _userManager.CreateAsync(user);

                //Add role
                await _userManager.AddToRoleAsync(user, "Student");
                if (!createResult.Succeeded)
                {
                    return BadRequest("Error creating user.");
                }
            }

            //Link the user to the external login provider.
            IdentityResult loginResult = await _userManager.AddLoginAsync(user, info);

            //Generate a JWT token for the authenticated user.
            string tokenString = await GenerateToken(user);

            // Mapping to DTO (UserResponse)
            UserResponse response = new UserResponse
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                IsActive = user.IsActive,
                CreateDate = user.CreatedAt,
                UpdateDate = user.UpdateDate,
                CreateBy = user.CreateBy,
                UpdateBy = user.UpdateBy
            };

            // Return HTML response with both token and user data for frontend compatibility
            string htmlResponse = $@"
                <html>
                <body>
                    <script>
                    window.opener.postMessage({{
                        token: '{tokenString}',
                        response: {System.Text.Json.JsonSerializer.Serialize(response)}
                    }}, 'https://swd-392-se-1709-group1-fe.vercel.app/');
                    </script>
                </body>
                </html>";

            return Content(htmlResponse, "text/html");
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            var nowUtc = DateTime.UtcNow;
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            byte[] key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName!)
                }.Concat(roleClaims)),
                IssuedAt = nowUtc,
                NotBefore = nowUtc,
                Expires = nowUtc.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
