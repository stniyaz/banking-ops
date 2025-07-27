using Microsoft.EntityFrameworkCore;
using SecureBankMS.Account.Models;

namespace SecureBankMS.Account.DAL;

public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
}
