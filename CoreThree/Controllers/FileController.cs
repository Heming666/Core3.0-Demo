using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using CoreThree.Models;
using IService;
using Microsoft.AspNetCore.StaticFiles;
using System.Web;
using Microsoft.AspNetCore.Http;
using NLog;

namespace CoreThree.Controllers
{
    public class FileController : Controller
    {
        private readonly Logger _logger;
        private readonly string _path;
        public FileController(IUserInfo userInfo, IDearptment dept)
        {
            _path = "F:\\迅雷下载";
            _logger = LogManager.GetCurrentClassLogger();
        }
        public IActionResult Index()
        {

            List<FileModel> videoList;
            var filePaths = Directory.GetFiles(_path);
            videoList = GetAllVideo(filePaths);
            return View(videoList);
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetFile(string filePath)
        {
            try
            {
                #region 分片下载
                string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

                int bufferSize = 1024;//这就是ASP.NET Core循环读取下载文件的缓存大小，这里我们设置为了1024字节，也就是说ASP.NET Core每次会从下载文件中读取1024字节的内容到服务器内存中，然后发送到客户端浏览器，这样避免了一次将整个下载文件都加载到服务器内存中，导致服务器崩溃

                Response.ContentType = await GetMIMEAsync(filePath);//由于我们下载的是一个Excel文件，所以设置ContentType为application/vnd.ms-excel

                var contentDisposition = "attachment;" + "filename=" + HttpUtility.UrlEncode(fileName);//在Response的Header中设置下载文件的文件名，这样客户端浏览器才能正确显示下载的文件名，注意这里要用HttpUtility.UrlEncode编码文件名，否则有些浏览器可能会显示乱码文件名
                Response.Headers.Add("Content-Disposition", new string[] { contentDisposition });

                //使用FileStream开始循环读取要下载文件的内容
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (Response.Body)//调用Response.Body.Dispose()并不会关闭客户端浏览器到ASP.NET Core服务器的连接，之后还可以继续往Response.Body中写入数据
                    {
                        long contentLength = fs.Length;//获取下载文件的大小
                        Response.ContentLength = contentLength;//在Response的Header中设置下载文件的大小，这样客户端浏览器才能正确显示下载的进度

                        byte[] buffer;
                        long hasRead = 0;//变量hasRead用于记录已经发送了多少字节的数据到客户端浏览器

                        //如果hasRead小于contentLength，说明下载文件还没读取完毕，继续循环读取下载文件的内容，并发送到客户端浏览器
                        while (hasRead < contentLength)
                        {
                            //HttpContext.RequestAborted.IsCancellationRequested可用于检测客户端浏览器和ASP.NET Core服务器之间的连接状态，如果HttpContext.RequestAborted.IsCancellationRequested返回true，说明客户端浏览器中断了连接
                            if (HttpContext.RequestAborted.IsCancellationRequested)
                            {
                                //如果客户端浏览器中断了到ASP.NET Core服务器的连接，这里应该立刻break，取消下载文件的读取和发送，避免服务器耗费资源
                                break;
                            }

                            buffer = new byte[bufferSize];

                            int currentRead =await fs.ReadAsync(buffer, 0, bufferSize);//从下载文件中读取bufferSize(1024字节)大小的内容到服务器内存中

                            await Response.Body.WriteAsync(buffer, 0, currentRead);//发送读取的内容数据到客户端浏览器
                            //Request.Body.Position = 0;
                            //Response.Body.Flush();//注意每次Write后，要及时调用Flush方法，及时释放服务器内存空间
                            hasRead += currentRead;//更新已经发送到客户端浏览器的字节数
                        }
                        await Response.Body.FlushAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
                #endregion
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var data = Request.Form.Files["data"];
            string lastModified = Request.Form["lastModified"].ToString();
            var total = Request.Form["total"];
            var fileName = Request.Form["fileName"];
            var index = Request.Form["index"].ToString();

            string temporary = Path.Combine(_path, lastModified);//临时保存分块的目录
            try
            {
                if (!Directory.Exists(temporary))
                    Directory.CreateDirectory(temporary);
                string filePath = Path.Combine(temporary, index.ToString());
                if (!Convert.IsDBNull(data))
                {
                    await Task.Run(() =>
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        {
                            data.CopyTo(fs);
                        }
                    });
                }
                bool mergeOk = false;
                if (total == index)
                {
                    mergeOk = await FileMerge(lastModified, fileName);
                }

                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("number", index);
                result.Add("mergeOk", mergeOk);
                return Json(result);

            }
            catch (Exception ex)
            {
                Directory.Delete(temporary);//删除文件夹
                throw ex;
            }
        }

        public async Task<bool> FileMerge(string lastModified, string fileName)
        {
            return await Task.Run(() =>
             {
                 bool ok = false;
                 try
                 {
                     var temporary = Path.Combine(_path, lastModified);//临时文件夹
                    fileName = Request.Form["fileName"];//文件名
                    string fileExt = Path.GetExtension(fileName);//获取文件后缀
                    var files = Directory.GetFiles(temporary);//获得下面的所有文件
                    var finalPath = Path.Combine(_path, fileName.Substring(fileName.LastIndexOf('\\') + 1));//最终的文件名（demo中保存的是它上传时候的文件名，实际操作肯定不能这样）
                    var fs = new FileStream(finalPath, FileMode.Create);
                     foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                    {
                         var bytes = System.IO.File.ReadAllBytes(part);
                         fs.Write(bytes, 0, bytes.Length);
                         bytes = null;
                         System.IO.File.Delete(part);//删除分块
                    }
                     fs.Close();
                     fs.DisposeAsync();
                     Directory.Delete(temporary);//删除文件夹
                    ok = true;
                 }
                 catch (Exception ex)
                 {
                     throw ex;
                 }
                 return ok;
             });
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Task<FileModel> GetFileInfo(string path)
        {
            return Task.Run(() =>
             {
                 using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1, true))
                 {
                     double size = stream.Length;
                     FileModel model = new FileModel()
                     {
                         FileName = path.Substring(path.LastIndexOf('\\') + 1),
                         FilePath = path,
                         FileType = path.Substring(path.LastIndexOf('.') + 1),
                         CreateTime = Directory.GetCreationTime(path)
                     };
                     if (size < 1024)
                     {
                         model.FileSize = size;
                         model.Unit = Unit.B;
                     }
                     else if (size / 1024 < 1024)
                     {
                         model.FileSize = size / 1024;
                         model.Unit = Unit.KB;
                     }
                     else if (size / (1024 * 1024) < 1024)
                     {
                         model.FileSize = size / (1024 * 1024);
                         model.Unit = Unit.MB;
                     }
                     else if (size / (1024 * 1024 * 1024) < 1024)
                     {
                         model.FileSize = size / (1024 * 1024 * 1024);
                         model.Unit = Unit.G;
                     }
                     else
                     {
                         model.FileSize = size;
                         model.Unit = Unit.B;
                     }
                     model.FileSize = Math.Round(model.FileSize, 2);
                     return model;
                 }
             });
        }

        /// <summary>
        /// 获取文件的MIME类型
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private   async Task<string> GetMIMEAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                string mimeType = "application/unknown";
                if (!new FileExtensionContentTypeProvider().TryGetContentType(filePath, out mimeType))
                {
                    switch (filePath.Substring(filePath.LastIndexOf('.') + 1).ToLower())
                    {
                        case "rmvb":
                            mimeType = "audio/x-pn-realaudio";
                            break;
                        default:
                            break;
                    }
                }
                return mimeType;
            });
        }

        /// <summary>
        /// 获取所有的视频文件
        /// </summary>
        /// <param name="filePaths">文件地址</param>
        /// <returns></returns>
        private List<FileModel> GetAllVideo(string[] filePaths)
        {
            List<FileModel> list = new List<FileModel>();
            foreach (var item in filePaths)
            {
                list.Add(GetFileInfo(item).Result);
            }
            var videoList = list.Where(x => "mp4,avi,rmvb,flv".Contains(x.FileType)).OrderByDescending(p => p.CreateTime).ToList();
            return videoList;
        }
    }
}