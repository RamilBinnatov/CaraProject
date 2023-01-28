using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Social : BaseEntity
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Logo { get; set; }
    }
}
