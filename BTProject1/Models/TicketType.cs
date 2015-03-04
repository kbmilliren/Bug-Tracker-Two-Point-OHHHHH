﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTProject1.Models
{
    public class TicketType
    {
        public TicketType()
        {
            Tickets = new HashSet<Ticket>();
        }
         
        public int Id { get; set; }
        public string Name { get; set; }
       
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}