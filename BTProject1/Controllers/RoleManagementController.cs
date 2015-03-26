using BTProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace BTProject1.Controllers
{
    [Authorize]
    public class RoleManagementController : Controller
    {
        private ApplicationDbContext db;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get 
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public RoleManagementController()
        {
            db = new ApplicationDbContext();
        }


        // GET: /Account/ListRoles
        [Authorize(Roles = "Administrator")]
        public ActionResult ListRoles()
        {
            var db = new ApplicationDbContext();
            var model = db.Roles
                          .Select(m => new RoleViewModel() { RoleId = m.Id, RoleName = m.Name })
                          .ToList();

            return View(model);
        }

        //
        // GET: /Account/AssignUserRole
        [Authorize(Roles = "Administrator")]
        public ActionResult AssignUserRole(string roleid)
        {
            if (String.IsNullOrWhiteSpace(roleid))
                return RedirectToAction("ListRoles");
            //Code to assign users to the given role
                        ViewBag.Message = (string)TempData["ListError"];

            var model = db.Roles
                          .Where(m => m.Id == roleid)
                          .Select(m => new UserRoleViewModel { RoleId = m.Id, RoleName = m.Name })
                          .FirstOrDefault();

            var userList = db.Users
                             .Include("Roles")
                             .Where(m => (m.Roles.Count(r => r.RoleId == roleid) < 1))
                             .OrderBy(u => u.UserName)
                             .ToList();

            model.Users = new MultiSelectList(userList, "Id", "UserName");

            return View(model);

        }

        //
        // POST: /Account/AssignUserRole
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUserRole(UserRoleViewModel model)
        {
            var db = new ApplicationDbContext();
            // check the model state - if it's valid, continue, 
            //  if not, return the view with the current model
            if (ModelState.IsValid)
            {
                // check the SelectedUsers attribute of the model - if it's NOT null, continue
                if (model.SelectedUsers != null)
                {
                    // loop through the elements in model.SelectedUsers - for each one in the array, 
                    //      1) locate the user in the database, 
                    //      2) add the user to the role specified in the model
                    foreach (string userid in model.SelectedUsers)
                    {
                        UserManager.AddToRole(userid, model.RoleName);
                    }
                    // we've succeeded - go back to the roles list view
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    TempData["ListError"] = "You must select at least one user from the list.";
                    return RedirectToAction("AssignUserRole", "RoleManagement", new { roleid = model.RoleId });
                }
            }
            // if we get to this point, there's a problem - return the view with the model
            return View(model);
        }

        //
        // GET: /Account/UnassignUserRole
        [Authorize(Roles = "Administrator")]
        public ActionResult UnassignUserRole(string roleid)
        {
            ViewBag.Message = (string)TempData["ListError"];

            var model = db.Roles
                          .Where(m => m.Id == roleid)
                          .Select(m => new UserRoleViewModel { RoleId = m.Id, RoleName = m.Name })
                          .FirstOrDefault();

            var userList = db.Users
                             .Include("Roles")
                             .Where(m => (m.Roles.Count(r => r.RoleId == roleid) >= 1))
                             .OrderBy(u => u.UserName)
                             .ToList();

            model.Users = new MultiSelectList(userList, "Id", "UserName");

            return View(model);
        }

        //
        // POST: /Account/UnassignUserRole
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult UnassignUserRole(UserRoleViewModel model)
        {
            var db = new ApplicationDbContext();
            // check the model state - if it's valid, continue, 
            //  if not, return the view with the current model
            if (ModelState.IsValid)
            {
                // check the SelectedUsers attribute of the model - if it's NOT null, continue
                if (model.SelectedUsers != null)
                {
                    // loop through the elements in model.SelectedUsers - for each one in the array, 
                    //      1) locate the user in the database, 
                    //      2) add the user to the role specified in the model
                    foreach (string userid in model.SelectedUsers)
                    {
                        UserManager.RemoveFromRole(userid, model.RoleName);
                    }
                    // we've succeeded - go back to the roles list view
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    TempData["ListError"] = "You must select at least one user from the list.";
                    return RedirectToAction("UnassignUserRole", "RoleManagement",  new { roleid = model.RoleId });
                }
            }
            // if we get to this point, there's a problem - return the view with the model
            return View(model);
        }


    }
}