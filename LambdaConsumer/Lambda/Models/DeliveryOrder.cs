namespace Lambda.Models
{
    public class DeliveryOrder
    {
        public Guid Id { get; set; }
        public DateTime DateDeliveryOrderCreate { get; set; }
        public double CostDelivery { get; set; }
        public Guid IdStatusDeliveryOrder { get; set; }
        public DateTime DateNotified { get; set; }
    }
}
