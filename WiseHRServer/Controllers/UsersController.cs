using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using WiseHRServer.Models;
namespace WiseHRServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Supabase.Client _supabase;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public UsersController(Supabase.Client supabase, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _supabase = supabase;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var supabaseUrl = _configuration["Supabase:Url"];
                var serviceRoleKey = _configuration["Supabase:ServiceRoleKey"];

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
                client.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

                var userPayload = new
                {
                    email = request.Email,
                    password = request.Password,
                    email_confirm = true
                };
                var content = new StringContent(JsonSerializer.Serialize(userPayload), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{supabaseUrl}/auth/v1/admin/users", content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var createdUser = JsonSerializer.Deserialize<SupabaseUser>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var roleData = new Role { UserId = createdUser.Id, RoleName = "Employee" };
                await _supabase.From<Role>().Insert(roleData);

                return Ok(new User
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Role = "Employee"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating user: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var supabaseUrl = _configuration["Supabase:Url"];
                var serviceRoleKey = _configuration["Supabase:ServiceRoleKey"];

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
                client.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

                var response = await client.GetAsync($"{supabaseUrl}/auth/v1/admin/users");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<SupabaseUsersResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var rolesResponse = await _supabase.From<Role>().Get();
                var roles = rolesResponse.Models;

                var result = users.Users
                    .Select(u => new User
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Role = roles.FirstOrDefault(r => r.UserId == u.Id)?.RoleName ?? "No Role"
                    })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] string role)
        {
            try
            {
                var roleData = new Role { UserId = id, RoleName = role };
                await _supabase.From<Role>().Upsert(roleData);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var supabaseUrl = _configuration["Supabase:Url"];
                var serviceRoleKey = _configuration["Supabase:ServiceRoleKey"];

                // Delete role first
                await _supabase.From<Role>().Where(r => r.UserId == id).Delete();

                // Delete user using admin endpoint
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
                client.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

                var response = await client.DeleteAsync($"{supabaseUrl}/auth/v1/admin/users/{id}");
                response.EnsureSuccessStatusCode();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }

    public class SupabaseUsersResponse
    {
        public List<SupabaseUser> Users { get; set; }
    }

    public class SupabaseUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}