namespace Abno.Models
{
    public class EditProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public Credentials credentials { get; set; }
        public int ProductId { get; internal set; }
    }
}
