﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BTProject.Models;
using BTProject1.Models;

namespace BTProject1.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
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
        /*
        // GET: /Account/AssignUserRole
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignProjectUser(ProjectUserViewModel model)
        {

            return RedirectToAction("Edit", new { Id = model.Id });

        }
        */

        // GET: Projects/Delete/5
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
        /*
        // GET: /Account/AssignUserRole
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignProjectUser(Project model)
        {
            foreach (var userId in model.NewlyRemovedUsers)
            {
                var tempUser = new ApplicationUser();
                tempUser.Id = userId;
                db.Users.Attach(tempUser);
                db.Entry(tempUser).State = EntityState.Deleted;
            }
            

            foreach (var userId in model.NewlyAssignedUsers)
            {
                var tempUser = new ApplicationUser();
                tempUser.Id = userId;
                db.Users.Attach(tempUser);
                db.Entry(tempUser).State = EntityState.Added;
            }

            db.SaveChanges();

            return RedirectToAction("Edit", new { Id = model.Id });

        }
        */

    }
}