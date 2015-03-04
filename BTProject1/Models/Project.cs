using BTProject1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTProject1.Models
{
    public class Project
    {
        public Project()
        {
            Tickets = new HashSet<Ticket>();
            ApplicationUsers = new HashSet<ApplicationUser>();
        }

        public string Name { get; set; }
        public int Id { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }

    [Bind(Exclude="PossibleUsersToAssign,PossibleUsersToRemove")]
    public class ProjectViewModel
    {

        public string Name { get; set; }
        public int Id { get; set; }

        public MultiSelectList PossibleUsersToAssign { get; set; }
        public string[] NewlyAssignedUsers { get; set; }
        public MultiSelectList PossibleUsersToRemove { get; set; }
        public string[] NewlyRemovedUsers { get; set; }
    }
}
