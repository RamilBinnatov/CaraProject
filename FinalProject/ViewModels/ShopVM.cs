using FinalProject.Models;
using System.Collections.Generic;


namespace FinalProject.ViewModels
{
    public class ShopVM
    {
        public IEnumerable<Models.Product> Products { get; set; }
        public IEnumerable<PageHeader> PageHeaders { get; set; }
    }
}
