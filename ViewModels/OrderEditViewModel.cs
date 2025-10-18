using Cucina_De_Corazon.Models;

namespace Cucina_De_Corazon.ViewModels
{
    public class OrderEditViewModel
    {
        public int OrderId { get; set; }
        public DateTime? ReservedDate { get; set; }
        public string? SpecialInstructions { get; set; }
        public List<OrderProduct> OrderProducts { get; set; } = new();
        public List<Product> AvailableProducts { get; set; } = new();

        // For POST
        public List<int> OrderProductIds { get; set; } = new();
        public List<int> ProductIds { get; set; } = new();
        public List<int> Quantities { get; set; } = new();
        public string? Instructions { get; set; }
    }
}
