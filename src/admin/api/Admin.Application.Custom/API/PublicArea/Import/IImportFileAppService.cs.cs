using Abp.Application.Services;
using Admin.Application.Custom.API.PublicArea.Import.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.PublicArea.Import
{
    public interface IImportFileAppService : IApplicationService
    {
        // <summary>
        /// 附件上传返回文件上传路径
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        Task<ReturnResult> AnnexImportUpload([FromForm] IFormCollection files);
    }
}
