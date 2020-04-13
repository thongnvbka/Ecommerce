using System;
using System.IO;

namespace Common.Helper
{
    public class TemplateHtmlHelper
    {
        public static string Render(string template, object model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (String.IsNullOrEmpty(template))
            {
                throw new ArgumentNullException("template");
            }
            if (!File.Exists(template))
            {
                throw new Exception("Template Not Found!");
            }
            var templateContent = File.ReadAllText(template);
            var templateService = new TemplateService();
            var result = templateService.Parse(templateContent, model, null, null);
            return result;
        }
    }
}