namespace MTOGO_Restaurant_System.Model
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int RestaurantId { get; set; }

    }
}
