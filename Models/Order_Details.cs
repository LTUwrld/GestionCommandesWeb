using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestionCommandesWeb.Models
{
    [Table("Order_Details")]
    public class Order_Details
    {
        public int OrderID { get; set; }

        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public virtual Orders Orders { get; set; }
        public virtual Products Products { get; set; }

    }
}