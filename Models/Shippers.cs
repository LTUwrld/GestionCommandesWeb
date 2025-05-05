using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCommandesWeb.Models
{
    using System;
    using System.Collections.Generic;
    using GestionCommandesWeb.Models;

    public partial class Shippers
    {
        public Shippers()
        {
            this.Orders = new HashSet<Orders>();
        }

        [Key]
        public int ShipperID { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}

