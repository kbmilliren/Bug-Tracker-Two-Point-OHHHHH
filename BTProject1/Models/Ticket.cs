using BTProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTProject1.Models
{

    public class Ticket
    {
  
      

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public int OwnerUserId { get; set; }
        public int AssignedToUserId { get; set; }
        public string SubmitterId { get; set; }
        public string AssignedUserId { get; set; }
        public string MediaUrl { get; set; }

        public virtual Project Project { get; set; }
        public virtual ApplicationUser Submitter { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; }
        public Ticket()
        {
            Comments = new HashSet<TicketComment>();
        }


    }
}