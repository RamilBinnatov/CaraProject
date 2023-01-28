using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }

    }
}
