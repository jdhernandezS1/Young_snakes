using Microsoft.AspNetCore.Identity;
using Young_snakes.Models;

namespace Young_snakes.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public int? IdTeam { get; set; }

        public Team Team { get; set; }
    }
}