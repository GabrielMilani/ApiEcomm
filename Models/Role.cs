namespace ApiEcomm.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public IList<User> Users { get; set; }
    }
}
