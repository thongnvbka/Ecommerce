using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Items
{
    public class PageItem
    {
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Url { get; set; }
    }
}
