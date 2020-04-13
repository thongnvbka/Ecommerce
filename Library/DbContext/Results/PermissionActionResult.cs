namespace Library.DbContext.Results
{
    public class PermissionActionResult
    {
        public int Id { get; set; }
        public byte AppId { get; set; }
        public short ModuleId { get; set; }
        public short ModuleLevel { get; set; }
        public short PageId { get; set; }
        public short? GroupPermisionId { get; set; }
        public byte RoleActionId { get; set; }
        public string AppName { get; set; }
        public string ModuleName { get; set; }
        public string ModuleIdPath { get; set; }
        public string ModuleNamePath { get; set; }
        public string ModuleIcon { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public string PageIcon { get; set; }
        public int PageOrderNo { get; set; }
        public string GroupPermisionName { get; set; }
        public string RoleName { get; set; }
        public bool Checked { get; set; }
        public bool IsShowMenu { get; set; }
    }
}
