using MTOGO_Customer_System.Model.DTO;

namespace MTOGO_Customer_System.Model.Interfaces
{
    public interface ICustomerDBActions
    {
        public bool InsertCustomer(Customer customer, string catalog = "MTOGO");
        public Customer GetCustomerById(int id, string catalog = "MTOGO");
        public bool AuthenticateCustomer(string email, string enteredPassword, string catalog = "MTOGO");
        public Customer LoginCustomer(string email, string password, string catalog = "MTOGO");
        public bool UpdateCustomer(Customer customer, string catalog = "MTOGO");
        public int PlaceOrder(int customerId, List<OrderLine> orderLines, string catalog = "MTOGO");
        public void AddOrderLine(int orderId, OrderLine orderLine, string catalog = "MTOGO");
        public void UpdateOrderPrice(int orderId, string catalog = "MTOGO");
        public List<OrderDTO> GetOrdersByCustomerId(int customerId, string catalog = "MTOGO");
        public List<OrderDTO> RemoveRedundantInfo(List<OrderDTO> orders);
        public OrderDTO GetOrderById(int orderId, string catalog = "MTOGO");


    }
}
