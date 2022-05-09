using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.PublicArea.SendMail.Dto
{
    public class SendMailDto
    {
        /// <summary>
        /// 收件人邮箱,多个收件人用英文逗号隔开
        /// </summary>
        public String ReciptUser { get; set; }
        /// <summary>
        /// 邮件主题
        /// </summary>
        public String Title { get; set; }
        /// <summary>
        /// 邮件正文
        /// </summary>
        public String MailBody { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public String file { get; set; }
    }
}
