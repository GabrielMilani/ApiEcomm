using ApiEcomm.Models;

namespace ApiEcomm.ViewModels.Products
{
    public class ListProductsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdateDate { get; set; } = DateTime.Now;
        public Category Category { get; set; }
    }
}
