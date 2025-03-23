using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Common.Data
{
    public abstract class ApplicationIdentityDbContext<TIdentity>(DbContextOptions options) : IdentityDbContext<TIdentity>(options) where TIdentity : IdentityUser
    {
        protected void OnModelCreating<T>(ModelBuilder modelBuilder)
        {
            this.ConfigureEntities<T>(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
