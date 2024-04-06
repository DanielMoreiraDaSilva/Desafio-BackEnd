namespace Core.Models
{
    public class ValidateReturn
    {
        public ValidateReturn()
        {
            Errors = new();
        }
        public List<string> Errors { get; set; }
    }
}
