using System.Data;

namespace Lambda.Models
{
    public class ControlConnection
    {
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
    }
}
