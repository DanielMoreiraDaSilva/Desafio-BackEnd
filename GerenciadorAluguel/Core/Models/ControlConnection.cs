using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Core.Models
{
    [ExcludeFromCodeCoverage]
    public class ControlConnection
    {
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
    }
}
