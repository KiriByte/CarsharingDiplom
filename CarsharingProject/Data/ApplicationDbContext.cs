using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarsharingProject.Models;

namespace CarsharingProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           Database.Migrate();
        }
        public DbSet<BankCardModel> BankCards{ get; set; }
        public DbSet<RentCarsModel> RentCars { get; set; }
        public DbSet<RentModel> RentHistory { get; set; }
    }
}