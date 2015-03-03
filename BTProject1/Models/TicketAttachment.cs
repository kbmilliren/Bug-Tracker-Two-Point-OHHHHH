using BTProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTProject.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public int UserId { get; set; }
        public string FileUrl { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
    }
}