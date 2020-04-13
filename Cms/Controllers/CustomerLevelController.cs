using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Items;
using Library.DbContext.Entities;
using System;

namespace Cms.Controllers
{
    public class CustomerLevelController : BaseController
    {
        // GET: CustomerLevel
        public async Task<ActionResult> Index(ModelView<CustomerLevel, string> model)
        {
            long totalRecord;
            model.PageInfo.PageSize = 100;
            model.Items = await UnitOfWork.CustomerLevelRepo.FindAsync(out totalRecord, null, x => x.OrderByDescending(y => y.Name),
                model.PageInfo.CurrentPage, model.PageInfo.PageSize);

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Url = Url.Action("Index", "CustomerLevel");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }
            return View(model);
        }
        public ActionResult Add()
        {
            var model = new CustomerLevel() { Id = 0 };
            return View(model);
        }
        public async Task<ActionResult> Edit(int id)
        {
            var item = await UnitOfWork.CustomerLevelRepo.SingleOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return HttpNotFound($"There is no level with ID being {id}");
            return View("Add", item);
        }
        [HttpPost]
        public async Task<ActionResult> AddItem(CustomerLevel item)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
                return RedirectToAction("Add", item);
            if (await UnitOfWork.CustomerLevelRepo.AnyAsync(x => x.Id != item.Id && x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Name", $"Name of customer level \"{item.Name }\" already exists");
                return RedirectToAction("Add", item);
            }
            var result = 0;
            if (item.Id == 0)
            {
                item.CreateDate = DateTime.Now;
                item.UpdateDate = DateTime.Now;
                item.UserName = UserState.UserName;
                UnitOfWork.CustomerLevelRepo.Add(item);
                result = await UnitOfWork.CustomerLevelRepo.SaveAsync();
                if (result > 0)
                    TempData["Msg"] = $"Successfully added VIP level <b>{item.Name}</b>";
            }
            else
            {
                var tmpItem = await UnitOfWork.CustomerLevelRepo.SingleOrDefaultAsync(x => x.Id == item.Id);

                if (tmpItem == null)
                {
                    ModelState.AddModelError("Name", $"Name of VIP level \"{item.Name }\" does not exist");
                    return RedirectToAction("Add", item);
                }
                tmpItem.Name = item.Name;
                tmpItem.Description = item.Description;
                tmpItem.Status = item.Status;
                //tmpItem.TotalMoney = item.TotalMoney;
                UnitOfWork.CustomerLevelRepo.Update(tmpItem);
                result = await UnitOfWork.CustomerLevelRepo.SaveAsync();
                if (result > 0)
                    TempData["Msg"] = $"Edit VIP level successfully <b>{item.Name}</b>";
            }
            if (result <= 0)
            {
                return RedirectToAction("Add", item);
            }

            return RedirectToAction("Add");
        }
        public int CheckExistsName(string name, int id)
        {
            int result = 0;
            if (UnitOfWork.CustomerLevelRepo.Any(x => x.Id != id && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                result = 1;
            }
            return result;
        }
    }
}