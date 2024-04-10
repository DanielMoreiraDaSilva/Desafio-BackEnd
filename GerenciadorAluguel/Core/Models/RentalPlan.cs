using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    [ExcludeFromCodeCoverage]
    public class RentalPlan
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int DurationInDays { get; set; }
        public double DailyCost { get; set; }
        public double FactorCalculateCost { get; set; }
        public double CostExtraDays { get; set; }
    }
}
