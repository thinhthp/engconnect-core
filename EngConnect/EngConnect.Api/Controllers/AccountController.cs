using EngConnect.Entities.Entities;
using EngConnect.Services.DTOs.Account;
using EngConnect.Services.Services.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("register/student")]
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

                // Generate and send confirmation email
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string encodedToken = WebUtility.UrlEncode(token);
                string? callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "Account",
                    new { userId = user.Id, token = encodedToken },
                    protocol: Request.Scheme);

                string html = $@"
                    <p>Hi {WebUtility.HtmlEncode(user.UserName)},</p>
                    <p>Thanks for registering at EngConnect. Please confirm your email by clicking the link below:</p>
                    <p><a href=""{callbackUrl}"">Confirm your email</a></p>
                    <p>If you did not create this account, you can ignore this email.</p>";

                await _emailService.SendEmailAsync(user.Email!, "Confirm your EngConnect account", html);

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

        [HttpPost("register/tutor")]
        public async Task<IActionResult> RegisterTutor([FromBody] Services.DTOs.Account.RegisterRequest request)
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
                await _userManager.AddToRoleAsync(user, "Tutor");

                // Generate and send confirmation email
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string encodedToken = WebUtility.UrlEncode(token);
                string? callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "Account",
                    new { userId = user.Id, token = encodedToken },
                    protocol: Request.Scheme);

                string html = $@"
                    <p>Hi {WebUtility.HtmlEncode(user.UserName)},</p>
                    <p>Thanks for registering at EngConnect. Please confirm your email by clicking the link below:</p>
                    <p><a href=""{callbackUrl}"">Confirm your email</a></p>
                    <p>If you did not create this account, you can ignore this email.</p>";

                await _emailService.SendEmailAsync(user.Email!, "Confirm your EngConnect account", html);

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

            //Check email confirmed
            //if (!user.EmailConfirmed)
            //{
            //    return BadRequest(new { message = "Email not confirmed. Please check your inbox." });
            //}
            if(user.IsActive == false)
            {
                return BadRequest(new { message = "Your account has been deactivated. Please contact support for assistance." });
            }

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

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid confirmation link.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            string decodedToken = WebUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                // redirect to FE
                string? redirect = _configuration["SendGrid:ConfirmEmailRedirectUrl"];
                if (!string.IsNullOrWhiteSpace(redirect))
                {
                    return Redirect($"{redirect}?success=true&email={WebUtility.UrlEncode(user.Email)}");
                }

                return Content("<html><body><h3>Email confirmed. You can close this tab and sign in.</h3></body></html>", "text/html");
            }

            return Content("<html><body><h3>Invalid or expired confirmation link.</h3></body></html>", "text/html");
        }

        [HttpGet("google-signin")]
        public IActionResult GoogleSignIn()
        {
            string? redirectUrl = Url.Action(nameof(GoogleResponse), "Account");
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        //Callback endpoint that processes the external login info from Google.
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            //Retrieve external login info from the temporary external cookie.
            ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("Error loading external login information.");

            //Retrieve the user's email from the external login claims.
            string? email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim not received from Google.");

            // Try existing external login
            Microsoft.AspNetCore.Identity.SignInResult? signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false);

            ApplicationUser? user;

            if (signInResult.Succeeded)
            {
                user = await _userManager.FindByEmailAsync(email);
            }
            else
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                        return BadRequest("Error creating user.");
                    await _userManager.AddToRoleAsync(user, "Student");
                }

                // Link external login
                IdentityResult addLoginResult = await _userManager.AddLoginAsync(user, info);
            }

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
                    }}, 'http://localhost:5173/');
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] Services.DTOs.Account.ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email!);

            // Always return 200 to prevent account enumeration
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok(new { message = "If an account with that email exists, a password reset email has been sent." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            // Link to GET endpoint that will redirect to FE
            var callbackUrl = Url.Action(
                nameof(BeginResetPassword),
                "Account",
                new { userId = user.Id, token = encodedToken },
                protocol: Request.Scheme
            );

            string html = $@"
        <p>We received a request to reset your EngConnect password.</p>
        <p><a href=""{callbackUrl}"">Reset your password</a></p>
        <p>If you didn't request this, you can safely ignore this email.</p>";

            await _emailService.SendEmailAsync(user.Email!, "Reset your EngConnect password", html);

            return Ok(new { message = "If an account with that email exists, a password reset email has been sent." });
        }

        // GET: redirect to frontend with the token/userId
        [HttpGet("reset-password")]
        public IActionResult BeginResetPassword([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid reset link.");

            string? redirect = _configuration["SendGrid:PasswordResetRedirectUrl"];
            if (!string.IsNullOrWhiteSpace(redirect))
            {
                return Redirect($"{redirect}?userId={WebUtility.UrlEncode(userId)}&token={WebUtility.UrlEncode(token)}");
            }

            return Content("<html><body><h3>Open the app to complete your password reset.</h3></body></html>", "text/html");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] Services.DTOs.Account.ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(request.UserId!);
            if (user == null)
                return BadRequest(new { message = "Invalid user." });

            var decodedToken = WebUtility.UrlDecode(request.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken!, request.NewPassword!);

            if (result.Succeeded)
                return Ok(new { message = "Password has been reset successfully." });

            return BadRequest(result.Errors);
        }
    }
}
