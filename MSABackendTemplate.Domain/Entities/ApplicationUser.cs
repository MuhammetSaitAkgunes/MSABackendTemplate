using Microsoft.AspNetCore.Identity;

namespace MSABackendTemplate.Domain.Entities
{
    // IdentityUser'dan miras alıyoruz.
    // Bu sayede Id, UserName, Email, PasswordHash, PhoneNumber gibi alanlar BEDAVA geliyor.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}