using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Banner : BaseEntity
    {
        public string Image { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public bool isActive { get; set; }

    }
}
