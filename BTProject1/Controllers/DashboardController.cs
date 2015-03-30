using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BTProject1.Models;
using DataTables.Mvc;

namespace BTProject1.Controllers
{   
    [Authorize]
    public class DashboardController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var userId = user.Id;
            var model = new DashboardViewModel();

            var recentTickets = db.Tickets.OrderByDescending(d => d.DateUpdated).Take(10);

            if (User.IsInRole("Administrator"))
                recentTickets = db.Tickets.OrderByDescending(d => d.DateUpdated).Take(10);
            else if (User.IsInRole("Project Manager"))
                recentTickets = user.Projects.SelectMany(t => t.Tickets).AsQueryable();
            else if (User.IsInRole("Developer"))
                recentTickets = db.Tickets.Where(p => p.AssignedUserId == userId);
            else recentTickets = db.Tickets.Where(p => p.SubmitterId == userId);
           
            model.numTickets = db.Tickets.Count(t => t.Status.Name == "Active");
            

            model.numProjects = db.Projects.Count();
            model.numUsers = db.Users.Count();
            model.recentTickets = recentTickets.ToList();


            return View(model);
        }
    }
}