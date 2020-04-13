using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class PageMeta
    {
        public short Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [Required]
        public int AppId { get; set; }

        public int OrderNo { get; set; }

        [Required]
        public string Url { get; set; }

        public string Icon { get; set; }

        public bool ShowInMenu { get; set; }

        [Required]
        public List<RoleActionMeta> RoleActions { get; set; }
    }

    public class RoleActionMeta
    {
        [Required]
        public byte Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Name { get; set; }

        [Required]
        public bool Checked { get; set; }
    }
}
