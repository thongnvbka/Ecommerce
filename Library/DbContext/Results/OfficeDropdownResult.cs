namespace Library.DbContext.Results
{
    public class OfficeDropdownResult
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public string IdPath { get; set; } // IdPath (length: 400)
        public string NamePath { get; set; } // NamePath (length: 2000)
    }
}
