using Cucina_De_Corazon.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Diagnostics.CodeAnalysis;

namespace Cucina_De_Corazon.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        [ValidateNever]
        public List<Product> Products { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public Payment Payment { get; set; } = new();
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
}
}

