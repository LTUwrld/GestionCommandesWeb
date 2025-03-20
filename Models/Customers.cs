using System.ComponentModel.DataAnnotations;

namespace GestionCommandesWeb.Models
{
    public class Customers
    {
        public Customers()
        {
            this.Orders = new HashSet<Orders>();
        }

        [Key]
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }

    }
}