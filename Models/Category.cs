namespace ApiEcomm.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public IList<Product> Products { get; set; }
    }
}
