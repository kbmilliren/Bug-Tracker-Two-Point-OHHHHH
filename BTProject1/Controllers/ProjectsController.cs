using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BTProject1.Models;
using PagedList;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;

namespace BTProject1.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects

        public ActionResult Index(int? page, string query)
        {
            var projects = db.Projects.AsQueryable();
            if (!String.IsNullOrWhiteSpace(query))
            {
                projects = projects.Where(p => p.Name.Contains(query));
                ViewBag.Query = query;

            }
            projects = projects.OrderByDescending(p => p.Name);
            return View(projects.ToPagedList(page ?? 1, 3));
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                project.DateCreated = DateTimeOffset.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var model = new ProjectViewModel { Id = project.Id, Name = project.Name };
            model.PossibleUsersToRemove = new MultiSelectList(project.ApplicationUsers.ToList(), "Id", "UserName");
            model.PossibleUsersToAssign = new MultiSelectList(
                    db.Users.Where(u => !u.Projects.Any(p=>p.Id == id)).ToList(), "Id", "UserName" );

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectViewModel model)
        {
            var project = db.Projects.Find(model.Id);
            if (model.NewlyRemovedUsers != null)
            {
                foreach (var userId in model.NewlyRemovedUsers)
                {
                    var tempUser = new ApplicationUser();
                    tempUser.Id = userId;
                    db.Users.Attach(tempUser);
                    project.ApplicationUsers.Remove(tempUser);
                }
            }

            
            if(model.NewlyAssignedUsers != null)
            {
                foreach (var userId in model.NewlyAssignedUsers)
                {
                    var tempUser = new ApplicationUser();
                    tempUser.Id = userId;
                    db.Users.Attach(tempUser);
                    project.ApplicationUsers.Add(tempUser);
                }
            }


            db.SaveChanges();

            return RedirectToAction("Edit", new { Id = model.Id });

        }
      
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment([Bind(Include = "ProjectId,Comment")] ProjectComment addition)
        {
            if (ModelState.IsValid)
            {
                addition.AssignedUserId = User.Identity.GetUserId();
                addition.DateCreated = System.DateTimeOffset.Now;
                db.ProjectComments.Add(addition);
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = addition.ProjectId });
            }

            return RedirectToAction("Details", new { Id = addition.ProjectId });
        }*/
  
    }
}

