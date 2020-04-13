using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class ProfitDay
    {
        public string Created { get; set; } 
        public int TotalOrder { get; set; }
        public decimal? TotalMoney { get; set; }
        public decimal? TotalBargain { get; set; }
        public decimal? Diminishe { get; set; }
        public decimal? Increase { get; set; }
        public decimal? Balance { get; set; }

        public ProfitDay()
        {
            Diminishe = 0;
            Increase = 0;
            Balance = 0;
        }
    }


}
