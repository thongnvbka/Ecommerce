using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DbContext.Entities
{
    public partial class TicketComplain
    {
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 30)
        public byte TypeOrder { get; set; } 
        public int TypeService { get; set; }
        public string TypeServiceName { get; set; }
        public int? TypeServiceClose { get; set; }
        public string TypeServiceCloseName { get; set; }
        public string ImagePath1 { get; set; } // ImagePath1 (length: 255)
        public string ImagePath2 { get; set; } // ImagePath2 (length: 255)
        public string ImagePath3 { get; set; } // ImagePath3 (length: 255)
        public string ImagePath4 { get; set; } // ImagePath4 (length: 255)
        public string ImagePath5 { get; set; } // ImagePath5 (length: 255)
        public string ImagePath6 { get; set; } // ImagePath6 (length: 255)
        public string Content { get; set; } // Content (length: 2000)
        public int OrderId { get; set; } // OrderId
        public string OrderCode { get; set; } // OrderCode (length: 30)
        public byte? OrderType { get; set; } // OrderType
        public int CustomerId { get; set; } // CustomerId
        public string CustomerName { get; set; } // CustomerName (length: 255)
        public DateTime? CreateDate { get; set; } // CreateDate
        public DateTime? LastUpdateDate { get; set; } // LastUpdateDate
        public int? SystemId { get; set; } // SystemId
        public string SystemName { get; set; } // SystemName (length: 100)
        public byte? Status { get; set; } // Status
        public string LastReply { get; set; } // LastReply (length: 2000)
        public decimal? BigMoney { get; set; } // BigMoney
        public bool IsDelete { get; set; } // IsDelete
        public decimal? RequestMoney { get; set; } // RequestMoney
        public string UserName { get; set; } // UserName (length: 255)
        public int? UserId{ get; set; } // UserId
        public string UserClaimName { get; set; } // UserClaimName (length: 255)
        public DateTime? ReceiveDate { get; set; } // LastUpdateDate

        public List<string> UserSupport { get; set; }
        public int UserSupportNo { get; set; }
        public int CountClaimForRefund { get; set; } // CountClaimForRefund
        public int? StatusClaimForRefund { get; set; } // StatusClaimForRefund
        public string ContentInternal { get; set; }
        public decimal? RealTotalRefund { get; set; } // BigMoney
        public string ContentInternalOrder { get; set; } // ContentInternalOrder

        public long LevelId { get; set; }
        public string LevelName { get; set; }

        public TicketComplain()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
