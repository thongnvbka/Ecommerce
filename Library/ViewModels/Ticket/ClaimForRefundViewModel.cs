using Library.DbContext.Entities;
using System.Collections.Generic;

namespace Library.ViewModels.Ticket
{
    public class ClaimForRefundViewModel
    {
        public ClaimForRefund ClaimForRefund { get; set; }
        public List<ClaimForRefundDetail> LstClaimForRefundDetails { get; set; }
    }
}