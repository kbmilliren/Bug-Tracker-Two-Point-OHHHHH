﻿using BTProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTProject.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTimeOffset DateChanged { get; set; }
        public int UserId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
        
    }
}