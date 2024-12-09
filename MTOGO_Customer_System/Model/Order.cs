namespace MTOGO_Customer_System.Model
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public List<OrderLine> OrderLines { get; set; }
    }
}
