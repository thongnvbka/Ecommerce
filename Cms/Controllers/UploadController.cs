using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Helper;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Attachment = System.Net.Mail.Attachment;

namespace Cms.Controllers
{
    public class UploadController : BaseController
    {
        // GET: Upload
        public async Task<ActionResult> DownloadAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Content("Parameters can not be empty");
            }
            var path = Encryptor.Base64Decode(id);
            var fileName = Path.GetFileName(path);
            var filePath = Server.MapPath(path);

            if (!System.IO.File.Exists(filePath))
            {
                return Content("File does not exist");
            }

            return await Task.Run(() => File(filePath, GetContentType(filePath), fileName));
        }

        public async Task<ActionResult> ConvertWebp(string image)
        {
            if (string.IsNullOrWhiteSpace(image))
            {
                return Content("Parameters can not be empty");
            }
            // path dạng: /upload/yyyy/mm/dd/fileName.extention
            var path = Encryptor.Base64Decode(image);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var soucePath = Server.MapPath(path);

            if (!System.IO.File.Exists(soucePath))
            {
                return Content("File does not exist");
            }

            ISupportedImageFormat format = new JpegFormat();
            var isWebp = false;
            // Check Version
            if (Request.Browser.IsMobileDevice)
            {
                if ((Request.Browser.Browser.ToLower().Equals("chrome") && Request.Browser.MajorVersion >= 25) ||
                    (Request.Browser.Browser.ToLower().Equals("opera ") && Request.Browser.MajorVersion >= 13))
                {
                    format = new WebPFormat();
                    isWebp = true;
                }
            }
            else
            {
                if (Request.Browser.Browser.ToLower().Equals("chrome") && Request.Browser.MajorVersion >= 23)
                {
                    format = new WebPFormat();
                    isWebp = true;
                }
            }

            // Không cần chuyển thành webp
            if (!isWebp)
            {
                return await Task.Run(() => File(soucePath, GetContentType(soucePath), Path.GetFileName(path)));
            }

            // buid new file
            var folder = Path.GetDirectoryName(path);
            fileName = $"{fileName}.{format.DefaultExtension}";

            // ReSharper disable once PossibleNullReferenceException
            var folderCache = folder.Replace("upload", "thumb");

            var cachePath = Server.MapPath(folderCache);

            var filePath = Path.Combine(cachePath, fileName);

            // Tồn tại file đã resize
            if (System.IO.File.Exists(filePath))
            {
                //return Redirect(cachePath);
                return await Task.Run(() => File(filePath, GetContentType(filePath), fileName));
            }

            // Tạo folder cache nếu chưa có
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write);

            using (var imageFactory = new ImageFactory(true))
            {
                // Load, resize, set the format and quality and save an image.
                imageFactory.Load(soucePath)
                            .Format(format)
                            .BackgroundColor(Color.White)
                            .Save(stream);
            }
            await stream.FlushAsync();

            return await Task.Run(() => File(stream, GetContentType(filePath), fileName));
        }

        public async Task<ActionResult> ResizeThumbnail(string id, short width, short height, byte thumType = 1)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Content("Parameters can not be empty");
            }
            // path dạng: /upload/yyyy/mm/dd/fileName.extention
            var path = Encryptor.Base64Decode(id);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var soucePath = Server.MapPath(path);

            // soucepaht not exists
            if (!System.IO.File.Exists(soucePath))
            {
                return Content("File does not exist");
            }

            // không cho phép resize
            if (!AllowThumbSize(width, height))
                return await Task.Run(() => File(soucePath, GetContentType(soucePath), fileName));

            var folder = Path.GetDirectoryName(path);
            ISupportedImageFormat format = new JpegFormat();
            // Check Version
            if (Request.Browser.IsMobileDevice)
            {
                if ((Request.Browser.Browser.ToLower().Equals("chrome") && Request.Browser.MajorVersion >= 25) ||
                    (Request.Browser.Browser.ToLower().Equals("opera ") && Request.Browser.MajorVersion >= 13))
                {
                    format = new WebPFormat();
                }
            }
            else
            {
                if (Request.Browser.Browser.ToLower().Equals("chrome") && Request.Browser.MajorVersion >= 23)
                {
                    format = new WebPFormat();
                }
            }

            // ReSharper disable once PossibleNullReferenceException
            //var name = fileName.Substring(0, fileName.LastIndexOf('.'));
            fileName = $"{fileName}_{width}_{height}_{thumType}.{format.DefaultExtension}";

            // ReSharper disable once PossibleNullReferenceException
            var folderCache = folder.Replace("upload", "thumb");

            var cachePath = Server.MapPath(folderCache);

            var filePath = Path.Combine(cachePath, fileName);

            // Tồn tại file đã resize
            if (System.IO.File.Exists(filePath))
            {
                //return Redirect(cachePath);
                return await Task.Run(() => File(filePath, GetContentType(filePath), fileName));
            }

            // Tạo folder cache nếu chưa có
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            // resize file
            //ResizeStreamThumnal(soucePath, filePath, width, height, thumType);
            var size = new ResizeLayer(new Size(width, height));
            size.ResizeMode = thumType == 1 ? ResizeMode.Crop : ResizeMode.Pad;
            size.AnchorPosition = AnchorPosition.Center;

            var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            using (var imageFactory = new ImageFactory(true))
            {
                // Load, resize, set the format and quality and save an image.
                imageFactory.Load(soucePath)
                            .Resize(size)
                            .Quality(60)
                            .Format(format)
                            .BackgroundColor(Color.White)
                            .Save(stream);
            }
            //await stream.FlushAsync();

            return await Task.Run(() => File(stream, GetContentType(filePath), fileName));
        }

        public ActionResult UploadImages()
        {
            var r = new List<Attachment>();

            if (!Request.Files.Cast<string>().Any())
                return Json(r);

            if (!ValidateBlackListExtensions(Request))
            {
                return Json(-5);
            }

            var statuses = new List<object>();
            var headers = Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeImage(Request, statuses);
            }
            else
            {
                UploadPartialImage(headers["X-File-Name"], Request, statuses);
            }

            return JsonCamelCaseResult(statuses, JsonRequestBehavior.AllowGet);
        }

        private bool ValidateBlackListExtensions(HttpRequestBase request)
        {
            if (request.Files == null)
            {
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");
            }
            for (var i = 0; i < request.Files.Count; i++)
            {
                var ext = Path.GetExtension(request.Files[i].FileName);
                if (string.IsNullOrEmpty(ext))
                {
                    return false;
                }

                ext = ext.ToLower();

                if (GetBlackListExtensions().Any(f => f.Equals(ext)))
                {
                    return false;
                }
            }
            return true;
        }

        private void UploadWholeImage(HttpRequestBase request, IList<object> statuses)
        {
            for (var i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                if (file == null)
                    continue;

                // get extension file & get file name
                //var ext = Path.GetExtension(file.FileName) ?? "";
                var name = $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(file.FileName)}";

                // create path upload
                var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

                // create folder
                var mapPath = Server.MapPath(path);

                if (!Directory.Exists(mapPath))
                    Directory.CreateDirectory(mapPath);

                // svae file
                file.SaveAs(mapPath + name);

                //var url = GroupContentType(ext.Replace(".", "").ToLower()) == "Ảnh"
                //    ? "/Upload/Resize/" + Encryptor.Base64Encode(path + name) :
                //    "/Upload/Download/" + Encryptor.Base64Encode(path + name);

                var url = Encryptor.Base64Encode(path + name);

                // add file meta into status
                statuses.Add(new { url = url, path = (path + name) });
            }
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        private void UploadPartialImage(string fileName, HttpRequestBase request, IList<object> statuses)
        {
            if (request.Files.Count != 1 || request.Files[0] == null)
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];

            var inputStream = file.InputStream;

            // get extension file & get file name
            //var ext = Path.GetExtension(file.FileName) ?? "";
            var name = $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(file.FileName)}";

            // build path folder
            var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

            // create folder
            var mapPath = Server.MapPath(path);
            if (!Directory.Exists(mapPath))
                Directory.CreateDirectory(mapPath);

            // buidl full path on sẻver
            var fullName = Path.Combine(mapPath, Path.GetFileName(fileName) ?? "");

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }

            var url = Encryptor.Base64Encode(path + name);

            // add file meta into status
            statuses.Add(new { url = url, path = (path + name) });
        }

        [Authorize]
        [HttpPost]
        public JsonResult UploadAttachmentMessage(long id)
        {
            var statuses = new List<Attachment1>();

            if (!Request.Files.Cast<string>().Any())
                return null;

            if (!ValidateBlackListExtensions(Request))
            {
                return Json(-5);
            }

            var message = UnitOfWork.MessageRealTimeRepo.GetById(id, UserState.UserId);
            if (message == null)
            {
                return Json(-2);
            }
            if (message.SendTime.HasValue)
            {
                return Json(-2);
            }

            var headers = Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(Request, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], Request, statuses);
            }

            var attachments = new List<AttachmentMessageGetByMessageIdResults>();

            if (!statuses.Any()) return Json(attachments);

            statuses.ForEach(status =>
            {
                var attachmentId = UnitOfWork.AttachmentRepo.InsertForMessage(status.Name, status.Url, status.Ext, status.SizeByte,
                    status.SizeString, message.Id, UserState.UserId);

                if (attachmentId > 0)
                {
                    attachments.Add(new AttachmentMessageGetByMessageIdResults
                    {
                        Id = attachmentId,
                        AttachmentName = status.Name,
                        Extension = status.Ext,
                        Size = status.SizeByte,
                        SizeString = status.SizeString,
                        CreatedOnDate = DateTime.Now
                    });
                }
            });

            return Json(attachments);
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        private void UploadPartialFile(string fileName, HttpRequestBase request, ICollection<Attachment1> statuses)
        {
            if (request.Files.Count != 1 || request.Files[0] == null)
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];

            var inputStream = file.InputStream;

            // get extension file & get file name
            var ext = Path.GetExtension(file.FileName) ?? "";
            var name =
                $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(MyCommon.Ucs2Convert(file.FileName))}";

            // build path folder
            var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

            // create folder
            var mapPath = Server.MapPath(path);
            if (!Directory.Exists(mapPath))
                Directory.CreateDirectory(mapPath);

            // buidl full path on sẻver
            var fullName = Path.Combine(mapPath, Path.GetFileName(fileName) ?? "");

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }

            //var url = GroupContentType(ext.Replace(".", "").ToLower()) == "Ảnh"
            //    ? "/Upload/Resize/" + Encryptor.Base64Encode(path + name) :
            //    "/Upload/Download/" + Encryptor.Base64Encode(path + name);

            var url = Encryptor.Base64Encode(path + name);

            // add file meta into status
            statuses.Add(new Attachment1()
            {
                Name = file.FileName,
                Ext = ext.ToLower(),
                Type = GroupContentType(ext.Replace(".", "").ToLower()),
                TypeEn = GroupContentType(ext.Replace(".", "").ToLower(), false),
                SizeString = MyCommon.FormatFileSize(file.ContentLength),
                SizeByte = file.ContentLength,
                Url = url,
                UploaderId = UserState.UserId,
                UploaderName = UserState.FullName,
                Created = DateTime.Now,
            });
        }

        //DONT USE THIS IF YOU NEED TO ALLOW LARGE FILES UPLOADS
        private void UploadPartialFile(string fileName, HttpRequestBase request, ICollection<AttachmentMeta> statuses)
        {
            if (request.Files.Count != 1 || request.Files[0] == null)
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];

            var inputStream = file.InputStream;

            // get extension file & get file name
            var ext = Path.GetExtension(file.FileName) ?? "";
            var name =
                $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(MyCommon.Ucs2Convert(file.FileName))}";
            // build path folder
            var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

            // create folder
            var mapPath = Server.MapPath(path);
            if (!Directory.Exists(mapPath))
                Directory.CreateDirectory(mapPath);

            // buidl full path on sẻver
            var fullName = Path.Combine(mapPath, Path.GetFileName(MyCommon.Ucs2Convert(fileName)) ?? "");

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }

            var url = Encryptor.Base64Encode(path + name);

            // add file meta into status
            statuses.Add(new AttachmentMeta
            {
                Name = file.FileName,
                Ext = ext.ToLower(),
                Type = GroupContentType(ext.Replace(".", "").ToLower()),
                TypeEn = GroupContentType(ext.Replace(".", "").ToLower(), false),
                Size = MyCommon.FormatFileSize(file.ContentLength),
                SizeByte = file.ContentLength,
                Url = url,
                UploaderId = UserState.UserId,
                UploaderName = UserState.FullName,
                Created = DateTime.Now,
            });
        }


        private void UploadWholeFile(HttpRequestBase request, ICollection<Attachment1> statuses)
        {
            for (var i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                if (file == null)
                    continue;

                // get extension file & get file name
                var ext = Path.GetExtension(file.FileName) ?? "";
                var name =
                    $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(MyCommon.Ucs2Convert(file.FileName))}";

                // create path upload
                var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

                // create folder
                var mapPath = Server.MapPath(path);
                if (!Directory.Exists(mapPath))
                    Directory.CreateDirectory(mapPath);

                // svae file
                file.SaveAs(mapPath + name);

                //var url = GroupContentType(ext.Replace(".", "").ToLower()) == "Ảnh"
                //    ? "/Upload/Resize/" + Encryptor.Base64Encode(path + name) :
                //    "/Upload/Download/" + Encryptor.Base64Encode(path + name);

                var url = Encryptor.Base64Encode(path + name);

                // add file meta into status
                statuses.Add(new Attachment1
                {
                    Name = file.FileName,
                    Ext = ext.ToLower(),
                    Type = GroupContentType(ext.Replace(".", "").ToLower()),
                    TypeEn = GroupContentType(ext.Replace(".", "").ToLower(), false),
                    SizeString = MyCommon.FormatFileSize(file.ContentLength),
                    SizeByte = file.ContentLength,
                    Url = url,
                    UploaderId = UserState.UserId,
                    UploaderName = UserState.FullName,
                    Created = DateTime.Now,
                });
            }
        }

        public JsonResult UploadAttachment()
        {
            var r = new List<AttachmentMeta>();

            if (!Request.Files.Cast<string>().Any())
                return Json(r);

            //if (!ValidateBlackListExtensions(Request))
            //{
            //    return Json(-5);
            //}

            var statuses = new List<AttachmentMeta>();
            var headers = Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(Request, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], Request, statuses);
            }

            // Save attachment to database
            var listProview = UnitOfWork.AttachmentRepo.Insert(statuses, UserState.UserId, UserState.FullName);

            return Json(listProview);
        }

        private void UploadWholeFile(HttpRequestBase request, ICollection<AttachmentMeta> statuses)
        {
            for (var i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                if (file == null)
                    continue;

                // get extension file & get file name
                var ext = Path.GetExtension(file.FileName) ?? "";
                var name =
                    $"{DateTime.Now:ddMMyyyyssfffffff}_{Path.GetFileName(MyCommon.Ucs2Convert(file.FileName))}";

                // create path upload
                var path = $"/upload/{DateTime.Now:yyyy/MM/dd}/";

                // create folder
                var mapPath = Server.MapPath(path);
                if (!Directory.Exists(mapPath))
                    Directory.CreateDirectory(mapPath);

                // svae file
                file.SaveAs(mapPath + name);

                //var url = GroupContentType(ext.Replace(".", "").ToLower()) == "Ảnh"
                //    ? "/Upload/Resize/" + Encryptor.Base64Encode(path + name) :
                //    "/Upload/Download/" + Encryptor.Base64Encode(path + name);

                var url = Encryptor.Base64Encode(path + name);

                // add file meta into status
                statuses.Add(new AttachmentMeta
                {
                    Name = file.FileName,
                    Ext = ext.ToLower(),
                    Type = GroupContentType(ext.Replace(".", "").ToLower()),
                    TypeEn = GroupContentType(ext.Replace(".", "").ToLower(), false),
                    Size = MyCommon.FormatFileSize(file.ContentLength),
                    SizeByte = file.ContentLength,
                    Url = url,
                    UploaderId = UserState.UserId,
                    UploaderName = UserState.FullName,
                    Created = DateTime.Now,
                });
            }
        }

        private static string GroupContentType(string ext, bool isVi = true)
        {
            var listWord = new[] { "doc", "docx" };
            var listExcell = new[] { "xls", "xlsx" };
            var listPoint = new[] { "ppt", "pptx" };
            var listMusic = new[] { "mp3", "acc" };
            var listVideo = new[] { "mp4", "flv" };
            var listZip = new[] { "zip", "7zip", "rar" };
            var listCode = new[] { "html", "js" };
            var listImg = new[] { "jpg", "png", "jpeg", "gif" };

            if (listWord.Contains(ext))
            {
                return isVi ? "Tài liệu" : "Document";
            }
            if (listExcell.Contains(ext))
            {
                return isVi ? "Bảng tính" : "Spreadsheet";
            }
            if (listPoint.Contains(ext))
            {
                return isVi ? "Trình chiếu" : "Slideshow";
            }
            if (listMusic.Contains(ext))
            {
                return isVi ? "Âm thanh" : "Sound";
            }
            if (listVideo.Contains(ext))
            {
                return isVi ? "Picture" : "Picture";
            }
            if (listZip.Contains(ext))
            {
                return isVi ? "yuanp nén" : "File";
            }
            if (listCode.Contains(ext))
            {
                return isVi ? "Mã" : "Code";
            }
            if (listImg.Contains(ext))
            {
                return isVi ? "Ảnh" : "Image";
            }

            return "";
        }

        // Check allow thumb size.
        private static bool AllowThumbSize(int width, int height)
        {
            var size = $"{width}x{height}".ToLower();
            var thumbs = ConfigurationManager.AppSettings["THUMBNAIL_SIZES"];
            if (string.IsNullOrEmpty(thumbs)) return true;

            var t = thumbs.ToLower().Split(new[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

            return t.Any(s1 => s1.Contains(size));
        }

        private static string GetContentType(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return "Image/bmp";
                case ".gif": return "Image/gif";
                case ".jpg": return "Image/jpeg";
                case ".png": return "Image/png";
                case ".tif": return "Image/tif";
                case ".webp": return "Image/webp";
                default: return "application/octet-stream";
            }
        }
    }
}