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
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Fax { get; set; }


        public virtual ICollection<Orders> Orders { get; set; }

    }
}