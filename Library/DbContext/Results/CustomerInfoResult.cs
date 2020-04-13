using System;

namespace Library.DbContext.Results
{
    public class CustomerInfoResult
    {
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public decimal CustomerBalanceAvalible { get; set; }
    }
}
