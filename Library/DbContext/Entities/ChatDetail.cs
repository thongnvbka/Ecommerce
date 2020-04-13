using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DbContext.Entities
{
    public class ChatDetail
    {
       
        public long Id { get; set; } // Id (Primary key)

       
        public string Description { get; set; } // Description (length: 500)

       
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Mã nhân viên
        ///</summary>
        public int? UserId { get; set; } // UserId
        public DateTime? CreateDate { get; set; } // CreateDate
        public bool? IsRead { get; set; } // IsRead
        public string CustomerName { get; set; } // CustomerName (length: 50)
        public string UserName { get; set; } // UserName (length: 50)
       
        public byte? OrderType { get; set; } // OrderType
       
        public byte? CommentType { get; set; } //CommentType
       
        public string UserOffice { get; set; }
      
        public int? GroupId { get; set; }
        public string AvatarCustomer { get; set; }
        public string AvatarUser { get; set; } 

    }
}
