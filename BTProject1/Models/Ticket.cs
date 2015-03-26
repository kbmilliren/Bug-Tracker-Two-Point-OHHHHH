using BTProject1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BTProject1.Models
{

    public class Ticket
    {
  
      

        public int Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        [Required]
        public string Description { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public Nullable <DateTimeOffset> DateUpdated { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string AssignedToUserId { get; set; }
        public string SubmitterId { get; set; }
        public string AssignedUserId { get; set; }
        public string MediaUrl { get; set; }

        public virtual Project Project { get; set; }
        public virtual ApplicationUser Submitter { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
        public virtual TicketType Type { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual TicketPriority Priority { get; set; }


        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<TicketHistory> History { get; set; }
        public Ticket()
        {
            Comments = new HashSet<TicketComment>();
            History = new HashSet<TicketHistory>();
        }

        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        
      

    }

    public class TicketViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public Nullable <DateTimeOffset> DateUpdated { get; set; }
        public string ProjectId { get; set; }
        public string TicketType { get; set; }
        public string TicketPriority { get; set; }
        public string TicketStatus { get; set; }
        public string AssignedToUser { get; set; }
        public string SubmitterId { get; set; }
        public string AssignedUserId { get; set; }
        public string MediaUrl { get; set; }
        public string Actions { get; set; }

        public TicketViewModel(Ticket ticket)
        {
            Id = ticket.Id;
            Title = "<a href=\"/Tickets/Details/" + ticket.Id +  "\">" + ticket.Title + "</a>";
            Description = ticket.Description;
            DateCreated  = ticket.DateCreated;
            DateUpdated = ticket.DateUpdated;
            AssignedToUser = ticket.AssignedUser != null ? ticket.AssignedUser.UserName : "";;
            ProjectId = ticket.Project.Name;
            TicketType = ticket.Type.Name;
            TicketPriority = ticket.Priority.Name;
            TicketStatus = ticket.Status.Name;
            SubmitterId = ticket.Submitter != null ? ticket.Submitter.UserName : "";; 
            AssignedUserId = ticket.AssignedUserId;
            MediaUrl = ticket.MediaUrl;

        
        }
    }

}