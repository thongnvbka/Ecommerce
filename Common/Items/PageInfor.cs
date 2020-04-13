using System;
using System.Text.RegularExpressions;

namespace Common.Items
{
    public class PageInfor
    {
        public PageInfor()
        {
            CurrentPage = 1;
            TotalRecord = 0;
            PageSize = 20;
            PagePerSegment = 6;
            Url = "";
            Name = "records";
        }

        public string Name { get; set; }
        public int CurrentPage { get; set; }
        public int TotalRecord { get; set; }
        public int TotalPage => (int) Math.Ceiling(TotalRecord/(double) PageSize);
        public int CurrentSement => (int) Math.Ceiling(CurrentPage/(double) PagePerSegment);
        public int PagePerSegment { get; set; }
        public int TotalSegment => (int) Math.Ceiling(TotalPage/(double) PagePerSegment);
        public int MinPageInSegment => (CurrentSement - 1)*PagePerSegment + 1;
        public int MaxPageInSegment => MinPageInSegment + PagePerSegment - 1;
        public int PageSize { get; set; }
        public string Url { get; set; }
    }
}
