using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinancialPlanner.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public int? HouseholdId { get; set; }

        public virtual Household Household { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Household> households { get; set; }
        public DbSet<Account> accounts { get; set; }
        public DbSet<Budget> budgets { get; set; }
        public DbSet<AccountType> accountTypes { get; set; }
        public DbSet<Categories> categories { get; set; }
        public DbSet<SubCategories> subCategories { get; set; }
        public DbSet<Transaction> transactions { get; set; }
        public DbSet<TransactionStatus> transactionStatuses { get; set; }
        public DbSet<TransactionType> transactionTypes { get; set; }
        public DbSet<JoinNotifications> joinNotifications { get; set; }
    }
}