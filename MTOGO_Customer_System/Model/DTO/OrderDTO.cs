namespace MTOGO_Customer_System.Model.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int RestaurantId { get; set; }
        public OrderLineDTO OrderLineDTO { get; set; }
        public List<OrderLineDTO> OrderLines { get; set; }
    }
}
