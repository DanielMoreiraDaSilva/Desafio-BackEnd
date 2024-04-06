using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class DeliveryOrder
    {
        [Required(ErrorMessage = "Id is required.")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "DateCreate is required.")]
        public DateTime DateCreate { get; set; }
        [Required(ErrorMessage = "CostDelivery is required.")]
        public double CostDelivery { get; set; }
        [Required(ErrorMessage = "IdStatusDeliveryOrder is required.")]
        public Guid IdStatusDeliveryOrder { get; set; }
    }
}
