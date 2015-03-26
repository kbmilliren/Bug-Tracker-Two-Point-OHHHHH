using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BTProject1.Models;
using Microsoft.AspNet.Identity;
using DataTables.Mvc;
using System.IO;
using SendGrid;
using System.Net.Mail;

namespace BTProject1.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets

        public ActionResult Index()
        {

            return View();
        }

        public JsonResult GetTickets([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestParameters)
        {

            var user = db.Users.Single(u => u.UserName == User.Identity.Name);
            var userId = user.Id;
            IQueryable<Ticket> tickets;
            if (User.IsInRole("Administrator"))
                tickets = db.Tickets;
            else if (User.IsInRole("Project Manager"))
                tickets = user.Projects.SelectMany(t => t.Tickets).AsQueryable();
            else if (User.IsInRole("Developer"))
                tickets = db.Tickets.Where(p => p.AssignedUserId == userId);
            else tickets = db.Tickets.Where(p => p.SubmitterId == userId);

            var totalCount = tickets.Count();
            var search = requestParameters.Search.Value;
            // do your stuff...

            if (!string.IsNullOrWhiteSpace(search))
            {
                tickets = tickets.Where(t => t.Title.Contains(search) || t.Description.Contains(search)
                    || t.Project.Name.Contains(search) || t.Submitter.Email.Contains(search) || t.Status.Name.Contains(search) || t.Priority.Name.Contains(search)
                    || t.AssignedUser.Email.Contains(search));
            }

            tickets = tickets.OrderByDescending(t => t.DateCreated);

            var column = requestParameters.Columns.FirstOrDefault(r => r.IsOrdered == true);
            if (column != null)
            {
                if (column.SortDirection == Column.OrderDirection.Descendant)
                {
                    switch (column.Data)
                    {
                        case "Title":
                            tickets = tickets.OrderByDescending(t => t.Title);
                            break;
                        case "Description":
                            tickets = tickets.OrderByDescending(t => t.Description);
                            break;
                        case "AssignedUser":
                            tickets = tickets.OrderByDescending(t => t.AssignedUser);
                            break;
                        case "Submitter":
                            tickets = tickets.OrderByDescending(t => t.Submitter);
                            break;
                        case "DateUpdated":
                            tickets = tickets.OrderByDescending(t => t.DateUpdated);
                            break;
                        case "DateCreated":
                            tickets = tickets.OrderByDescending(t => t.DateCreated);
                            break;
                        case "TicketPriority":
                            tickets = tickets.OrderByDescending(t => t.Priority);
                            break;
                        case "TicketStatus":
                            tickets = tickets.OrderByDescending(t => t.Status);
                            break;
                        case "TicketType":
                            tickets = tickets.OrderByDescending(t => t.Type);
                            break;
                    }
                }
                else

                    switch (column.Data)
                    {
                        case "Title":
                            tickets = tickets.OrderBy(t => t.Title);
                            break;
                        case "Description":
                            tickets = tickets.OrderBy(t => t.Description);
                            break;
                        case "AssignedUser":
                            tickets = tickets.OrderBy(t => t.AssignedUser);
                            break;
                        case "Submitter":
                            tickets = tickets.OrderBy(t => t.Submitter);
                            break;
                        case "DateUpdated":
                            tickets = tickets.OrderBy(t => t.DateUpdated);
                            break;
                        case "DateCreated":
                            tickets = tickets.OrderBy(t => t.DateCreated);
                            break;
                        case "TicketPriority":
                            tickets = tickets.OrderBy(t => t.Priority);
                            break;
                        case "TicketStatus":
                            tickets = tickets.OrderBy(t => t.Status);
                            break;
                        case "TicketType":
                            tickets = tickets.OrderBy(t => t.Status);
                            break;
                    }
            }

            var paged = tickets.Skip(requestParameters.Start).Take(requestParameters.Length).ToList().Select(t => new TicketViewModel(t));
            return Json(new DataTablesResponse(requestParameters.Draw, paged, tickets.Count(), totalCount));
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            if (TempData["AttachError"] != null)
                ViewBag.AttachError = TempData["AttachError"];

            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "Email");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,DateCreated,DateUpdated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId,SubmitterId,AssignedUserId")] Ticket ticket, string AttachmentDescription, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {


                db.Tickets.Add(ticket);
                ticket.DateCreated = DateTimeOffset.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "Email", ticket.AssignedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "Email", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            TempData["old_ticket"] = ticket;
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "Email", ticket.AssignedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "Email", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,DateCreated,DateUpdated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId,SubmitterId,AssignedUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var old_ticket = (Ticket)TempData["old_ticket"];
                var userId = db.Users.Single(u => u.UserName == User.Identity.Name).Id;



                if (old_ticket.AssignedUser != ticket.AssignedUser)
                {
                    var assignedUser = db.Users.Find(ticket.AssignedUserId);
                    var projectName = db.Projects.Find(ticket.ProjectId).Name;
                    var mailer = new EmailService();
                    mailer.SendAsync(new IdentityMessage { Destination = assignedUser.Email, Subject = "New Ticket", Body = "You have been added to the ticket titled " + ticket.Title + " on project " + projectName + "." });



                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "AssignedUser",
                        DateChanged = DateTimeOffset.Now,
                        OldValue = db.Users.Find(old_ticket.AssignedUserId).Email,
                        NewValue = db.Users.Find(ticket.AssignedUserId).Email,
                        TicketId = ticket.Id,
                        UserId = userId
                    });
                }

                if (old_ticket.Description != ticket.Description)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Description",
                        DateChanged = DateTimeOffset.Now,
                        OldValue = old_ticket.Description,
                        NewValue = ticket.Description,
                        TicketId = ticket.Id,
                        UserId = userId
                    });

                }
                if (old_ticket.Title != ticket.Title)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Title",
                        DateChanged = DateTimeOffset.Now,
                        OldValue = old_ticket.Title,
                        NewValue = ticket.Title,
                        TicketId = ticket.Id,
                        UserId = userId
                    });



                }

                if (old_ticket.ProjectId != ticket.ProjectId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Project",
                        DateChanged = DateTimeOffset.Now,
                        OldValue = old_ticket.Project.Name,
                        NewValue = db.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId).Name,
                        TicketId = ticket.Id,
                        UserId = userId
                    });

                }

                if (old_ticket.TicketStatusId != ticket.TicketStatusId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Status",
                        DateChanged = DateTimeOffset.Now,
                        OldValue = old_ticket.Status.Name,
                        NewValue = db.TicketStatuses.FirstOrDefault(p => p.Id == ticket.TicketStatusId).Name,
                        TicketId = ticket.Id,
                        UserId = userId
                    });

                }

                if (old_ticket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Priority",
                        DateChanged = DateTimeOffset.Now,
                        OldValue = old_ticket.Priority.Name,
                        NewValue = db.TicketPriorities.FirstOrDefault(p => p.Id == ticket.TicketPriorityId).Name,
                        TicketId = ticket.Id,
                        UserId = userId
                    });

                }

                if (old_ticket.TicketTypeId != ticket.TicketTypeId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Type",
                        DateChanged = DateTimeOffset.Now,
                        OldValue = old_ticket.Type.Name,
                        NewValue = db.TicketTypes.FirstOrDefault(p => p.Id == ticket.TicketTypeId).Name,
                        TicketId = ticket.Id,
                        UserId = userId
                    });

                }

                ticket.DateUpdated = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.Entry(ticket).Property(p => p.DateCreated).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "Email", ticket.AssignedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "Email", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);

        }

        // GET: Tickets/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment([Bind(Include = "TicketId,Comment")] TicketComment addition)
        {
            if (ModelState.IsValid)
            {
                addition.AssignedUserId = User.Identity.GetUserId();
                addition.DateCreated = System.DateTimeOffset.Now;
                db.TicketComments.Add(addition);
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = addition.TicketId });
            }

            return RedirectToAction("Details", new { Id = addition.TicketId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttachment([Bind(Include = "TicketId,Description")] TicketAttachment post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    if (image.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        var fileExtension = Path.GetExtension(fileName);
                        if ((fileExtension == ".jpg") || (fileExtension == ".gif") || (fileExtension == ".jpeg") || (fileExtension == ".png"))
                        {
                            var path = Server.MapPath("~/Uploads/Attachments/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            var filePath = Path.Combine(path, fileName);
                            image.SaveAs(filePath);

                            post.FilePath = filePath;
                            post.FileUrl = "/Uploads/Attachments/" + fileName;
                            post.UserId = User.Identity.GetUserId();
                            post.DateCreated = System.DateTimeOffset.Now;

                        }
                        else
                        {
                            TempData["AttachError"] = "Invalid image extension. Only .jpg, gif, png";
                            return RedirectToAction("Details", new { Id = post.TicketId });
                        }
                    }

                }

                post.DateCreated = System.DateTimeOffset.Now;
                db.TicketAttachments.Add(post);
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = post.TicketId });

            }
            return RedirectToAction("Details", new { Id = post.TicketId });

        }

        /*public ActionResult TicketCount()
        {
            var tickets = db.Tickets.Where(p => p.TicketStatusId == 1);
            return TicketCount().Count;
           

        }*/

        
    }
    
}
