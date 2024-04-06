using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Motorcycle
    {
        [Required(ErrorMessage = "Id is required.")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Year is required.")]
        public int Year { get; set; }
        [Required(ErrorMessage = "Model is required.")]
        public string Model { get; set; }
        [Required(ErrorMessage = "Plate is required.")]
        public string Plate { get; set; }
    }
}
