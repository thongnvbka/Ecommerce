namespace Library.DbContext.Results
{
    public class UserGetShortInfoByFullNameResults
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int TitleId { get; set; }
        public string TitleName { get; set; }
        public string OfficeName { get; set; }
        public string Image { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}