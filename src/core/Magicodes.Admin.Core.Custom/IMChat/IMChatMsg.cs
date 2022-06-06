using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.IMChat
{
    public class IMChatMsg : EntityBase<int>
    {
        /// <summary>
        /// 会话窗口Id (客户端用户Id)
        /// </summary>
        [Display(Name = "会话窗口Id")]
        public long? ClientChatId { get; set; }
        /// <summary>
        /// 消息所有者身份：sys 系统消息；client 会话消息
        /// </summary>
        [Display(Name = "消息所有者身份")]
        [MaxLength(20)]
        public string Role { get; set; }
        /// <summary>
        /// 消息类型：文本、文件
        /// </summary>
        [Display(Name = "消息类型")]
        [MaxLength(20)]
        public string ContentType { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        [Display(Name = "消息内容")]
        [MaxLength(500)]
        public string Content { get; set; }
        /// <summary>
        /// 是否有新消息
        /// </summary>
        [Display(Name = "是否有新消息")]
        public bool? IsNewMsg { get; set; }
    }
}
