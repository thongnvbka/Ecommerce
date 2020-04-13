using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Items;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using AutoMapper;
using System;
using Common.Emums;
using Cms.Attributes;

namespace Cms.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        // GET: Product
        [LogTracker(EnumAction.View, EnumPage.Product)]
        public async Task<ActionResult> Index(ModelView<ProductResult, Library.ViewModels.ProductViewModel> model)
        {
            ViewBag.CategoryTree = GetCategoryParent();

            long totalRecord;
            if (model.SearchInfo == null)
            {
                model.SearchInfo = new Library.ViewModels.ProductViewModel();
            }

            var number = model.SearchInfo.Number == null ? 0 : model.SearchInfo.Number == "" ? 0 : int.Parse(model.SearchInfo.Number);

            model.Items = await UnitOfWork.ProductRepo.GetListAsync(
                out totalRecord,
                x => (model.SearchInfo.Keyword == null || (x.Name.Contains(model.SearchInfo.Keyword) || x.Properties.Contains(model.SearchInfo.Keyword) || x.Link.Contains(model.SearchInfo.Keyword)))
                    && (number == 0 || x.Quantity == number) && (model.SearchInfo.CategoryId == -1 || x.CategoryId == model.SearchInfo.CategoryId),
                x => x.OrderByDescending(y => y.Id),
                model.PageInfo.CurrentPage,
                model.PageInfo.PageSize
            );

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Url = Url.Action("Index", "Product");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }

            return View(model);
        }

        public string GetCategoryParent()
        {
            string str = "";

            var list = UnitOfWork.DbContext.Categories.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
            foreach (var item in list)
            {
                if (item.ParentId == 0)
                {
                    str += "{ id: " + item.Id + ", text: '" + item.Name + "', parent: '#'},";
                }
                else
                {
                    str += "{ id: " + item.Id + ", text: '" + item.Name + "', parent: '" + item.ParentId + "'},";
                }
            }
            return "[" + str + "]";
        }
    }
}