using Microsoft.AspNetCore.Identity;

namespace DotNetIdentity.Models;

public class AppUser : IdentityUser
{
    public DateTime Birthdate { get; set; }
    public string Url { get; set; }
    public int CountryCode { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public int State { get; set; }
}