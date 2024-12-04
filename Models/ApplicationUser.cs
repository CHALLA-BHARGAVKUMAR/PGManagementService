using Microsoft.AspNetCore.Identity;

namespace PGManagementService.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Name { get; set; }
    }
}