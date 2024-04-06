namespace Core.Models
{
    public class UserValidateReturn
    {
        public UserValidateReturn()
        {
            Errors = new();
        }
        public List<string> Errors { get; set; }
    }
}
