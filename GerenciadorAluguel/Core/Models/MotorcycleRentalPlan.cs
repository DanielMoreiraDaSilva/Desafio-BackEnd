using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class MotorcycleRentalPlan
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdMotorcycle { get; set; }
        public Guid IdRentalPlan { get; set; }
        [JsonIgnore]
        public DateTime RentalStartDate { get; set; }
        [JsonIgnore]
        public DateTime RentalEndDate { get; set; }
        [Required(ErrorMessage = "ExpectedReturnDate is required.")]
        public DateTime ExpectedReturnDate { get; set; }
    }
}
