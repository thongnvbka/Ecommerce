namespace Library.Models
{
    public class AssignToShipper
    {
        public int DeliveryId { get; set; }
        public int? UserId { get; set; }
        public int? TitleId { get; set; }
        public int? OfficeId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}
