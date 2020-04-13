using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Items;
using Library.DbContext.Entities;
using System;
using AutoMapper;
using Newtonsoft.Json;
using Library.Models;
using Library.ViewModels;
using System.Collections.Generic;
using Common.Helper;
using Common.Emums;
using Cms.Attributes;
using Library.ViewModels.Items;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.ComponentModel;
using System.Runtime.ExceptionServices;

namespace Cms.Controllers
{
    [Authorize]
    public class FinanceController : BaseController
    {
        // GET: Finance
        [LogTracker(EnumAction.View, EnumPage.Fund, EnumPage.AccountingRegulations, EnumPage.AccountingWallet)]
        public ActionResult Index()
        {
            return View();
        }

        #region [Quỹ]
        [LogTracker(EnumAction.View, EnumPage.Fund)]
        public async Task<ActionResult> Fund(ModelView<FinanceFund, FinanceFundViewModel> model)
        {
            var kt = UnitOfWork.FinaceFundRepo.List_Position();
            ViewBag.financeFundJsTree = FinanceFundJsTree();
            long totalRecord;

            if (model.SearchInfo == null)
            {
                model.SearchInfo = new FinanceFundViewModel();
            }
            model.Items = await UnitOfWork.FinaceFundRepo.FindAsync(
               out totalRecord,
               x => !x.IsDelete && (x.IdPath == model.SearchInfo.Path || x.IdPath.StartsWith(model.SearchInfo.Path + ".")),
               x => x.OrderBy(y => y.ParentId),
               model.PageInfo.CurrentPage,
               model.PageInfo.PageSize
           );

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Name = "Fund";
            model.PageInfo.Url = Url.Action("Fund", "Finance");

            if (Request.IsAjaxRequest())
            {
                return PartialView("FundList", model);
            }



            return View(model);
        }

        [LogTracker(EnumAction.View, EnumPage.Fund)]
        [CheckPermission(EnumAction.Add, EnumPage.Fund)]
        public ActionResult AddFund()
        {
            var list = UnitOfWork.DbContext.Users.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" }).ToList();
            ViewBag.financeFundJsTree = FinanceFundJsTree();
            ViewBag.user = list.ToList();
            Session["UserPermistionFinance"] = new List<UserPermistionItem>();
            var tmpList = new List<UserPermistionItem>();
            ViewBag.ListPermistion = tmpList;

            //Lấy danh sách các Type tiền
            var listCurrency = new List<SelectListItem>();
            foreach (Currency currency in Enum.GetValues(typeof(Currency)))
            {
                if (currency >= 0)
                {
                    listCurrency.Add(new SelectListItem { Text = currency.ToString(), Value = currency.ToString() });
                }
            }

            ViewBag.ListCurrency = listCurrency;
            return View();
        }

        [LogTracker(EnumAction.View, EnumPage.Fund)]
        [CheckPermission(EnumAction.View, EnumPage.Fund)]
        public async Task<ActionResult> EditFund(int id)
        {
            var list = UnitOfWork.DbContext.Users.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" }).ToList();
            ViewBag.user = list.ToList();

            ViewBag.financeFundJsTree = FinanceFundJsTree();
            var trea = await UnitOfWork.FinaceFundRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            Session["UserPermistionFinance"] = new List<UserPermistionItem>();
            var tmpList = new List<UserPermistionItem>();

            if (trea != null && trea.Maxlength != null)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                tmpList = jss.Deserialize<List<UserPermistionItem>>(trea.Maxlength);
                Session["UserPermistionFinance"] = tmpList;
            }

            ViewBag.ListPermistion = tmpList;
            if (trea == null)
                return HttpNotFound($"No fund with Id is {id}");

            //Lấy danh sách các Type tiền
            var listCurrency = new List<SelectListItem>();
            foreach (Currency currency in Enum.GetValues(typeof(Currency)))
            {
                if (currency >= 0)
                {
                    listCurrency.Add(new SelectListItem { Text = currency.ToString(), Value = currency.ToString() });
                }
            }

            ViewBag.ListCurrency = listCurrency;

            return View(Mapper.Map<FinanceFundData>(trea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogTracker(EnumAction.View, EnumPage.Fund)]
        [CheckPermission(EnumAction.Add, EnumPage.Fund)]
        public async Task<ActionResult> AddFund(FinanceFundData model)
        {
            var list = UnitOfWork.DbContext.Users.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" }).ToList();
            ViewBag.user = list.ToList();
            ViewBag.financeFundJsTree = FinanceFundJsTree();
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                ViewBag.user = list.ToList();
                return View();
            }


            // Tên quỹ đã tồn tại
            if (await UnitOfWork.FinaceFundRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete))
            {
                ModelState.AddModelError("Name", $"Fund name \"{model.Name }\" already exists");
                ViewBag.user = list.ToList();
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.Balance < 0)
                    {
                        model.Balance = 0;
                    }
                    var trea = Mapper.Map<FinanceFund>(model);
                    var user = UnitOfWork.UserRepo.SingleOrDefault(u => u.Id == model.UserId);
                    trea.UserPhone = user.Phone;

                    //Quỹ cha
                    var parent = UnitOfWork.FinaceFundRepo.SingleOrDefault(p => p.Id == trea.ParentId);
                    if (parent != null)
                    {
                        parent.IsParent = true;
                    }

                    await UnitOfWork.FinaceFundRepo.SaveAsync();


