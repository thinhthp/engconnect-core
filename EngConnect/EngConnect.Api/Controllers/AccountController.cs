using EngConnect.Entities.Entities;
using EngConnect.Services.DTOs.Account;
using EngConnect.Services.Services.Auth;
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
        private readonly IAuthTokenService _authTokenService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IEmailService emailService, IAuthTokenService authTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _authTokenService = authTokenService;
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
            if (!user.EmailConfirmed)
            {
                return BadRequest(new { message = "Email not confirmed. Please check your inbox." });
            }
            if (user.IsActive == false)
            {
                return BadRequest(new { message = "Your account has been deactivated. Please contact support for assistance." });
            }

            //Get role
            //IList<string> roles = await _userManager.GetRolesAsync(user);
            //var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

            //JWT
            var jwt = await GenerateToken(user);
            var refreshToken = await _authTokenService.CreateRefreshTokenAsync(user!, jwt.JwtId, jwt.Expires);

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

                var authResponse = new AuthResponse
                {
                    User = response,
                    AccessToken = jwt.Token,
                    RefreshToken = refreshToken
                };

                return Ok(authResponse);
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
            ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("Error loading external login information.");

            string? email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim not received from Google.");

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
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

                IdentityResult addLoginResult = await _userManager.AddLoginAsync(user, info);
            }

            var jwt = await GenerateToken(user!);
            var refreshToken = await _authTokenService.CreateRefreshTokenAsync(user!, jwt.JwtId, jwt.Expires);

            UserResponse response = new UserResponse
            {
                Id = user!.Id,
                Name = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                IsActive = user.IsActive,
                CreateDate = user.CreatedAt,
                UpdateDate = user.UpdateDate,
                CreateBy = user.CreateBy,
                UpdateBy = user.UpdateBy
            };

            string htmlResponse = $@"
                <html>
                <body>
                    <script>
                    window.opener.postMessage({{
                        token: '{jwt.Token}',
                        refreshToken: '{refreshToken}',
                        response: {System.Text.Json.JsonSerializer.Serialize(response)}
                    }}, 'http://localhost:5173/');
                    </script>
                </body>
                </html>";

            return Content(htmlResponse, "text/html");
        }

        private async Task<(string Token, string JwtId, DateTime Expires)> GenerateToken(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            var nowUtc = DateTime.UtcNow;
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            byte[] key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!);

            var jwtId = Guid.NewGuid().ToString("N");
            var expires = nowUtc.AddHours(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtId)
                }.Concat(roleClaims)),
                IssuedAt = nowUtc,
                NotBefore = nowUtc,
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(tokenDescriptor);
            string tokenString = handler.WriteToken(token);

            return (tokenString, jwtId, expires);
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token;
            try
            {
                token = handler.ReadJwtToken(request.AccessToken);
            }
            catch
            {
                return BadRequest(new { message = "Invalid access token." });
            }

            string? userId = token.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            string? jti = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (userId == null || jti == null)
                return BadRequest(new { message = "Invalid access token payload." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            var refreshToken = await _authTokenService.GetRefreshTokenAsync(userId, request.RefreshToken);

            if (refreshToken == null)
                return Unauthorized(new { message = "Refresh token not found." });

            if (refreshToken.IsUsed || refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
                return Unauthorized(new { message = "Refresh token is invalid." });

            if (refreshToken.JwtId != jti)
                return Unauthorized(new { message = "Token pair mismatch." });

            await _authTokenService.MarkRefreshTokenUsedAsync(refreshToken);

            var newJwt = await GenerateToken(user);
            var newRefreshToken = await _authTokenService.CreateRefreshTokenAsync(user, newJwt.JwtId, newJwt.Expires);

            var response = new UserResponse
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

            return Ok(new AuthResponse
            {
                User = response,
                AccessToken = newJwt.Token,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return Ok();

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Ok();

            await _authTokenService.RevokeAllUserRefreshTokensAsync(userId);

            await _signInManager.SignOutAsync();

            return Ok(new { message = "Logged out successfully." });
        }
    }
}
