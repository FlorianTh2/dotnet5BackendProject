using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet5BackendProject.Domain
{
    public class Project : AuditableEntity
    {
        [Key] public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual List<ProjectTag> Tags { get; set; }
        
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}