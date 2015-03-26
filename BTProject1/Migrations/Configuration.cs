namespace BTProject1.Migrations
{
    using BTProject1.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BTProject1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BTProject1.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Administrator"))
            {
                roleManager.Create(new IdentityRole("Administrator"));
            }
            if (!roleManager.RoleExists("Project Manager"))
            {
                roleManager.Create(new IdentityRole("Project Manager"));
            }

            if (!roleManager.RoleExists("Developer"))
            {
                roleManager.Create(new IdentityRole("Developer"));
            }

            if (!roleManager.RoleExists("Submitter"))
            {
                roleManager.Create(new IdentityRole("Submitter"));
            }


            var user = userManager.FindByName("kbmilliren@northstate.net");
            if (user == null)
            {
                user = new ApplicationUser { UserName = "kbmilliren@northstate.net", Email = "kbmilliren@northstate.net" };
                var result = userManager.Create(user, "Password-1");

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Administrator");
                }
            }
            else
            {
                if (!userManager.IsInRole(user.Id, "Administrator"))
                {
                    userManager.AddToRole(user.Id, "Administrator");
                }
            }

            if (!context.TicketStatuses.Any(t => t.Name == "New"))
            {
                context.TicketStatuses.Add(new TicketStatus { Name = "New" });
            }
            if (!context.TicketStatuses.Any(t => t.Name == "Active"))
            {
                context.TicketStatuses.Add(new TicketStatus { Name = "Active" });
            }
            if (!context.TicketStatuses.Any(t => t.Name == "Resolved"))
            {
                context.TicketStatuses.Add(new TicketStatus { Name = "Resolved" });
            }
            if (!context.TicketPriorities.Any(t=>t.Name == "Low"))
            {
                context.TicketPriorities.Add(new TicketPriority { Name = "Low" });
            }
            if (!context.TicketPriorities.Any(t => t.Name == "High"))
            {
                context.TicketPriorities.Add(new TicketPriority { Name = "High" });
            }
            if (!context.TicketPriorities.Any(t => t.Name == "Urgent"))
            {
                context.TicketPriorities.Add(new TicketPriority { Name = "Urgent" });
            }

            if (!context.TicketTypes.Any(t => t.Name == "Bug" ))
            {
                context.TicketTypes.Add(new TicketType { Name = "Bug" });
            }
            if (!context.TicketTypes.Any(t => t.Name == "Enhancement"))
            {
                context.TicketTypes.Add(new TicketType { Name = "Enhancement" });
            }

            context.SaveChanges();

        }
    }
}
