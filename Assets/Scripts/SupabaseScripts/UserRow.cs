using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SupabaseScripts
{
    [Table("Users")]
    public class UserRow : BaseModel
    {
        [PrimaryKey("id")] public string Id { get; set; }
        [Column("last_zip")] public string lastZip { get; set; }
        [Column("last_check")] public string lastCheck { get; set; }
    }
}