using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System;
using Common.Host;
using System.Web.Mvc;
using System.Web.Routing;
using Common.ActionResult;
using Library.Models;
using Library.UnitOfWork;
using Newtonsoft.Json;
using Library.ViewModels.Items;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Microsoft.Owin.Security;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace Cms.Controllers
{
    public class BaseController : Controller
    {
        public UserState UserState { get; set; }
        public CultureInfo CultureInfo = new CultureInfo("vi-VN", false);
        public UnitOfWork UnitOfWork { get; }

        public BaseController()
        {
            UnitOfWork = new UnitOfWork();
        }

        public static DateTime GetStartOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }
        public static DateTime GetEndOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999);
        }

        public decimal ExchangeRate()
        {
            return (decimal)5.19;
        }

        public int TimeDelay => int.Parse(ConfigurationManager.AppSettings["TimeDelay"]);

        // GET: Base
        protected override void OnException(ExceptionContext filterContext)
        {
            //Log Exception e
            OutputLog.WriteOutputLog(filterContext.Exception);
        }

        public async Task<string> GetLayoutJsTree(int? selectedId = null)
        {
            var offices = await
                UnitOfWork.LayoutRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var listJsTree = offices.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentLayoutId?.ToString() ?? "#",
                state = new { opened = !o.ParentLayoutId.HasValue, selected = selectedId != null && o.Id == selectedId },
                idPath = o.IdPath,
                code = o.Code
            });

            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> GetOfficeJsTree(int? selectedId = null)
        {
            var offices = await
                UnitOfWork.OfficeRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var listJsTree = offices.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId?.ToString() ?? "#",
                state = new { opened = !o.ParentId.HasValue, selected = selectedId != null && o.Id == selectedId },
                idPath = o.IdPath,
                code = o.Code
            });

            return JsonConvert.SerializeObject(listJsTree);
        }
        #region Lấy dữ liệu dùng chung quỹ + định khoản
        public async Task<string> GetFinanceFundJsTree(string curence, int? selectedId = null)
        {
            var userId = UserState.UserId;
            var listObject = await
                UnitOfWork.FinaceFundRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var listJsTree = new List<dynamic> { new
            {
                id = "0",
                text = "Fund management",
                parent = "#",
                idPath = "0",
                state = new { opened = true, disabled = true},
                CardName = "",
                CardId = "",
                CardBank = "",
                CardBranch = "",
                BankAccountNumber = "",
                Department = "",
                NameBank = "",
                UserId = "",
                UserCode = "",
                UserFullName = "",
                UserPhone = "",
                UserEmail = ""
            } };
            foreach (var item in listObject)
            {
                if (GetFundWallet(userId, item.Maxlength))
                {
                    listJsTree.Add(new
                    {
                        id = item.Id.ToString(),
                        text = item.Name,
                        parent = item.ParentId.ToString(),
                        state = new { opened = true, selected = selectedId != null && item.Id == selectedId, disabled = item.IsParent },
                        idPath = item.IdPath,
                        item.CardName,
                        item.CardId,
                        item.CardBank,
                        item.CardBranch,
                        item.BankAccountNumber,
                        item.Department,
                        item.NameBank,
                        item.UserId,
                        item.UserCode,
                        item.UserFullName,
                        item.UserPhone,
                        item.UserEmail
                    });
                }
                else
                {
                    listJsTree.Add(new
                    {
                        id = item.Id.ToString(),
                        text = item.Name,
                        parent = item.ParentId.ToString(),
                        state = new { opened = true, selected = selectedId != null && item.Id == selectedId, disabled = item.IsParent || item.Currency != curence },
                        idPath = item.IdPath,
                        item.CardName,
                        item.CardId,
                        item.CardBank,
                        item.CardBranch,
                        item.BankAccountNumber,
                        item.Department,
                        item.NameBank,
                        item.UserId,
                        item.UserCode,
                        item.UserFullName,
                        item.UserPhone,
                        item.UserEmail
                    });
                }
            }
            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> GetFinanceFundJsAccountTree(string curence, int? selectedId = null)
        {
            var userId = UserState.UserId;
            var listObject = await
                UnitOfWork.FinaceFundRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var listJsTree = new List<dynamic> { new
            {
                id = "0",
                text = "Fund management",
                parent = "#",
                idPath = "0",
                state = new { opened = true, disabled = true},
                CardName = "",
                CardId = "",
                CardBank = "",
                CardBranch = "",
                BankAccountNumber = "",
                Department = "",
                NameBank = "",
                UserId = "",
                UserCode = "",
                UserFullName = "",
                UserPhone = "",
                UserEmail = ""
            } };
            foreach (var item in listObject)
            {
                if (GetFundWallet(userId, item.Maxlength))
                {
                    listJsTree.Add(new
                    {
                        id = item.Id.ToString(),
                        text = item.Name,
                        parent = item.ParentId.ToString(),
                        state = new { opened = true, selected = selectedId != null && item.Id == selectedId, disabled = item.IsParent },
                        idPath = item.IdPath,
                        item.CardName,
                        item.CardId,
                        item.CardBank,
                        item.CardBranch,
                        item.BankAccountNumber,
                        item.Department,
                        item.NameBank,
                        item.UserId,
                        item.UserCode,
                        item.UserFullName,
                        item.UserPhone,
                        item.UserEmail
                    });
                }
                else
                {
                    listJsTree.Add(new
                    {
                        id = item.Id.ToString(),
                        text = item.Name,
                        parent = item.ParentId.ToString(),
                        state = new { opened = true, selected = selectedId != null && item.Id == selectedId, disabled = item.IsParent || item.Currency != curence },
                        idPath = item.IdPath,
                        item.CardName,
                        item.CardId,
                        item.CardBank,
                        item.CardBranch,
                        item.BankAccountNumber,
                        item.Department,
                        item.NameBank,
                        item.UserId,
                        item.UserCode,
                        item.UserFullName,
                        item.UserPhone,
                        item.UserEmail
                    });
                }
            }
            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> GetFinanceFundJsTreeSearch(string curence, int? selectedId = null)
        {
            var userId = UserState.UserId;
            var listObject = await
                UnitOfWork.FinaceFundRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var listJsTree = new List<dynamic> { new
            {
                id = "0",
                text = "Fund management",
                parent = "#",
                idPath = "0",
                state = new { opened = true, disabled = false},
                CardName = "",
                CardId = "",
                CardBank = "",
                CardBranch = "",
                BankAccountNumber = "",
                Department = "",
                NameBank = "",
                UserId = "",
                UserCode = "",
                UserFullName = "",
                UserPhone = "",
                UserEmail = ""
            } };
            foreach (var item in listObject)
            {
                //if (GetFundWallet(userId, item.Maxlength))
                //{
                    listJsTree.Add(new
                    {
                        id = item.Id.ToString(),
                        text = item.Name,
                        parent = item.ParentId.ToString(),
                        state = new { opened = true, selected = selectedId != null && item.Id == selectedId, disabled = false},
                        idPath = item.IdPath,
                        item.CardName,
                        item.CardId,
                        item.CardBank,
                        item.CardBranch,
                        item.BankAccountNumber,
                        item.Department,
                        item.NameBank,
                        item.UserId,
                        item.UserCode,
                        item.UserFullName,
                        item.UserPhone,
                        item.UserEmail
                    });
                //}
                //else
                //{
                //    listJsTree.Add(new
                //    {
                //        id = item.Id.ToString(),
                //        text = item.Name,
                //        parent = item.ParentId.ToString(),
                //        state = new { opened = true, selected = selectedId != null && item.Id == selectedId, disabled = item.Currency != curence },
                //        idPath = item.IdPath,
                //        item.CardName,
                //        item.CardId,
                //        item.CardBank,
                //        item.CardBranch,
                //        item.BankAccountNumber,
                //        item.Department,
                //        item.NameBank,
                //        item.UserId,
                //        item.UserCode,
                //        item.UserFullName,
                //        item.UserPhone,
                //        item.UserEmail
                //    });
                //}
            }

            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> GetFinanceFundJsAccountantSearch(int? selectedId = null)
        {
            var userId = UserState.UserId;
            var listObject = await
                UnitOfWork.FinaceFundRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var listJsTree = new List<dynamic> { new
            {
                id = "0",
                text = "Fund management",
                parent = "#",
                idPath = "0",
                state = new { opened = true, disabled = true},
                CardName = "",
                CardId = "",
                CardBank = "",
                CardBranch = "",
                BankAccountNumber = "",
                Department = "",
                NameBank = "",
                UserId = "",
                UserCode = "",
                UserFullName = "",
                UserPhone = "",
                UserEmail = ""
            } };
            foreach (var item in listObject)
            {
                listJsTree.Add(new
                {
                    id = item.Id.ToString(),
                    text = item.Name,
                    parent = item.ParentId.ToString(),
                    state = new { opened = true, selected = selectedId != null && item.Id == selectedId, disabled = item.IsParent },
                    idPath = item.IdPath,
                    item.CardName,
                    item.CardId,
                    item.CardBank,
                    item.CardBranch,
                    item.BankAccountNumber,
                    item.Department,
                    item.NameBank,
                    item.UserId,
                    item.UserCode,
                    item.UserFullName,
                    item.UserPhone,
                    item.UserEmail
                });


            }

            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> GetTreasureJsTree(int? selectedId = null, bool oper = false)
        {
            var listObject = await
                UnitOfWork.TreasureRepo.Entities.Where(x => !x.IsDelete && x.Status < 2 && x.Operator == oper)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var list = listObject.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                state = new { opened = true, selected = selectedId != null && o.Id == selectedId, disabled = o.IsParent },
                idPath = o.IdPath
            });

            var listJsTree = new List<dynamic> { new { id = "0", text = "Quản lý Type định khoản", parent = "#", idPath = "0", state = new { opened = true, disabled = true } } };
            listJsTree.AddRange(list);

            return JsonConvert.SerializeObject(listJsTree);
        }
        // Treasure Collect - Công nợ phải thu
        public async Task<string> GetTreasureCollectJsTree(int? selectedId = null)
        {
            var listObject = await
                UnitOfWork.PayReceivableRepo.Entities.Where(x => !x.IsDelete && x.Status < 2 && x.Operator == true)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var list = listObject.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                state = new { opened = true, selected = selectedId != null && o.Id == selectedId, disabled = o.IsParent || o.IsIdSystem == true },
                idPath = o.IdPath
            });

            var listJsTree = new List<dynamic> { new { id = "0", text = "Quản lý loại định khoản", parent = "#", idPath = "0", state = new { opened = true, disabled = true } } };
            listJsTree.AddRange(list);

            return JsonConvert.SerializeObject(listJsTree);
        }

        // Treasure Return - Công nợ phải trả
        public async Task<string> GetTreasureReturnJsTree(int? selectedId = null)
        {
            var listObject = await
                UnitOfWork.PayReceivableRepo.Entities.Where(x => !x.IsDelete && x.Status < 2 && x.Operator == false)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var list = listObject.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                state = new { opened = true, selected = selectedId != null && o.Id == selectedId, disabled = o.IsParent || o.IsIdSystem == true },
                idPath = o.IdPath
            });

            var listJsTree = new List<dynamic> { new { id = "0", text = "Quản lý loại định khoản", parent = "#", idPath = "0", state = new { opened = true, disabled = true } } };
            listJsTree.AddRange(list);

            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> GetTreasureJsTreeSearch(int? selectedId = null)
        {
            var listObject = await
                UnitOfWork.TreasureRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var list = listObject.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                state = new { opened = true, selected = selectedId != null && o.Id == selectedId, disabled = false/* o.IsParent*/ },
                idPath = o.IdPath
            });

            var listJsTree = new List<dynamic> { new { id = "0", text = "Quản lý loại định khoản", parent = "#", idPath = "0", state = new { opened = true, disabled = false } } };
            listJsTree.AddRange(list);

            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> GetTreasureWalletJsTreeSearch(int? selectedId = null)
        {
            var listObject = await
                UnitOfWork.CustomerWalletRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var list = listObject.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                state = new { opened = true, selected = selectedId != null && o.Id == selectedId, disabled = false/* o.IsParent*/ },
                idPath = o.IdPath
            });

            var listJsTree = new List<dynamic> { new { id = "0", text = "Quản lý loại định khoản ví", parent = "#", idPath = "0", state = new { opened = true, disabled = false } } };
            listJsTree.AddRange(list);

            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> PayReceiveSearchTree(int? selectedId = null)
        {
            var listObject = await
                UnitOfWork.PayReceivableRepo.Entities.Where(x => !x.IsDelete && x.Status < 2)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var list = listObject.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                state = new { opened = true, selected = selectedId != null && o.Id == selectedId, disabled = false/* o.IsParent*/ },
                idPath = o.IdPath
            });

            var listJsTree = new List<dynamic> { new { id = "0", text = "Quản lý loại định khoản", parent = "#", idPath = "0", state = new { opened = true, disabled = false } } };
            listJsTree.AddRange(list);

            return JsonConvert.SerializeObject(listJsTree);
        }

        public async Task<string> GetTreasureWalletJsTree(int? selectedId = null, bool oper = false)
        {
            var listObject = await
                UnitOfWork.CustomerWalletRepo.Entities.Where(x => !x.IsDelete && x.Status < 2 && x.Operator == oper)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var list = listObject.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                state = new { opened = true, selected = selectedId != null && o.Id == selectedId, disabled = o.IsParent || o.IsIdSystem == true },
                idPath = o.IdPath
            });

            var listJsTree = new List<dynamic> { new { id = "0", text = "Quản lý loại định khoản", parent = "#", idPath = "0", state = new { opened = true, disabled = true } } };
            listJsTree.AddRange(list);

            return JsonConvert.SerializeObject(listJsTree);
        }

        #endregion

        public string GetCatetgoryJsTree()
        {
            var offices = UnitOfWork.CategoryRepo.Entities.Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToList();

            var listJsTree = offices.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId == 0 ? "#" : o.ParentId.ToString(),
                idPath = o.IdPath,
            });

            return JsonConvert.SerializeObject(listJsTree);
        }

        protected string[] GetBlackListExtensions()
        {
            string[] blackList = { ".exe", ".cshtml", ".vbhtml", ".aspx", ".ascx", ".msi", ".bin", ".js", ".bat", ".cmd", ".ps1", ".reg", ".rgs", ".ws", ".wsf" };
            var blacklistConfig = ConfigurationManager.AppSettings["BlackListExtentions"];
            if (!string.IsNullOrWhiteSpace(blacklistConfig))
            {
                blacklistConfig = blacklistConfig.Replace(" ", "").ToLower();
                var split = blacklistConfig.Split(',', ';');
                if (split.Length != 0)
                {
                    blackList = split;
                }
            }
            return blackList;
        }

        protected int GetMaxFileLength()
        {
            var maxLengthConfig = ConfigurationManager.AppSettings["MaxFileLength"];
            int maxLength;
            if (string.IsNullOrWhiteSpace(maxLengthConfig))
            {
                maxLength = 5120000;
            }
            else
            {
                if (!int.TryParse(maxLengthConfig, out maxLength))
                {
                    maxLength = 5120000;
                }
            }

            return maxLength;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            // Grab the user's login information from Identity
            UserState userState = new UserState();
            if (User is ClaimsPrincipal)
            {
                var user = User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                var userStateString = GetClaim(claims, "userState");
                //var name = GetClaim(claims, ClaimTypes.Name);
                //var id = GetClaim(claims, ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }
            UserState = userState;

            ViewData["UserState"] = UserState;
        }

        public static string GetClaim(List<Claim> claims, string key)
        {
            var claim = claims.FirstOrDefault(c => c.Type == key);
            if (claim == null)
                return null;

            return claim.Value;
        }

        /// <summary>
        /// Allow external initialization of this controller by explicitly
        /// passing in a request context
        /// </summary>
        /// <param name="requestContext"></param>
        public void InitializeForced(RequestContext requestContext)
        {
            Initialize(requestContext);
        }

        public ActionResult JsonCamelCaseResult(object obj, JsonRequestBehavior jsonAllwoBehavior)
        {
            return new JsonCamelCaseResult(obj, jsonAllwoBehavior);
        }

        #region [Hàm chung lọc phân quyền quỹ]
        public bool GetFundWallet(int userId, string maxLength)
        {
            bool check = false;
            var tmpList = new List<UserPermistionItem>();
            if (maxLength != null)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                tmpList = jss.Deserialize<List<UserPermistionItem>>(maxLength);
            }
            foreach (var item in tmpList)
            {
                if (item.Id == userId)
                {
                    check = true;
                    break;
                }
            }

            return check;
        }
        #endregion

        public void AddUpdateClaim(string key, string value)
        {
            var identity = User.Identity as ClaimsIdentity;

            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));

            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant =
                new AuthenticationResponseGrant(new ClaimsPrincipal(identity),
                    new AuthenticationProperties() { IsPersistent = true });
        }

        public string GetClaimValue(string key)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.First(c => c.Type == key);
            return claim.Value;
        }

        public string RemoveCode(string code)
        {
            return code.Trim().ToUpper().Replace("ORD", "").Replace("DEP", "").Replace("COM", "").Replace("SOU", "").Replace("STO", "");
        }

        public bool DataCompare(dynamic before, dynamic after)
        {
            return before != after;
        }
    }
}