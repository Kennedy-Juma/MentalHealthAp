using Azure;
using MentalHealthAp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using MentalHealthAp.Services;
using Microsoft.AspNetCore.Authorization;

namespace MentalHealthAp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly MailSettings _mailSettings;

        public AccountController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IEmailService emailService,
            MailSettings mailSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _mailSettings = mailSettings;
        }

        //Login 
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login userLogin)
        {
            var user = await _userManager.FindByNameAsync(userLogin.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, userLogin.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo.AddSeconds(3600)
                    
                });
            }
            return Unauthorized();
        }

        //sign up
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register userRestration)
        {
            var userExists = await _userManager.FindByNameAsync(userRestration.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Status = "Error", Message = "User already exists!" });

            var user = new AppUser
            {

                Email = userRestration.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userRestration.Email,
                FullName = userRestration.FullName,
                PhoneNumber = userRestration.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, userRestration.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new ApiResponse { Status = "Success", Message = "User created successfully!" });
        }

        //edit user profile
        [HttpPost]
        [Route("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfile editProfile)
        {
            var user = await _userManager.FindByNameAsync(editProfile.Email);
            if (user == null)
            {
                return NotFound(new ApiResponse { Status = "Error", Message = "User not found!" });
            }
            user.FullName = editProfile.FullName;
            user.PhoneNumber = editProfile.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new ApiResponse { Status = "Success", Message = "Profile updated successfully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Status = "Error", Message = "Failed to update profile!" });
            }
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            var user = await _userManager.FindByNameAsync(changePassword.Email);
            if (user == null)
            {
                return NotFound(new ApiResponse { Status = "Error", Message = "User not found!" });
            }
            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, changePassword.OldPassword);
            if (!isOldPasswordValid)
            {
                return BadRequest(new ApiResponse { Status = "Error", Message = "Old password is incorrect!" });
            }
            var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new ApiResponse { Status = "Success", Message = "Password changed successfully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Status = "Error", Message = "Failed to change password!" });
            }
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPassword resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user == null)
            {
                return NotFound(new ApiResponse { Status = "Error", Message = "User not found!" });
            }

            string newPassword = GenerateRandomPassword();

            var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), newPassword);

            if (result.Succeeded)
            {
                bool emailSent = SendPasswordResetEmail(user.Email, newPassword);

                if (emailSent)
                {
                    return Ok(new ApiResponse { Status = "Success", Message = "New password sent to the user's email!" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Status = "Error", Message = "Failed to send the new password via email!" });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse { Status = "Error", Message = "Failed to reset the password!" });
            }
        }

        //logout
        [AllowAnonymous]
        [Route("logout")]
        [HttpPost]
        public IActionResult LogOut()
        {
            //Delete the Cookie from Browser.
            Response.Cookies.Delete("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoia2VubmVkeWp1bWE0ODBAZ21haWwuY29tIiwianRpIjoiYTQ4ZjcwZmMtMmJmNy00NWU0LWFkYjctNTU5MDcwYjIxY2M2IiwiZXhwIjoxNzAzMDIzODc5LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjQyMDAifQ.m4Bu3bU7eIjAFytBpOfZjzhn3tZ-3H9aaSiSPBbnhhk");

            // Send a response to clear the client-side token
            Response.Headers.Add("Clear-Site-Data", "\"*\"");

            return Ok("User Logged Out");
        }


        //jwt token generator
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private bool SendPasswordResetMail(string email, string newPassword)
        {
            try
            {

                var smtpClient = new SmtpClient(_mailSettings.Server)
                {
                    Port = Int32.Parse(_mailSettings.Port),
                    Credentials = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password),
                    EnableSsl = true,
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_mailSettings.SenderEmail),
                    Subject = "Important : Password Reset",
                    Body = String.Format("<p>Use the temporary password below to login and change your password. This temporary password will expire after 1 hour. <br /><br />" +
                    "<h1> <strong style='background-color: #317399; padding: 15px; color: #fff;border-radius: 25px'> {0} </strong></h1></p>", newPassword),
                    Priority = MailPriority.High,
                    IsBodyHtml = true,

                };
                message.To.Add(email);

                smtpClient.Send(message);
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
        private string GenerateRandomPassword()
        {
            // Use a regular expression to generate a random password
            string regexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!])(?!.*\s).{8,}$";
            Random random = new Random();
            string newPassword = "";

            while (!Regex.IsMatch(newPassword, regexPattern))
            {
                newPassword = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%^&+=!", 12)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return newPassword;
        }

        // [Authorize]
        [HttpGet]
        [Route("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByEmailAsync(id);

            if (user != null)
            {
                var userDetails = new
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.PhoneNumber
                };

                return Ok(userDetails);
            }

            return NotFound(new { message = "User not found" });
        }



        private bool SendPasswordResetEmail(string userEmail, string newPassword)
        {
            try
            {

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("kennedyjuma1997@gmail.com", "nnympgdxbjgznplr"),
                    EnableSsl = true,
                };

                var message = new MailMessage
                {
                    From = new MailAddress("kennedyjuma1997@gmail.com"),
                    Subject = "Important : Password Reset",
                    Body = String.Format("<p>Use the temporary password below to login and change your password. This temporary password will expire after 1 hour. <br /><br />" +
                    "<h1> <strong style='background-color: #317399; padding: 15px; color: #fff;border-radius: 25px'> {0} </strong></h1></p>", newPassword),
                    Priority = MailPriority.High,
                    IsBodyHtml = true,

                };



                message.To.Add(userEmail);

                smtpClient.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }
}
