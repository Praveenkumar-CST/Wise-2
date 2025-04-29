using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WiseHRServer.Models
{
    [Table("roles")]
    public class Role : BaseModel
    {
        [Column("user_id")]
        public string? UserId { get; set; }

        [Column("role")]
        public string? RoleName { get; set; }
    }
}