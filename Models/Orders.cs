using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCommandesWeb.Models
{
    public class Orders
    {
        public Orders()
        {
            this.Order_Details = new HashSet<Order_Details>();
        }

        [Key]
        public int OrderID { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }

        [ForeignKey("Customers")]
        public string CustomerID { get; set; }
        public virtual Customers Customers { get; set; }
        public virtual ICollection<Order_Details> Order_Details { get; set; }
    }
}
