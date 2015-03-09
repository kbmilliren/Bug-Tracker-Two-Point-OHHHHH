using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTProject1.Models
{
    public class ProjectComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string AssignedUserId { get; set; }
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
    }
}