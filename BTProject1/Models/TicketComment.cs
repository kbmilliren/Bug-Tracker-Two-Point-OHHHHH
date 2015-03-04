using BTProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTProject1.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string AssignedUserId { get; set; }
        public int TicketId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
    }
}