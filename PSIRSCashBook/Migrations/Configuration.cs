using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PSIRSCashBook.Models;

namespace PSIRSCashBook.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PSIRSCashBook.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PSIRSCashBook.Models.ApplicationDbContext context)
        {
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "Operator"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Operator" };

                manager.Create(role);
            }


            if (!context.Users.Any(u => u.UserName == "Admin@psirs.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "Admin@psirs.com", Email = "Admin@psirs.com", EmailConfirmed = true };

                manager.Create(user, "admin1111");
                manager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
