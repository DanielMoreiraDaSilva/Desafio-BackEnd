using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    [ExcludeFromCodeCoverage]
    public class ValidateReturn
    {
        public ValidateReturn()
        {
            Errors = new();
        }
        public List<string> Errors { get; set; }
    }
}
