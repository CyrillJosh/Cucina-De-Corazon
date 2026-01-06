using Cucina_De_Corazon.Models;

namespace Cucina_De_Corazon.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<Product> Products { get; set; }
    }
}
