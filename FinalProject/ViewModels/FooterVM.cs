using FinalProject.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class FooterVM
    {
        public IEnumerable<Models.Social> Socials { get; set; }
        public List<BasketDetailVM> BasketProduct { get; set; }
    }
}
