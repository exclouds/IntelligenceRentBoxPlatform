using Abp.UI;
using Admin.Application.Custom.API.PublicArea.SendMail.Dto;
using Magicodes.Admin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Admin.Application.Custom.API.PublicArea.SendMail
{
    public static class SendMailService
    {
        // <summary>
        /// 邮件发送(无配置附件)
        /// </summary>  
        /// <param name="dto">参数</param>      
        public static String GetSendmailby(SendMailDto dto)
        {
            string msg = "";//返回结果
            try
            {
                //发件服务器
                String smtp = ConfigurationManager.AppSettings["Smtp"].ToString();
                SmtpClient client = new SmtpClient(smtp);

                //发件人邮箱账号和密码
                String SendUser = ConfigurationManager.AppSettings["SendUser"].ToString();
                String SendUserPSW = ConfigurationManager.AppSettings["SendUserPSW"].ToString();
                client.Credentials = new System.Net.NetworkCredential(SendUser, SendUserPSW);
                //   client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis;


                if (string.IsNullOrEmpty(dto.ReciptUser))
                {
                    return "收件人不能为空！！";
                }
                else
                {

                    MailMessage message = new MailMessage(SendUser, dto.ReciptUser, dto.Title, dto.MailBody);

                    //判断是否有附件，多个附件以英文;隔开
                    if (!string.IsNullOrEmpty(dto.file))
                    {
                        string[] path = dto.file.Split(';');
                        Attachment data;

                        for (int i = 0; i < path.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(path[i]))
                            {
                                data = new Attachment(path[i], MediaTypeNames.Application.Octet);
                                message.Attachments.Add(data);//添加到附件中 
                            }

                        }
                    }
                    client.Send(message);
                    msg = "发送成功!";
                }
                return msg;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(3000, "邮件发送失败！");
            }

        }
    }
}
