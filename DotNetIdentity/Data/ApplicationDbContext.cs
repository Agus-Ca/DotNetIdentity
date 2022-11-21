using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetIdentity.Data;

public class ApplicationDbContext : IdentityDbContext
{
	public ApplicationDbContext(DbContextOptions options) : base(options) { }
}