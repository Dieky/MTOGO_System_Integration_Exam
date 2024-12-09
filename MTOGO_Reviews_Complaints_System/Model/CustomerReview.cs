namespace MTOGO_Reviews_Complaints_System.Model
{
    public class CustomerReview
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
    }
}
