using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Admin.Application.Custom.API.PublicArea.Annex.Dto;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Admin.Application.Custom.API.PublicArea.Annex
{
    class AttachmentAppService : AppServiceBase
    {
        private readonly IHostingEnvironment _hosting;
        private readonly IRepository<AttachmentInfo, long> _AttachmentInfoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AttachmentAppService(IHostingEnvironment hosting,
            IRepository<AttachmentInfo, long> AttachmentInfoRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _hosting = hosting;
            _AttachmentInfoRepository = AttachmentInfoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        #region 上传附件
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="type"></param>
        /// <param name="billno"></param>
        /// <returns></returns>
        public async Task AnnexUploaFile([FromForm] IFormCollection files, string type, string billno,string id)
        {          
            FormFileCollection filelist = (FormFileCollection)files.Files;

            // string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            String path = type + "/" + billno + "/";

            string FilePath = _hosting.WebRootPath + "/" + path;//取根路径

            DirectoryInfo di = new DirectoryInfo(FilePath);
            if (!di.Exists)
            {
                di.Create();
            }
            foreach (var file in filelist)
            {
                using (FileStream fs = System.IO.File.Create(FilePath + file.FileName))
                {
                    // 复制文件
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();


                    string filepath = path + file.FileName;
                    AttachmentInfo filemodel = new AttachmentInfo
                    {
                        CreationTime = DateTime.Now,
                        CreatorUserId = AbpSession.UserId,
                        Name = file.Name,
                        ContentType = file.ContentType,
                        FileLength = file.Length,
                        Url = filepath,
                        BlobName = billno.Trim().ToUpper(),
                        ContainerName = id
                    };
                    _AttachmentInfoRepository.Insert(filemodel);
                }



            }



        }

        #endregion

        #region 获取文件列表
        public List<FileInfoModel> GetUPFile(string Id)
        {
            var url = _httpContextAccessor.HttpContext.Request.Host;//取后台Url路径
            var allfile = _AttachmentInfoRepository.GetAll().Where(p => p.ContainerName == Id.ToString())
                .Select(p => new FileInfoModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Url = "http://" + url.Value + "/" + p.Url
                }).ToList();

            return allfile;
        }
        #endregion

        #region 批量删除附件
        /// <summary>
        /// 批量删除附件
        /// </summary>
        /// <param name="id"></param>
        public void BathDeltefile(List<long> id)
        {
            var infomodel = _AttachmentInfoRepository.GetAll().Where(p => id.Contains(p.Id)).ToList();

            foreach (var item in infomodel)
            {
                item.IsDeleted = true;
                item.DeleterUserId = AbpSession.UserId;
                item.DeletionTime = DateTime.Now;
            }
        }
       
        #endregion
    }
}
