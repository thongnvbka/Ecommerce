using System;
using System.IO;

namespace Common.Helper
{
    public class PdfConvert
    {
        public static byte[] ConvertHtmlToPdfWithHeaderAndFooter(string htmlContent, PageOrientation orientation = PageOrientation.Default,
            PageSize pageSize = PageSize.Default, PageMargins margin = null, string customWkHtmlArgs = null)
        {
            var convert = new HtmlToPdfConverter
            {
                PageHeaderHtml = string.Format("<img style=\"width:810px;\" src=\"{0}\" alt=\"\" />", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\Images\HeaderVTI.jpg")),
                PageFooterHtml = string.Format("<div style=\"width:100%;text-align:center\"><img style=\"width:810px;\" src=\"{0}\" alt=\"\" /></div>", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\Images\FooterVTI.jpg")),
                CustomWkHtmlArgs = "--header-spacing 5 --footer-spacing 5 ",
                Orientation = orientation,
                Size = pageSize,
                Margins = margin
            };
            if(!string.IsNullOrWhiteSpace(customWkHtmlArgs))
            {
                convert.CustomWkHtmlArgs += customWkHtmlArgs;
            }

            return convert.GeneratePdf(htmlContent);
        }

        public static byte[] ConvertHtmlToPdf(string htmlContent, PageOrientation orientation = PageOrientation.Default,
            PageSize pageSize = PageSize.Default, PageMargins margin = null, string customWkHtmlArgs = null)
        {
            var convert = new HtmlToPdfConverter
            {
                CustomWkHtmlArgs = customWkHtmlArgs,
                Orientation = orientation,
                Size = pageSize,
                Margins = margin
            };

            return convert.GeneratePdf(htmlContent);
        }
    }
}