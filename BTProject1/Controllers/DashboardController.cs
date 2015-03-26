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

            var model = new DashboardViewModel();

            var recentTickets = db.Tickets.OrderByDescending(d => d.DateUpdated).Take(10);

       
            model.numTickets = db.Tickets.Count(t => t.Status.Name == "Active");
            

            model.numProjects = db.Projects.Count();
            model.numUsers = db.Users.Count();
            model.recentTickets = recentTickets.ToList();


            return View(model);
        }
    }
}