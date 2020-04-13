using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class GroupPermissionMeta
    {
        public short Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }
    }

    public class PermissionMeta
    {
        public int GroupPermissionId { get; set; }
        public int PageId { get; set; }
        public List<RoleActionMeta> Actions { get; set; }
    }

    public class UpdatePermissionMeta
    {
        public int PermissionId { get; set; }
        public int PageId { get; set; }
        public byte ActionId { get; set; }
        public bool Checked { get; set; }
    }
}
