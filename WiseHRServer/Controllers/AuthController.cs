using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;
using MongoDB.Driver;
using WiseHRServer.Models;
using WiseHRServer.Services;
using Microsoft.AspNetCore.Identity.Data;
using Supabase.Gotrue.Exceptions;
using Supabase.Interfaces;
using System;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace WiseHRServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly Supabase.Client _supabase;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<PasswordResetOtp> _otpCollection;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _supabaseUrl;
        private readonly string _serviceRoleKey;
        private readonly HttpClient _httpClient;

        public AuthController(Supabase.Client supabase, MongoDbService mongoDbService, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _supabase = supabase;
            _usersCollection = mongoDbService.GetCollection<User>("users");
            _otpCollection = mongoDbService.GetCollection<PasswordResetOtp>("password_reset_otps");

            // SMTP configuration for sending emails
            _smtpHost = configuration["Smtp:Host"];
            _smtpPort = int.Parse(configuration["Smtp:Port"]);
            _smtpUsername = configuration["Smtp:Username"];
            _smtpPassword = configuration["Smtp:Password"];
            _fromEmail = configuration["Smtp:FromEmail"];
            _fromName = configuration["Smtp:FromName"];

            // Supabase configuration for REST API calls
            _supabaseUrl = configuration["Supabase:Url"];
            _serviceRoleKey = configuration["Supabase:ServiceRoleKey"];

            // Initialize HttpClient for REST API calls
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            Console.WriteLine($"Signup request for email: {request.Email}");
            var response = await _supabase.Auth.SignUp(request.Email, request.Password);
            if (response == null || response.User == null)
            {
                Console.WriteLine("Signup failed in Supabase.");
                return BadRequest("Signup failed.");
            }

            Console.WriteLine($"Supabase signup successful. UserId: {response.User.Id}");

            // Check if this is the first user in the database
            var userCount = await _usersCollection.CountDocumentsAsync(_ => true);
            var role = userCount == 0 ? "Admin" : "Employee"; // First user gets "admin", others get "user"

            // Create a user document in MongoDB
            var user = new User
            {
                Id = response.User.Id,
                Email = request.Email,
                Role = role,
                Onboarding = new OnboardingData
                {
                    Status = "draft",
                    AgreementAccepted = false,
                    LastUpdated = DateTime.UtcNow
                }
            };
            try
            {
                await _usersCollection.InsertOneAsync(user);
                Console.WriteLine($"User inserted into MongoDB: {user.Id} with role: {user.Role}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to insert user into MongoDB: {ex.Message}");
                throw;
            }

            if (string.IsNullOrEmpty(response.AccessToken))
                return Ok(new { Message = "Signup successful.", UserId = response.User.Id });

            return Ok(new { Token = response.AccessToken, UserId = response.User.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _supabase.Auth.SignIn(request.Email, request.Password);
            if (response == null || response.User == null)
                return Unauthorized("Invalid credentials.");

            return Ok(new { Token = response.AccessToken, UserId = response.User.Id });
        }

        [HttpGet("verify")]
        [Authorize]
        public async Task<IActionResult> Verify()
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var email = User.FindFirst("email")?.Value;

            // Fetch role from MongoDB
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                Console.WriteLine($"User {userId} not found in database. Creating new user document.");
                user = new User
                {
                    Id = userId,
                    Email = email,
                    Role = "user", // Default role
                    Onboarding = new OnboardingData
                    {
                        Status = "draft",
                        AgreementAccepted = false,
                        LastUpdated = DateTime.UtcNow
                    }
                };
                await _usersCollection.InsertOneAsync(user);
            }

            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            Console.WriteLine("Claims in Verify: " + System.Text.Json.JsonSerializer.Serialize(claims));

            return Ok(new { UserId = userId, Role = user.Role });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                // Check if the user exists in MongoDB
                var user = await _usersCollection.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                // Generate a 6-digit OTP
                var otp = new Random().Next(100000, 999999).ToString();
                var expiresAt = DateTime.UtcNow.AddMinutes(10); // OTP expires in 10 minutes

                // Store the OTP in MongoDB
                var passwordResetOtp = new PasswordResetOtp
                {
                    Email = request.Email,
                    Otp = otp,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow
                };
                await _otpCollection.InsertOneAsync(passwordResetOtp);

                // Send the OTP via email using SMTP
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = "Password Reset OTP",
                        Body = $@"Hi there,

                                We received a request to reset your password. Please use the following OTP to reset your password:

                                {otp}

                                This OTP is valid for 10 minutes. If you didn’t request a password reset, you can ignore this email.

                                Thanks,
                                WiseHR Team",
                                IsBodyHtml = false
                    };
                    mailMessage.To.Add(request.Email);

                    await smtpClient.SendMailAsync(mailMessage);
                }

                return Ok(new { message = "OTP sent to your email", email = request.Email });
            }
            catch (GotrueException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the OTP", error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                // Find the OTP in MongoDB
                var otpEntry = await _otpCollection.Find(o => o.Email == request.Email && o.Otp == request.Otp).FirstOrDefaultAsync();
                if (otpEntry == null)
                {
                    return BadRequest(new { message = "Invalid OTP" });
                }

                // Check if the OTP has expired
                if (otpEntry.ExpiresAt < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "OTP has expired" });
                }

                // Find the user in MongoDB
                var user = await _usersCollection.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                // Log the service role key for debugging
                Console.WriteLine($"Service Role Key: {_serviceRoleKey}");

                // Update the user's password in Supabase using the REST API
                var updateUrl = $"{_supabaseUrl}/auth/v1/admin/users/{user.Id}";
                var updatePayload = new { password = request.NewPassword };
                var content = new StringContent(
                    JsonSerializer.Serialize(updatePayload),
                    Encoding.UTF8,
                    "application/json"
                );

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_serviceRoleKey}");
                _httpClient.DefaultRequestHeaders.Add("apikey", _serviceRoleKey);

                // Log the headers for debugging
                Console.WriteLine($"Authorization Header: Bearer {_serviceRoleKey}");
                Console.WriteLine($"API Key Header: {_serviceRoleKey}");

                var response = await _httpClient.PutAsync(updateUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Supabase API Error: {errorContent}");
                    return StatusCode((int)response.StatusCode, new { message = "Failed to update password", error = errorContent });
                }

                // Delete the OTP from MongoDB
                await _otpCollection.DeleteOneAsync(o => o.Email == request.Email && o.Otp == request.Otp);

                return Ok(new { message = "Password reset successful" });
            }
            catch (GotrueException ex)
            {
                Console.WriteLine($"Gotrue error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred during password reset", error = ex.Message });
            }
        }

        public class SignupRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
            public string NewPassword { get; set; }
        }
    }

    public class PasswordResetOtp
    {
        public MongoDB.Bson.ObjectId Id { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}