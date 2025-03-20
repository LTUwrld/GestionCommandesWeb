using GestionCommandesWeb.Models;
using NuGet.DependencyResolver;

namespace GestionCommandesWeb.Shared
{
    public class PersistOrdersViewModel
    {
        public Orders Orders { get; set; }
        public Order_Details Order_Detail { get; set; }
        public Order_Details Update_Order_Detail { get; set; }

        public PersistOrdersViewModel()
        {
            Orders = new Orders{ OrderDate = DateTime.Now, CustomerID = ""};
            Order_Detail = new Order_Details();
            Update_Order_Detail = new Order_Details();
        }
    }
}
