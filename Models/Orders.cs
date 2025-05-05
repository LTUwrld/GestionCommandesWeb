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
        public string? CustomerID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<System.DateTime> RequiredDate { get; set; }
        public Nullable<System.DateTime> ShippedDate { get; set; }

        [ForeignKey("Shippers")]
        public Nullable<int> ShipVia { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public virtual Customers Customers { get; set; }
        public virtual Shippers Shippers { get; set; }
        public virtual ICollection<Order_Details> Order_Details { get; set; }
    }
}
