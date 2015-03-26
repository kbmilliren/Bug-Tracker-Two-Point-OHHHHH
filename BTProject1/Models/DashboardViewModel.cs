using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTProject1.Models
{
    public class DashboardViewModel
    {
        public int numTickets { get; set; }
        public int numProjects { get; set; }
        public int numUsers { get; set; }

        public List<Ticket> recentTickets { get; set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public Nullable<DateTimeOffset> DateUpdated { get; set; }
        public string ProjectId { get; set; }
        public string TicketType { get; set; }
        public string TicketPriority { get; set; }
        public string TicketStatus { get; set; }
        public string AssignedToUser { get; set; }
        public string SubmitterId { get; set; }
        public string AssignedUserId { get; set; }
        public string MediaUrl { get; set; }
        public string Actions { get; set; }

    }
}