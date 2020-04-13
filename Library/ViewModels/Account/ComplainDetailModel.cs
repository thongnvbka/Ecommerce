using Library.DbContext.Entities;
using Library.ViewModels.Complains;
using System.Collections.Generic;

namespace Library.ViewModels.Account
{
    public class ComplainDetailModel
    {
        public Complain ComplainItem { get; set; }
        public List<ComplainUserComment> ListComments { get; set; }
    }
}
