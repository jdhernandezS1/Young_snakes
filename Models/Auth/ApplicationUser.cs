using Microsoft.AspNetCore.Identity;

namespace Young_snakes.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        // Relación con Team
        public int? IdTeam { get; set; }
    }
}