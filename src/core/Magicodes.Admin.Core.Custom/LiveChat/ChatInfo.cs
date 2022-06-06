using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.LiveChat
{
    /// <summary>
    /// 聊天信息
    /// </summary>
    [Display(Name = "聊天信息")]
    public class ChatInfo: EntityBase<int>
    {
        /// <summary>
        /// 信息发送发
        /// </summary>
        [Display(Name = "信息发送发")]
        [Required]
        public int SendUserId { get; set; }

        /// <summary>
        /// 信息接收
        /// </summary>
        [Display(Name = "信息接收")]
        [Required]
        public int AcceptUserId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        [Display(Name = "消息内容")]
        [Required]
        [MaxLength(500)]
        public string Message { get; set; }
        /// <summary>
        /// 是否阅读
        /// </summary>
        [Display(Name = "是否阅读")]
        public bool IsRead { get; set; }
        /// <summary>
        /// 阅读时间
        /// </summary>
        [Display(Name = "阅读时间")]
        public DateTime? ReadTime { get; set; }
        /// <summary>
        /// 是否关闭会话（1分钟自动关闭）
        /// </summary>
        [Display(Name = "是否关闭会话")]
        public bool IsClose { get; set; }
    }
}
