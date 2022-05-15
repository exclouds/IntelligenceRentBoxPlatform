using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Magicodes.Admin;
using Magicodes.Admin.Attachments;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Admin.Application.Custom.API.PublicArea.Annex
{
    [AbpAllowAnonymous] 
    public class AnnexAppService : AppServiceBase
    {
        private readonly IHostingEnvironment _hosting;
        private readonly IRepository<AttachmentInfo, long> _AttachmentInfoRepository;
        
        public AnnexAppService(IHostingEnvironment hosting,
            IRepository<AttachmentInfo, long> AttachmentInfoRepository)
        {
            _hosting = hosting;
            _AttachmentInfoRepository = AttachmentInfoRepository;
        }
        /// <summary>
        /// 公司注册上传营业执照
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<string> AnnexUpload([FromForm] IFormCollection files)
        {
            FormFileCollection filelist = (FormFileCollection)files.Files;


            string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            String path = "GSZC/" + datetime + "/";

            string FilePath = _hosting.WebRootPath + "/" + path + "/";//取根路径

            DirectoryInfo di = new DirectoryInfo(FilePath);
            if (!di.Exists)
            {
                di.Create();
            }
            var file = filelist[0];
            using (FileStream fs = System.IO.File.Create(FilePath + file.FileName))
            {
                // 复制文件
                file.CopyTo(fs);
                // 清空缓冲区数据
                fs.Flush();

            }

            return path + file.FileName;
        }
      
    }
}
