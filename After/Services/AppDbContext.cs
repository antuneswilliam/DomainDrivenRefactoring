using After.Model;
using Microsoft.EntityFrameworkCore;

namespace After.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<OfferType> OfferTypes { get; set; }
        public DbSet<Offer> Offers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseInMemoryDatabase("AppDb");
        }
    }
}
