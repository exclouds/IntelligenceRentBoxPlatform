using Abp.Application.Services;
using Admin.Application.Custom.API.PublicArea.SendMail.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.PublicArea.SendMail
{
    public interface ISendMailService: IApplicationService
    {
        /// <summary>
        /// 邮件发送(无配置附件)
        /// </summary>  
        /// <param name="dto">参数</param>      
        String GetSendmailby(SendMailDto dto);
    }
}
