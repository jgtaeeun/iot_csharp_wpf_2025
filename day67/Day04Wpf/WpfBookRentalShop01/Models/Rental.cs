using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBookRentalShop01.Models
{
    public class Rental
    {
        public int Idx { get; set; }
        public int MemberIdx { get; set; }  
        public int BookIdx { get; set; }
        public DateTime RentalDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public string MNames { get; set; }

        public string BNames { get; set; }
    }
}
