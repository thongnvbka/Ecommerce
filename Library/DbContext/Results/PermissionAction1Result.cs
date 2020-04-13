namespace Library.DbContext.Results
{
    public class PermissionAction1Result
    {
        public byte AppId { get; set; }
        public short ModuleId { get; set; }
        public short PageId { get; set; }
        public short? GroupPermisionId { get; set; }
        public string AppName { get; set; }
        public string ModuleName { get; set; }
        public string PageName { get; set; }
        public string GroupPermisionName { get; set; }
    }
}