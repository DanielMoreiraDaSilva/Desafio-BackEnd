using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    [ExcludeFromCodeCoverage]
    public class StatusDeliveryOrder
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }
}
