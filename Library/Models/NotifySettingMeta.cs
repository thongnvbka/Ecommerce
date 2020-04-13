using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Library.Settings;

namespace Library.Models
{
    public class NotifySettingMeta
    {
        [Required]
        public byte OfficeType { get; set; }

        [Required]
        public bool IsFollow { get; set; }

        [Required]
        public List<UserNotify> Users { get; set; }
    }
}