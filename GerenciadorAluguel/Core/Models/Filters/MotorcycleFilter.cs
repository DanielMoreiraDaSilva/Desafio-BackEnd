using System.Diagnostics.CodeAnalysis;

namespace Core.Models.Filters
{
    [ExcludeFromCodeCoverage]
    public class MotorcycleFilter
    {
        public string Plate { get; set; }
        public bool? Disponible { get; set; }
    }
}
