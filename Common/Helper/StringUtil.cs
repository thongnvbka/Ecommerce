using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using RazorEngine;

namespace Common.Helper
{
    public class StringUtil
    {
        public static string GetWords(string source, int wordsCount)
        {
            return source == null ? string.Empty : GetWords(source, wordsCount, source.Length);
        }
        public static string GetWords(string source, int wordsCount, int maxLength)
        {
            var strRtn = string.Empty;
            var intWCount = 0;
            var limitLength = wordsCount * 4;
            if (!string.IsNullOrEmpty(source))
            {
                //source = Localization.LocalizationUtils.RemoveSpecialCharsV2(source);
                var words = source.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if (!string.IsNullOrEmpty(word.Trim()))
                    {
                        if (strRtn.Length == 0)
                            strRtn = word;
                        else
                            strRtn += " " + word;

                        intWCount += 1;
                    }

                    if (intWCount >= wordsCount
                        || strRtn.Length > Math.Min(limitLength, maxLength)) break;
                }

                if (strRtn.Length < source.Length) strRtn += "...";
            }
            return strRtn;
        }

        public static string GetWordByLength(string source, int length)
        {
            var strRtn = string.Empty;
            if (!string.IsNullOrEmpty(source))
            {
                //source = Localization.LocalizationUtils.RemoveSpecialCharsV2(source);
                var words = source.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if (!string.IsNullOrEmpty(word.Trim()))
                    {
                        if (strRtn.Length == 0)
                            strRtn = word;
                        else
                            strRtn += " " + word;
                    }
                    if (strRtn.Length > length && strRtn.Length < source.Length)
                    {
                        var lastSpaceIndex = strRtn.LastIndexOf(' ');
                        strRtn = strRtn.Substring(0, lastSpaceIndex);
                        break;
                    }
                }

                if (strRtn.Length < source.Length) strRtn += "...";
            }
            return strRtn;
        }

        public static string RenderPartialToString(string viewPath, object model)
        {
            var viewAbsolutePath = System.Web.Hosting.HostingEnvironment.MapPath(viewPath);

            var viewSource = File.ReadAllText(viewAbsolutePath);

            string renderedText = Razor.Parse(viewSource, model);
            return renderedText;
        }

        public static string RenderPartialViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            //try
            //{
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
            //}
            //catch (Exception ex)
            //{
            //    return ex.ToString();
            //}
        }
    }
}
