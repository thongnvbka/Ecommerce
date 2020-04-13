using System.Collections.Generic;

namespace Common.Items
{
    public class DropdownItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public static List<DropdownItem> GetDropdownPage()
        {
            List<DropdownItem> list = new List<DropdownItem>();
            list.Add(new DropdownItem() { Text = "10", Value = "10" });
            list.Add(new DropdownItem() { Text = "25", Value = "25" });
            list.Add(new DropdownItem() { Text = "50", Value = "50" });
            list.Add(new DropdownItem() { Text = "100", Value = "100" });
            return list;
        }
        public static List<DropdownItem> GetCountry()
        {
            List<DropdownItem> list = new List<DropdownItem>();
            list.Add(new DropdownItem() { Text = "Vietnam", Value = "VN" });
            list.Add(new DropdownItem() { Text = "Lao", Value = "LO" });
            list.Add(new DropdownItem() { Text = "Thai", Value = "TH" });
            list.Add(new DropdownItem() { Text = "China (Simplified)", Value = "zh-CN" });
            return list;
        }
    }
}
