using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.PublicArea.PubContactNO
{
    public interface IContactNOAPPService: IApplicationService
    {
        /// <summary>
        /// 单号生成
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        Task<string> GetBusNO(string type);
    }
}
