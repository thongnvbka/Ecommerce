using Common.Items;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace Library.DbContext.Repositories
{
    public class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(ProjectXContext context) : base(context)
        {
        }

        public List<DropdownItem> GetListDropdown(bool isDelete)
        {
            var result = new List<DropdownItem>();
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "sp_Category_SelectForDropdown";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("isDelete", isDelete));
                try
                {
                    context.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();
                    result =
                        ((IObjectContextAdapter)context).ObjectContext.Translate<DropdownItem>(reader).ToList();
                    reader.Close();
                    return result;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }
    }
}