                    if (trea.ParentId == 0)
                    {
                        trea.IdPath = trea.Id.ToString();
                        trea.NamePath = trea.Name;
                        trea.ParentName = string.Empty;
                    }
                    else
                    {
                        trea.IdPath = parent.IdPath;
                        trea.NamePath = parent.NamePath + "/" + trea.Name;
                        trea.ParentName = parent.Name;

                    }
                    if (user != null)
                    {
                        trea.UserEmail = user.Email;
                        trea.UserFullName = user.FullName;
                        trea.UserPhone = user.Phone;
                        trea.NameBank = user.NameBank;
                        trea.BankAccountNumber = user.BankAccountNumber;
                        trea.Department = user.Department;
                    }

                    UnitOfWork.FinaceFundRepo.Add(trea);
                    var rs = await UnitOfWork.FinaceFundRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        ViewBag.user = list.ToList();
                        return View();
                    }
                    if (trea.ParentId > 0)
                    {
                        trea.IdPath = parent.IdPath + "." + trea.Id;
                    }
                    else
                    {
                        trea.IdPath = trea.ParentId + "." + trea.Id.ToString();
                    }
                    //Them quyen
                    var tmpList = new List<UserPermistionItem>();
                    if (Session["UserPermistionFinance"] != null)
                    {
                        tmpList = (List<UserPermistionItem>)Session["UserPermistionFinance"];
                    }
                    if (tmpList.Any())
                    {
                        trea.Maxlength = JsonConvert.SerializeObject(tmpList);
                    }
                    await UnitOfWork.FinaceFundRepo.SaveAsync();
                    TempData["Msg"] = $"Successfully added funds \"<b>{trea.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("AddFund");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogTracker(EnumAction.View, EnumPage.Fund)]
        [CheckPermission(EnumAction.Update, EnumPage.Fund)]
        public async Task<ActionResult> EditFund(FinanceFundData model)
        {
            ViewBag.financeFundJsTree = FinanceFundJsTree();

            if (!ModelState.IsValid)
                return View(model);

            var trea = await UnitOfWork.FinaceFundRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (trea == null)
            {
                ModelState.AddModelError("NotExist", @"The Fund does not exist or has been deleted");
                return View(model);
            }

            FinanceFund treaParent = null;

            // Có thay đổi đơn vị cha
            if (model.ParentId != trea.ParentId)
            {
                // Kiểm tra đơn vị Cha có tồn tại hay không
                treaParent = await UnitOfWork.FinaceFundRepo.SingleOrDefaultAsync(x => x.Id == model.ParentId && !x.IsDelete);
                if (treaParent == null)
                {
                    ModelState.AddModelError("ParentId",
                        $"Father fund \"{model.ParentName}\" does not exist or has been deleted");
                    return View(model);
                }
                model.ParentName = treaParent.Name;
            }

            // Tên quỹ đã tồn tại
            if (
                await UnitOfWork.FinaceFundRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentId == model.ParentId && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Fund name\"{model.Name}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (model.Balance < 0)
                    {
                        model.Balance = 0;
                    }
                    var user = UnitOfWork.UserRepo.SingleOrDefault(s => s.Id == model.UserId);
                    var oldParentId = trea.ParentId;
                    var oldIdPath = trea.IdPath;
                    var oldNamePath = trea.NamePath;

                    trea = Mapper.Map(model, trea);
                    trea.UserPhone = user.Phone;

                    //Cập nhật thông tin người quản lý
                    if (user != null)
                    {
                        trea.UserEmail = user.Email;
                        trea.UserFullName = user.FullName;
                        trea.UserPhone = user.Phone;
                        trea.NameBank = user.NameBank;
                        trea.BankAccountNumber = user.BankAccountNumber;
                        trea.Department = user.Department;
                    }

                    if (treaParent == null)
                    {
                        if (trea.ParentId == 0)
                        {
                            trea.IdPath = trea.ParentId + "." + trea.Id;
                            trea.NamePath = trea.Name;
                            trea.ParentName = string.Empty;
                        }
                        else
                        {
                            //trea.IdPath = treaParent.IdPath + "." + trea.Id;
                            //trea.NamePath = treaParent.NamePath + "/" + trea.Name;
                            //trea.ParentName = treaParent.Name;
                        }

                    }
                    else
                    {

                        trea.IdPath = treaParent.IdPath + "." + trea.Id;
                        trea.NamePath = treaParent.NamePath + "/" + trea.Name;
                        trea.ParentName = treaParent.Name;

                        // cập nhật cho quỹ cha
                        treaParent.IsParent = true;

                        await UnitOfWork.FinaceFundRepo.SaveAsync();

                        //check quỹ cha cũ
                        var parent = await UnitOfWork.FinaceFundRepo.FirstOrDefaultAsync(x => x.Id == oldParentId);
                        if (parent != null)
                        {
                            var countChil = await UnitOfWork.FinaceFundRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                            parent.IsParent = countChil != 0;
                            await UnitOfWork.FinaceFundRepo.SaveAsync();
                        }
                    }
                    var rs = await UnitOfWork.FinaceFundRepo.SaveAsync();

                    if (rs < 0)
                    {
                        var list = UnitOfWork.DbContext.Users.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" }).ToList();
                        ViewBag.user = list.ToList();
                        return View(model);
                    }

                    // Cập nhật lại IdPath và NamePath cho đơn vị

                    if (model.ParentId != oldParentId)
                    {
                        if (treaParent == null)
                        {
                            trea.IdPath = trea.Id.ToString();
                        }
                        else
                        {
                            trea.IdPath = $"{treaParent.IdPath}.{trea.Id}";
                            trea.NamePath = $"{treaParent.NamePath}/{trea.Name}";
                        }

                        // Cập nhật lại IdPath của tất cả các đơn vị bên dưới
                        var listSubTrea = await UnitOfWork.FinaceFundRepo.FindAsync(
                                    x => !x.IsDelete && x.IdPath.StartsWith(oldIdPath + "."));

                        if (listSubTrea != null)
                        {
                            listSubTrea.ForEach(o =>
                            {
                                o.IdPath = $"{trea.IdPath}{o.IdPath.Substring(oldIdPath.Length, o.IdPath.Length - oldIdPath.Length)}";
                                o.NamePath = $"{trea.NamePath}{o.NamePath.Substring(oldNamePath.Length, o.NamePath.Length - oldNamePath.Length)}";
                            });
                        }

                    }
                    //Them quyen
                    var tmpList = new List<UserPermistionItem>();
                    if (Session["UserPermistionFinance"] != null)
                    {
                        tmpList = (List<UserPermistionItem>)Session["UserPermistionFinance"];
                    }
                    if (tmpList.Any())
                    {
                        trea.Maxlength = JsonConvert.SerializeObject(tmpList);
                    }
                    await UnitOfWork.TreasureRepo.SaveAsync();

                    TempData["Msg"] = $"Successful update fund \"<b>{trea.Name}</b>\"";

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("Fund");
        }

        public int FundCheckExistsName(string name, int id)
        {
            int result = 0;
            //if (UnitOfWork.FinaceFundRepo.Any(x => x.FundId != id && x.FundName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            //{
            //    result = 1;
            //}
            return result;
        }

        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.Fund)]
        [CheckPermission(EnumAction.Delete, EnumPage.Fund)]
        public async Task<ActionResult> FundDelete(int id)
        {
            var rs = 1;
            var trea = await UnitOfWork.FinaceFundRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    trea.IsDelete = true;
                    UnitOfWork.FinaceFundRepo.Update(trea);

                    rs = await UnitOfWork.TreasureRepo.SaveAsync();

                    //check quỹ cha cũ
                    var parent = await UnitOfWork.FinaceFundRepo.FirstOrDefaultAsync(x => x.Id == trea.ParentId);
                    if (parent != null)
                    {
                        var countChil = await UnitOfWork.FinaceFundRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                        parent.IsParent = countChil != 0;
                        await UnitOfWork.FinaceFundRepo.SaveAsync();
                    }
                    //Xóa quỹ con
                    if (trea.IsParent)
                    {
                        var listChil = await UnitOfWork.FinaceFundRepo.FindAsync(x => x.IdPath.Contains(trea.IdPath) && !x.IsDelete);
                        foreach (var item in listChil)
                        {
                            item.IsDelete = true;
                        }
                        await UnitOfWork.FinaceFundRepo.SaveAsync();
                    }

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult Account()
        {
            return View();
        }
        public ActionResult AccountAdd()
        {
            return View();
        }

        #region [Định khoản quỹ - AccountingRegulations] 
        [LogTracker(EnumAction.View, EnumPage.AccountingRegulations)]
        public async Task<ActionResult> AccountingRegulations(ModelView<Treasure, TreasureViewModel> model)
        {

            long totalRecord;

            if (model.SearchInfo == null)
            {
                model.SearchInfo = new TreasureViewModel();
            }
            model.Items = await UnitOfWork.TreasureRepo.FindAsync(
               out totalRecord,
               x => !x.IsDelete && (x.IdPath == model.SearchInfo.Path || x.IdPath.StartsWith(model.SearchInfo.Path + ".")),
               x => x.OrderBy(y => y.ParentId),
               model.PageInfo.CurrentPage,
               model.PageInfo.PageSize
           );

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Name = "Quỹ";
            model.PageInfo.Url = Url.Action("AccountingRegulations", "Finance");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }

            ViewBag.treasureJsTree = TreasureJsTree();

            return View(model);
        }

        [LogTracker(EnumAction.View, EnumPage.Fund, EnumPage.AccountingRegulations, EnumPage.AccountingWallet)]
        public ActionResult CreateTreasure()
        {
            ViewBag.treasureJsTree = TreasureActonJsTree();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> CreateTreasure(TreasureMeta model)
        {
            ViewBag.treasureJsTree = TreasureActonJsTree();
            ModelState.Remove("Id");
            ModelState.Remove("IsIdSystem");
            if (!ModelState.IsValid)
                return View();
            // IDD định khoản đã tồn tại
            if (await UnitOfWork.CustomerWalletRepo.AnyAsync(x => x.Idd == model.IDD && model.IDD != null && !x.IsDelete))
            {
                ModelState.AddModelError("IDD", $"IDD định khoản thu chi \"{model.IDD }\" already exists");
                return View();
            }
            // Tên quỹ đã tồn tại
            if (await UnitOfWork.TreasureRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete))
            {
                ModelState.AddModelError("Name", $"Tên định khoản thu chi \"{model.Name }\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var trea = Mapper.Map<Treasure>(model);
                    var obj = new Treasure();
                    if (trea.ParentId != 0)
                    {
                        obj = UnitOfWork.TreasureRepo.SingleOrDefault(x => x.Id == trea.ParentId);
                        if (obj != null)
                        {
                            obj.IsParent = true;
                            trea.IdPath = string.Empty;
                            trea.NamePath = obj.NamePath + "/" + trea.Name;
                            trea.ParentId = obj.Id;
                            trea.ParentName = obj.Name;
                            trea.Operator = obj.Operator;
                        }
                        else
                        {
                            TempData["Msg"] = $"does not exist hoặc đã xóa định khoản thu chi cha đã chọn";
                        }
                    }
                    else
                    {
                        trea.IdPath = string.Empty;
                        trea.NamePath = trea.Name;
                        trea.ParentId = 0;
                        trea.ParentName = "";
                        trea.Operator = false;
                    }

                    UnitOfWork.TreasureRepo.Add(trea);
                    var rs = await UnitOfWork.TreasureRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View();
                    }
                    //Cập nhật lại IdPath
                    if (trea.ParentId != 0)
                    {
                        trea.IdPath = obj.IdPath + "." + trea.Id;
                    }
                    else
                    {
                        trea.IdPath = "0." + trea.Id;
                    }

                    await UnitOfWork.TreasureRepo.SaveAsync();

                    TempData["Msg"] = $"Successfully added the revenue statement\"<b>{trea.Name}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("CreateTreasure");
        }

        public async Task<ActionResult> EditTreasure(int id)
        {
            ViewBag.treasureJsTree = TreasureActonJsTree();
            var trea = await UnitOfWork.TreasureRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return HttpNotFound($"Không có định khoản thu chi nào có Id là {id}");

            return View(Mapper.Map<TreasureMeta>(trea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTreasure(TreasureMeta model)
        {
            ViewBag.treasureJsTree = TreasureActonJsTree();

            ModelState.Remove("IsIdSystem");
            if (!ModelState.IsValid)
                return View(model);

            //1. Kiểm tra định khoản quỹ có tồn tại hay không
            var trea = await UnitOfWork.TreasureRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);
            if (trea == null)
            {
                ModelState.AddModelError("NotExist", "Định khoản does not exist or has been deleted");
                return View(model);
            }

            Treasure treaParent = null;

            // Có thay đổi đơn vị cha
            if (model.ParentId != trea.ParentId)
            {
                // Kiểm tra đơn vị Cha có tồn tại hay không
                treaParent =
                    await UnitOfWork.TreasureRepo.SingleOrDefaultAsync(x => x.Id == model.ParentId && !x.IsDelete);
                if (treaParent == null)
                {
                    ModelState.AddModelError("ParentId",
                        $"Định khoản cha \"{model.ParentName}\" does not exist or has been deleted");
                    return View(model);
                }
                model.ParentName = treaParent.Name;
            }
            else
            {
                model.ParentName = trea.ParentName;
            }

            if (trea.Idd != model.IDD)
            {
                var customerWalletDetail = await UnitOfWork.TreasureRepo.FindAsync(x => !x.IsDelete && x.Idd == model.IDD);
                if (customerWalletDetail.Count() > 0)
                {
                    ModelState.AddModelError("IDD", $"IDD định khoản thu chi \"{model.IDD }\" already exists");
                    return View();
                }
            }

            // Tên chuyên mục đã tồn tại
            if (
                await UnitOfWork.TreasureRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentId == model.ParentId && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Tên định khoản \"{model.Name}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldParentId = trea.ParentId;
                    var oldIdPath = trea.IdPath;
                    var oldNamePath = trea.NamePath;
                    trea = Mapper.Map(model, trea);

                    var rs = await UnitOfWork.TreasureRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View(model);
                    }

                    // Cập nhật lại IdPath và NamePath cho đơn vị
                    if (model.ParentId != oldParentId)
                    {
                        if (treaParent == null)
                        {
                            trea.IdPath = trea.Id.ToString();
                        }
                        else
                        {
                            trea.IdPath = $"{treaParent.IdPath}.{trea.Id}";
                            trea.NamePath = $"{treaParent.NamePath}/{trea.Name}";
                            treaParent.IsParent = true;

                            //check quỹ cha cũ
                            var parent = await UnitOfWork.TreasureRepo.FirstOrDefaultAsync(x => x.Id == oldParentId);
                            if (parent != null)
                            {
                                var countChil = await UnitOfWork.TreasureRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                                parent.IsParent = countChil != 0;
                                await UnitOfWork.TreasureRepo.SaveAsync();
                            }
                        }

                        // Cập nhật lại IdPath của tất cả các đơn vị bên dưới
                        var listSubTrea = await UnitOfWork.TreasureRepo.FindAsync(
                                    x => !x.IsDelete && x.IdPath.StartsWith(oldIdPath + "."));

                        if (listSubTrea != null)
                        {
                            listSubTrea.ForEach(o =>
                            {
                                o.IdPath = $"{trea.IdPath}{o.IdPath.Substring(oldIdPath.Length, o.IdPath.Length - oldIdPath.Length)}";
                                o.NamePath = $"{trea.NamePath}{o.NamePath.Substring(oldNamePath.Length, o.NamePath.Length - oldNamePath.Length)}";
                            });
                        }

                    }

                    await UnitOfWork.TreasureRepo.SaveAsync();

                    TempData["Msg"] = $"Successfully updated the statement \"<b>{trea.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("AccountingRegulations");
        }
        [HttpPost]
        public async Task<ActionResult> DeleteTreasure(int id)
        {
            var rs = 1;
            var trea = await UnitOfWork.TreasureRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    trea.IsDelete = true;
                    UnitOfWork.TreasureRepo.Update(trea);

                    rs = await UnitOfWork.TreasureRepo.SaveAsync();

                    //check định khoản quỹ cha cũ
                    var parent = await UnitOfWork.TreasureRepo.FirstOrDefaultAsync(x => x.Id == trea.ParentId);
                    if (parent != null)
                    {
                        var countChil =
                            await UnitOfWork.TreasureRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                        parent.IsParent = countChil != 0;
                        await UnitOfWork.TreasureRepo.SaveAsync();
                    }
                    //Xóa định khoản quỹ con
                    if (trea.IsParent)
                    {
                        var listChil = await UnitOfWork.TreasureRepo.FindAsync(x => x.IdPath.Contains(trea.IdPath) && !x.IsDelete);
                        foreach (var item in listChil)
                        {
                            item.IsDelete = true;
                        }
                        await UnitOfWork.TreasureRepo.SaveAsync();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        public string TreasureJsTree()
        {
            var list = UnitOfWork.DbContext.Treasures.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                idPath = o.IdPath,
                state = new { opened = "", selected = "" },
            }).ToList();
            list.Add(new { id = "0", text = "Management of revenue and expenditure accounts", parent = "#", idPath = "0", state = new { opened = "true", selected = "true" } });
            return JsonConvert.SerializeObject(list);
        }

        public string TreasureActonJsTree()
        {
            var list = UnitOfWork.DbContext.Treasures.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId == 0 ? "#" : o.ParentId.ToString(),
                idPath = o.IdPath
            }).ToList();
            return JsonConvert.SerializeObject(list);
        }

        public string FinanceFundJsTree()
        {
            var list = UnitOfWork.DbContext.FinanceFunds.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                idPath = o.IdPath,
            }).ToList();
            list.Add(new { id = "0", text = "Fund management", parent = "#", idPath = "0" });

            return JsonConvert.SerializeObject(list);
        }

        #endregion

        #region [Định khoản ví điện tử - AccountingWallet]
        [LogTracker(EnumAction.View, EnumPage.AccountingWallet)]
        public async Task<ActionResult> AccountingWallet(ModelView<CustomerWallet, CustomerWalletViewModel> model)
        {

            long totalRecord;

            if (model.SearchInfo == null)
            {
                model.SearchInfo = new CustomerWalletViewModel();
            }
            model.Items = await UnitOfWork.CustomerWalletRepo.FindAsync(
               out totalRecord,
               x => !x.IsDelete && (x.IdPath == model.SearchInfo.Path || x.IdPath.StartsWith(model.SearchInfo.Path + ".")),
               x => x.OrderBy(y => y.ParentId),
               model.PageInfo.CurrentPage,
               model.PageInfo.PageSize
           );

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Name = "Ví điện tử";
            model.PageInfo.Url = Url.Action("AccountingWallet", "Finance");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_WalletList", model);
            }

            ViewBag.walletJsTree = WalletJsTree();

            return View(model);
        }

        public ActionResult CreateWallet()
        {
            ViewBag.walletJsTree = WalletActonJsTree();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWallet(CustomerWallitMeta model)
        {
            ViewBag.walletJsTree = WalletActonJsTree();
            ModelState.Remove("Id");
            ModelState.Remove("IsIdSystem");
            if (!ModelState.IsValid)
                return View();

            // IDD định khoản đã tồn tại
            if (await UnitOfWork.CustomerWalletRepo.AnyAsync(x => x.Idd == model.IDD && model.IDD != null && !x.IsDelete))
            {
                ModelState.AddModelError("IDD", $"IDD định khoản thu chi \"{model.IDD }\" already exists");
                return View();
            }

            // Tên định khoản đã tồn tại
            if (await UnitOfWork.CustomerWalletRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete))
            {
                ModelState.AddModelError("Name", $"Tên định khoản thu chi \"{model.Name }\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var trea = Mapper.Map<CustomerWallet>(model);
                    var obj = new CustomerWallet();
                    if (trea.ParentId != 0)
                    {
                        obj = UnitOfWork.CustomerWalletRepo.SingleOrDefault(x => x.Id == trea.ParentId);
                        if (obj != null)
                        {
                            obj.IsParent = true;
                            trea.IdPath = string.Empty;
                            trea.NamePath = obj.NamePath + "/" + trea.Name;
                            trea.ParentId = obj.Id;
                            trea.ParentName = obj.Name;
                            trea.Operator = obj.Operator;
                        }
                        else
                        {
                            TempData["Msg"] = $"does not exist hoặc đã xóa định khoản thu-chi \"<b>{trea.Name}</b>\"";
                        }
                    }
                    else
                    {
                        trea.IdPath = string.Empty;
                        trea.NamePath = trea.Name;
                        trea.ParentId = 0;
                        trea.ParentName = "";
                        trea.Operator = false;
                    }

                    UnitOfWork.CustomerWalletRepo.Add(trea);
                    var rs = await UnitOfWork.CustomerWalletRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View();
                    }
                    //Cập nhật lại IdPath
                    if (trea.ParentId != 0)
                    {
                        trea.IdPath = obj.IdPath + "." + trea.Id;
                    }
                    else
                    {
                        trea.IdPath = "0." + trea.Id;
                    }
                    await UnitOfWork.CustomerWalletRepo.SaveAsync();

                    TempData["Msg"] = $"Successfully added the revenue statement \"<b>{trea.Name}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("CreateWallet");
        }

        public async Task<ActionResult> EditWallet(int id)
        {
            ViewBag.walletJsTree = WalletActonJsTree();
            var trea = await UnitOfWork.CustomerWalletRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return HttpNotFound($"Không có định khoản thu chi nào có Id là {id}");

            return View(Mapper.Map<CustomerWallitMeta>(trea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditWallet(CustomerWallitMeta model)
        {
            ViewBag.walletJsTree = WalletActonJsTree();
            ModelState.Remove("IsIdSystem");
            if (!ModelState.IsValid)
                return View(model);

            var trea = await UnitOfWork.CustomerWalletRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (trea == null)
            {
                ModelState.AddModelError("NotExist", "Định khoản does not exist or has been deleted");
                return View(model);
            }

            CustomerWallet treaParent = null;

            // Có thay đổi đơn vị cha
            if (model.ParentId != trea.ParentId)
            {
                // Kiểm tra đơn vị Cha có tồn tại hay không
                treaParent =
                    await UnitOfWork.CustomerWalletRepo.SingleOrDefaultAsync(x => x.Id == model.ParentId && !x.IsDelete);
                if (treaParent == null)
                {
                    ModelState.AddModelError("ParentId",
                        $"Regulations parents \"{model.ParentName}\" Does not exist or has been deleted");
                    return View(model);
                }
                model.ParentName = treaParent.Name;

            }
            else
            {
                model.ParentName = trea.ParentName;
            }
            // IDD định khoản đã tồn tại
            if (await UnitOfWork.CustomerWalletRepo.AnyAsync(x => x.Idd == model.IDD && model.IDD != null && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("IDD", $"IDD định khoản thu chi \"{model.IDD }\" already exists");
                return View();
            }
            // Tên chuyên mục đã tồn tại
            if (
                await UnitOfWork.CustomerWalletRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentId == model.ParentId && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Tên định khoản \"{model.Name}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldParentId = trea.ParentId;
                    var oldIdPath = trea.IdPath;
                    var oldNamePath = trea.NamePath;
                    trea = Mapper.Map(model, trea);

                    var rs = await UnitOfWork.CustomerWalletRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View(model);
                    }

                    // Cập nhật lại IdPath và NamePath cho đơn vị
                    if (model.ParentId != oldParentId)
                    {
                        if (treaParent == null)
                        {
                            trea.IdPath = trea.Id.ToString();
                        }
                        else
                        {
                            trea.IdPath = $"{treaParent.IdPath}.{trea.Id}";
                            trea.NamePath = $"{treaParent.NamePath}/{trea.Name}";
                            treaParent.IsParent = true;

                            //check quỹ cha cũ
                            var parent = await UnitOfWork.CustomerWalletRepo.FirstOrDefaultAsync(x => x.Id == oldParentId);
                            if (parent != null)
                            {
                                var countChil = await UnitOfWork.CustomerWalletRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                                parent.IsParent = countChil != 0;
                                await UnitOfWork.CustomerWalletRepo.SaveAsync();
                            }
                        }

                        // Cập nhật lại IdPath của tất cả các đơn vị bên dưới
                        var listSubTrea = await UnitOfWork.CustomerWalletRepo.FindAsync(
                                    x => !x.IsDelete && x.IdPath.StartsWith(oldIdPath + "."));

                        if (listSubTrea != null)
                        {
                            listSubTrea.ForEach(o =>
                            {
                                o.IdPath = $"{trea.IdPath}{o.IdPath.Substring(oldIdPath.Length, o.IdPath.Length - oldIdPath.Length)}";
                                o.NamePath = $"{trea.NamePath}{o.NamePath.Substring(oldNamePath.Length, o.NamePath.Length - oldNamePath.Length)}";
                            });
                        }

                    }

                    await UnitOfWork.CustomerWalletRepo.SaveAsync();

                    TempData["Msg"] = $"Successfully updated the statement \"<b>{trea.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("AccountingWallet");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteWallet(int id)
        {
            var rs = 1;
            var trea = await UnitOfWork.CustomerWalletRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    trea.IsDelete = true;
                    UnitOfWork.CustomerWalletRepo.Update(trea);

                    rs = await UnitOfWork.CustomerWalletRepo.SaveAsync();

                    //check định khoản ví cha cũ
                    var parent = await UnitOfWork.CustomerWalletRepo.FirstOrDefaultAsync(x => x.Id == trea.ParentId);
                    if (parent != null)
                    {
                        var countChil =
                            await UnitOfWork.CustomerWalletRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                        parent.IsParent = countChil != 0;
                        await UnitOfWork.CustomerWalletRepo.SaveAsync();
                    }

                    //Xóa định khoản ví con
                    if (trea.IsParent)
                    {
                        var listChil = await UnitOfWork.CustomerWalletRepo.FindAsync(x => x.IdPath.Contains(trea.IdPath) && !x.IsDelete);
                        foreach (var item in listChil)
                        {
                            item.IsDelete = true;
                        }
                        await UnitOfWork.CustomerWalletRepo.SaveAsync();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        public string WalletActonJsTree()
        {
            var list = UnitOfWork.DbContext.CustomerWallets.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId == 0 ? "#" : o.ParentId.ToString(),
                idPath = o.IdPath
            }).ToList();
            return JsonConvert.SerializeObject(list);
        }

        public string WalletJsTree()
        {
            var list = UnitOfWork.DbContext.CustomerWallets.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                idPath = o.IdPath,
                state = new { opened = "", selected = "" },
            }).ToList();
            list.Add(new { id = "0", text = "Manage digital wallet", parent = "#", idPath = "0", state = new { opened = "true", selected = "true" } });
            return JsonConvert.SerializeObject(list);
        }

        #endregion

        #region [Định khoản công nợ - AccountingPayReceivable]
        [LogTracker(EnumAction.View, EnumPage.AccountingPayReceivable)]
        public async Task<ActionResult> AccountingPayReceivable(ModelView<PayReceivable, PayReceivableModel> model)
        {

            long totalRecord;

            if (model.SearchInfo == null)
            {
                model.SearchInfo = new PayReceivableModel();
            }
            model.Items = await UnitOfWork.PayReceivableRepo.FindAsync(
               out totalRecord,
               x => !x.IsDelete && (x.IdPath == model.SearchInfo.Path || x.IdPath.StartsWith(model.SearchInfo.Path + ".")),
               x => x.OrderBy(y => y.ParentId),
               model.PageInfo.CurrentPage,
               model.PageInfo.PageSize
           );

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Name = "Định khoản công nợ";
            model.PageInfo.Url = Url.Action("AccountingPayReceivable", "Finance");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_PayReceivableList", model);
            }

            ViewBag.payReceivableJsTree = PayReceivableJsTree();

            return View(model);
        }

        public ActionResult CreatePayReceivable()
        {
            ViewBag.payReceivableJsTree = PayReceivableActonJsTree();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogTracker(EnumAction.Add, EnumPage.AccountingPayReceivable)]
        public async Task<ActionResult> CreatePayReceivable(PayReceivablesMeta model)
        {
            ViewBag.payReceivableJsTree = PayReceivableActonJsTree();
            ModelState.Remove("Id");
            ModelState.Remove("IsIdSystem");
            if (!ModelState.IsValid)
                return View();

            // IDD định khoản đã tồn tại
            if (await UnitOfWork.PayReceivableRepo.AnyAsync(x => x.Idd == model.IDD && model.IDD != null && !x.IsDelete))
            {
                ModelState.AddModelError("IDD", $"IDD định khoản công nợ \"{model.IDD }\" already exists");
                return View();
            }

            // Tên định khoản đã tồn tại
            if (await UnitOfWork.PayReceivableRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete))
            {
                ModelState.AddModelError("Name", $"Tên định khoản công nợ \"{model.Name }\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var trea = Mapper.Map<PayReceivable>(model);
                    var obj = new PayReceivable();
                    if (trea.ParentId != 0)
                    {
                        obj = UnitOfWork.PayReceivableRepo.SingleOrDefault(x => x.Id == trea.ParentId);
                        if (obj != null)
                        {
                            obj.IsParent = true;
                            trea.IdPath = string.Empty;
                            trea.NamePath = obj.NamePath + "/" + trea.Name;
                            trea.ParentId = obj.Id;
                            trea.ParentName = obj.Name;
                            trea.Operator = obj.Operator;
                        }
                        else
                        {
                            TempData["Msg"] = $"does not exist định khoản công nợ hoặc đã bị xóa \"<b>{trea.ParentName}</b>\"";
                        }

                    }
                    else
                    {
                        trea.IdPath = string.Empty;
                        trea.NamePath = trea.Name;
                        trea.ParentId = 0;
                        trea.ParentName = "";
                        trea.Operator = false;
                    }


                    UnitOfWork.PayReceivableRepo.Add(trea);
                    var rs = await UnitOfWork.PayReceivableRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View();
                    }
                    //Cập nhật lại IdPath
                    if (trea.ParentId != 0)
                    {
                        trea.IdPath = obj.IdPath + "." + trea.Id;
                    }
                    else
                    {
                        trea.IdPath = "0." + trea.Id;
                    }
                    await UnitOfWork.PayReceivableRepo.SaveAsync();

                    TempData["Msg"] = $"Add successful định khoản công nợ \"<b>{trea.Name}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("CreatePayReceivable");
        }

        [LogTracker(EnumAction.Update, EnumPage.AccountingPayReceivable)]
        public async Task<ActionResult> EditPayReceivable(int id)
        {
            ViewBag.payReceivableJsTree = PayReceivableActonJsTree();
            var trea = await UnitOfWork.PayReceivableRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return HttpNotFound($"Không có định khoản công nợ nào có Id là {id}");

            return View(Mapper.Map<PayReceivablesMeta>(trea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogTracker(EnumAction.Update, EnumPage.AccountingPayReceivable)]
        public async Task<ActionResult> EditPayReceivable(PayReceivablesMeta model)
        {
            ViewBag.payReceivableJsTree = PayReceivableActonJsTree();
            ModelState.Remove("IsIdSystem");
            if (!ModelState.IsValid)
                return View(model);

            var trea = await UnitOfWork.PayReceivableRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (trea == null)
            {
                ModelState.AddModelError("NotExist", "Định khoản does not exist or has been deleted");
                return View(model);
            }

            PayReceivable treaParent = null;

            // Có thay đổi đơn vị cha
            if (model.ParentId != trea.ParentId)
            {
                // Kiểm tra đơn vị Cha có tồn tại hay không
                treaParent =
                    await UnitOfWork.PayReceivableRepo.SingleOrDefaultAsync(x => x.Id == model.ParentId && !x.IsDelete);
                if (treaParent == null)
                {
                    ModelState.AddModelError("ParentId",
                        $"Định khoản cha \"{model.ParentName}\" does not exist or has been deleted");
                    return View(model);
                }
                model.ParentName = treaParent.Name;

            }
            else
            {
                model.ParentName = trea.ParentName;
            }
            // IDD định khoản đã tồn tại
            if (await UnitOfWork.PayReceivableRepo.AnyAsync(x => x.Idd == model.IDD && model.IDD != null && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("IDD", $"IDD định khoản công nợ \"{model.IDD }\" already exists");
                return View();
            }
            // Tên chuyên mục đã tồn tại
            if (
                await UnitOfWork.PayReceivableRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentId == model.ParentId && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Tên định khoản công nợ \"{model.Name}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldParentId = trea.ParentId;
                    var oldIdPath = trea.IdPath;
                    var oldNamePath = trea.NamePath;
                    trea = Mapper.Map(model, trea);

                    var rs = await UnitOfWork.PayReceivableRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View(model);
                    }

                    // Cập nhật lại IdPath và NamePath cho đơn vị
                    if (model.ParentId != oldParentId)
                    {
                        if (treaParent == null)
                        {
                            trea.IdPath = trea.Id.ToString();
                        }
                        else
                        {
                            trea.IdPath = $"{treaParent.IdPath}.{trea.Id}";
                            trea.NamePath = $"{treaParent.NamePath}/{trea.Name}";
                            treaParent.IsParent = true;

                            //check định khoản cha cũ
                            var parent = await UnitOfWork.PayReceivableRepo.FirstOrDefaultAsync(x => x.Id == oldParentId);
                            if (parent != null)
                            {
                                var countChil = await UnitOfWork.PayReceivableRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                                parent.IsParent = countChil != 0;
                                await UnitOfWork.PayReceivableRepo.SaveAsync();
                            }
                        }

                        // Cập nhật lại IdPath của tất cả các đơn vị bên dưới
                        var listSubTrea = await UnitOfWork.PayReceivableRepo.FindAsync(
                                    x => !x.IsDelete && x.IdPath.StartsWith(oldIdPath + "."));

                        if (listSubTrea != null)
                        {
                            listSubTrea.ForEach(o =>
                            {
                                o.IdPath = $"{trea.IdPath}{o.IdPath.Substring(oldIdPath.Length, o.IdPath.Length - oldIdPath.Length)}";
                                o.NamePath = $"{trea.NamePath}{o.NamePath.Substring(oldNamePath.Length, o.NamePath.Length - oldNamePath.Length)}";
                            });
                        }

                    }

                    await UnitOfWork.PayReceivableRepo.SaveAsync();

                    TempData["Msg"] = $"Successfully updated the statement \"<b>{trea.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("AccountingPayReceivable");
        }

        [HttpPost]
        [LogTracker(EnumAction.Delete, EnumPage.AccountingPayReceivable)]
        public async Task<ActionResult> DeletePayReceivable(int id)
        {
            var rs = 1;
            var trea = await UnitOfWork.PayReceivableRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    trea.IsDelete = true;
                    UnitOfWork.PayReceivableRepo.Update(trea);

                    rs = await UnitOfWork.PayReceivableRepo.SaveAsync();

                    //check công nợ cha cũ
                    var parent = await UnitOfWork.PayReceivableRepo.FirstOrDefaultAsync(x => x.Id == trea.ParentId);
                    if (parent != null)
                    {
                        var countChil =
                            await UnitOfWork.PayReceivableRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                        parent.IsParent = countChil != 0;
                        await UnitOfWork.PayReceivableRepo.SaveAsync();
                    }

                    //Xóa định khoản công nợ con
                    if (trea.IsParent)
                    {
                        var listChil = await UnitOfWork.PayReceivableRepo.FindAsync(x => x.IdPath.Contains(trea.IdPath) && !x.IsDelete);
                        foreach (var item in listChil)
                        {
                            item.IsDelete = true;
                        }
                        await UnitOfWork.PayReceivableRepo.SaveAsync();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        public string PayReceivableActonJsTree()
        {
            var list = UnitOfWork.DbContext.PayReceivables.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId == 0 ? "#" : o.ParentId.ToString(),
                idPath = o.IdPath
            }).ToList();
            return JsonConvert.SerializeObject(list);
        }

        public string PayReceivableJsTree()
        {
            var list = UnitOfWork.DbContext.PayReceivables.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                idPath = o.IdPath,
                state = new { opened = "", selected = "" },
            }).ToList();
            list.Add(new { id = "0", text = "Managing debt obligations", parent = "#", idPath = "0", state = new { opened = "true", selected = "true" } });
            return JsonConvert.SerializeObject(list);
        }
        

        #endregion

        #region [Permistion]
        public ActionResult AddUserPermistion(int userId, int isDelete)
        {
            var tmpList = new List<UserPermistionItem>();
            if (Session["UserPermistionFinance"] != null)
            {
                tmpList = (List<UserPermistionItem>)Session["UserPermistionFinance"];
            }
            if (tmpList.Any())
            {
                var tmpExit = tmpList.FirstOrDefault(m => m.Id == userId);
                if (tmpExit != null)
                {
                    if (isDelete == 1)
                    {
                        tmpList.Remove(tmpExit);
                    }
                }
                else
                {

                    var tmpUser = UnitOfWork.UserRepo.Find(m => m.Id == userId).FirstOrDefault();
                    if (tmpUser != null)
                    {
                        var tmpItem = new UserPermistionItem();
                        tmpItem.Id = tmpUser.Id;
                        tmpItem.UserName = tmpUser.UserName;
                        tmpItem.UserFullname = tmpUser.FullName;
                        tmpItem.OfficeName = tmpUser.Department;
                        tmpList.Add(tmpItem);
                    }
                }
            }
            else
            {
                var tmpExit = tmpList.FirstOrDefault(m => m.Id == userId);
                if (tmpExit == null)
                {
                    var tmpUser = UnitOfWork.UserRepo.Find(m => m.Id == userId).FirstOrDefault();
                    if (tmpUser != null)
                    {
                        var tmpItem = new UserPermistionItem();
                        tmpItem.Id = tmpUser.Id;
                        tmpItem.UserName = tmpUser.UserName;
                        tmpItem.UserFullname = tmpUser.FullName;
                        tmpItem.OfficeName = tmpUser.Department;
                        tmpList.Add(tmpItem);
                    }
                }
            }
            Session["UserPermistionFinance"] = tmpList;
            return PartialView("_ListPermistion", tmpList);
        }
        #endregion
    }
}