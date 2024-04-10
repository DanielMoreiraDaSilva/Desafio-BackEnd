using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    [ExcludeFromCodeCoverage]
    public class TypeCNH
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Valid { get; set; }
    }
}