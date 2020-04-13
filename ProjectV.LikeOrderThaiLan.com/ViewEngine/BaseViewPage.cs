using System.Globalization;
using System.Web.Mvc;
using Library.Models;

namespace ProjectV.LikeOrderThaiLan.com.ViewEngine
{
    public abstract class BaseViewPage : WebViewPage
    {
        public virtual CustomerState CustomerState => (CustomerState) ViewData["CustomerState"] ?? new CustomerState();
        public virtual int ExchangeRate => (int) ViewData["ExchangeRate"];
        public virtual CultureInfo CultureInfo => (CultureInfo)ViewData["CultureInfo"];

        //public bool CheckPermision(EnumAction action, int pageId)
        //{
        //    return CurrentUser.Pages.Any(c => c.PageId == pageId && (((c.Add && action == EnumAction.Add)) || (c.Update && action == EnumAction.Update) ||
        //        (c.Delete && action == EnumAction.Delete) || (c.View && action == EnumAction.View) || (c.Export && action == EnumAction.Export) ||
        //        (c.Approve && action == EnumAction.Approve)));
        //}
    }

    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual CustomerState CustomerState => (CustomerState)ViewData["CustomerState"] ?? new CustomerState();
        public virtual int ExchangeRate => (int)ViewData["ExchangeRate"];
        public virtual CultureInfo CultureInfo => (CultureInfo)ViewData["CultureInfo"];
        //public bool CheckPermision(EnumAction action, int pageId)
        //{
        //    return CurrentUser.Pages.Any(c => c.PageId == pageId && (((c.Add && action == EnumAction.Add)) || (c.Update && action == EnumAction.Update) ||
        //        (c.Delete && action == EnumAction.Delete) || (c.View && action == EnumAction.View) || (c.Export && action == EnumAction.Export) ||
        //        (c.Approve && action == EnumAction.Approve)));
        //}
    }
}