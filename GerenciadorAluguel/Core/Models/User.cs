namespace Core.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public DateTime BirthDay { get; set; }
        public string CNHNumber { get; set; }
        public string[] InsertCNHType { get; set; }
        public string CNHImageBase64 { get; set; }
    }
}
