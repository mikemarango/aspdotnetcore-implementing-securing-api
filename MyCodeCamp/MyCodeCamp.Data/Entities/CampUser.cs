using Microsoft.AspNetCore.Identity;

namespace MyCodeCamp.Data.Entities
{
    public class CampUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
