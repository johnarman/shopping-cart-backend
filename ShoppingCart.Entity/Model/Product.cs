namespace ShoppingCart.Entity.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int ReservedQuantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
