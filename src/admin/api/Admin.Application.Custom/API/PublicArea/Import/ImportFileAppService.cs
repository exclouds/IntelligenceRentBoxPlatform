using Abp.Auditing;
using Admin.Application.Custom.API.PublicArea.Import.Dto;
using Magicodes.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;



namespace Admin.Application.Custom.API.PublicArea.Import
{
    public class ImportFileAppService : AppServiceBase, IImportFileAppService
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        public ImportFileAppService(
            IHostingEnvironment hostingEnvironment
            )
        {
            _hostingEnvironment = hostingEnvironment;

        }
        


        public async Task<ReturnResult> AnnexImportUpload([FromForm] IFormCollection files)
        {
            FormFileCollection filelist = (FormFileCollection)files.Files;
            string FilePath = "";
            ReturnResult re = new ReturnResult();
            foreach (IFormFile file in filelist)
            {

                string webRootPath = _hostingEnvironment.WebRootPath;
                if (!Directory.Exists(webRootPath + "/Import/"))
                {
                    Directory.CreateDirectory(webRootPath + "/Import/");
                }
                //路径
                FilePath = webRootPath + "/Import/";
                string[] name = file.FileName.Split('.');
                DirectoryInfo di = new DirectoryInfo(FilePath);
                if (!di.Exists)
                {
                    di.Create();
                }
                using (FileStream fs = System.IO.File.Create(FilePath + file.FileName))
                {

                    // 复制文件
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }
                re.name = file.FileName;
                re.url = FilePath + file.FileName;
            }
            return re;
        }

        ///// <summary>
        ///// 将没有扩展名的文件从后台上传
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("AddFileExtension")]
        //[DisableAuditing]
        //public async Task<ReturnResult> AddFileExtension()
        //{
        //    try
        //    {
        //        var file = Request.Form.Files["file"];
        //        StreamReader reader = new StreamReader(file.OpenReadStream());
        //        //读取文件内容
        //        string content = reader.ReadToEnd();
        //        //创建新文件并写入内容
        //        string webRootPath = _hostingEnvironment.WebRootPath;
        //        if (!Directory.Exists(webRootPath + "/upload/"))
        //        {
        //            Directory.CreateDirectory(webRootPath + "/upload/");
        //        }
        //        string localfile = webRootPath + "/upload/" + file.FileName + ".txt";
        //        FileStream fs1 = new FileStream(localfile, FileMode.Create, FileAccess.Write);
        //        StreamWriter sw1 = new StreamWriter(fs1);
        //        sw1.WriteLine(content);
        //        sw1.Close();
        //        fs1.Close();
        //        //将新文件上传到服务器
        //        String uriString = localfile;
        //        // 创建一个新的 WebClient 实例.
        //        WebClient myWebClient = new WebClient();
        //        // 直接上传，并获取返回的二进制数据.
        //        byte[] responseArray = myWebClient.UploadFile(uriString, "POST", localfile);
        //        string str = Encoding.UTF8.GetString(responseArray);
        //        //Dto.FileResult objectList = JsonConvert.DeserializeObject<Dto.FileResult>(str);
        //        //ReturnResult re = new ReturnResult();
        //        //re.response = objectList;
        //        re.name = file.FileName;
        //        System.IO.File.Delete(@localfile);
        //        return re;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}
    }
}
