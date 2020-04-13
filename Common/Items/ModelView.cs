using System.Collections.Generic;

namespace Common.Items
{
    public class ModelView<TEntity, TSearch>
    {
        public ModelView()
        {
            PageInfo = new PageInfor();
        }

        public TSearch SearchInfo { get; set; }
        public PageInfor PageInfo { get; set; }
        public List<TEntity> Items { get; set; }
    }
}
