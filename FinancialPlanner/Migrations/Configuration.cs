namespace FinancialPlanner.Migrations
{
    using FinancialPlanner.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FinancialPlanner.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(FinancialPlanner.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Head"))
            {
                roleManager.Create(new IdentityRole { Name = "Head" });
            }


            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(r => r.Email == "jmahoney2261@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jmahoney2261@Mailinator.com",
                    Email = "jmahoney2261@Mailinator.com",
                    FirstName = "John",
                    LastName = "Mahoney"
                }, "penguins82");
            }

            var userId = userManager.FindByEmail("jmahoney2261@Mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Users.Any(r => r.Email == "bdavis@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "bdavis@Mailinator.com",
                    Email = "bdavis@Mailinator.com",
                    FirstName = "Brent",
                    LastName = "Davis"
                }, "password");
            }

            if (!context.Users.Any(r => r.Email == "jtwitchell@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jtwitchell@Mailinator.com",
                    Email = "jtwitchell@Mailinator.com",
                    FirstName = "Jason",
                    LastName = "Twitchell"
                }, "password");
            }

            if (!context.transactionTypes.Any(t => t.Name == "Checkings"))
            {
                context.accountTypes.AddOrUpdate(
                    new AccountType
                    {
                        Id = 1,
                        Name = "Checkings"
                    },
                    new AccountType
                    {
                        Id = 2,
                        Name = "Savings"
                    }
                );
            }

            context.categories.AddOrUpdate(
                new Categories
                {
                    Id = 1,
                    Name = "Utilities"
                },
                new Categories
                {
                    Id = 2,
                    Name = "Daily Life"
                },
                new Categories
                {
                    Id = 3,
                    Name = "Fun Cash"
                }
             );

            if (!context.subCategories.Any(s => s.Name == "Water Bill"))
            {
                context.subCategories.AddOrUpdate(
                    new SubCategories
                    {
                        Name = "Water Bill",
                        CategoryId = 1
                    },
                    new SubCategories
                    {
                        Name = "Power Bill",
                        CategoryId = 1
                    },
                    new SubCategories
                    {
                        Name = "Phone Bill",
                        CategoryId = 1
                    },
                    new SubCategories
                    {
                        Name = "Internet Bill",
                        CategoryId = 1
                    },
                    new SubCategories
                    {
                        Name = "Gas Bill",
                        CategoryId = 1
                    },
                    new SubCategories
                    {
                        Name = "Car Maintainence",
                        CategoryId = 1
                    },
                    new SubCategories
                    {
                        Name = "Grocery Shopping",
                        CategoryId = 2
                    },
                    new SubCategories
                    {
                        Name = "Car Gas",
                        CategoryId = 2
                    },
                    new SubCategories
                    {
                        Name = "Eating Out",
                        CategoryId = 3
                    },
                    new SubCategories
                    {
                        Name = "Vacation",
                        CategoryId = 3
                    },
                    new SubCategories
                    {
                        Name = "Misc.",
                        CategoryId = 3
                    }
                 );
            }

            if (!context.transactionStatuses.Any(t => t.Name == "Posted"))
            {
                context.transactionStatuses.AddOrUpdate(
                    new TransactionStatus
                    {
                        Id = 1,
                        Name = "Pending"
                    },
                    new TransactionStatus
                    {
                        Id = 2,
                        Name = "Posted"
                    },
                    new TransactionStatus
                    {
                        Id = 3,
                        Name = "Void"
                    }
                );
            }

            if (!context.transactionTypes.Any(t => t.Name == "Checkings"))
            {
                context.transactionTypes.AddOrUpdate(
                    new TransactionType
                    {
                        Id = 1,
                        Name = "Checkings"
                    },
                    new TransactionType
                    {
                        Id = 2,
                        Name = "Savings"
                    }
                 );
            }

        }
    }
}
