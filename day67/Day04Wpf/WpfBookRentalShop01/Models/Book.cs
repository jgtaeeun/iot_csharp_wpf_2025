using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBookRentalShop01.Models
{
    public class Book
    {
        public int Idx { get; set; }
        public string Names { get; set; }
        public string Author { get; set; }
        public string Division { get; set; }
        public string DNames { get; set; }

        public string ISBN { get; set; }
        public int Price { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}
