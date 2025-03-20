using System.ComponentModel.DataAnnotations;

namespace GestionCommandesWeb.Models
{
    public class Products
    {
        public Products()
        {
            this.Order_Details = new HashSet<Order_Details>();
        }

        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public virtual ICollection<Order_Details> Order_Details { get; set; }

    }
}