using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.IMChat
{
    /// <summary>
    /// 会话窗口信息
    /// </summary>
    [Display(Name = "会话窗口信息")]
    public class IMClientEn : EntityBase<int>
    {
        /// <summary>
        /// 会话窗口Id (客户端用户Id)
        /// </summary>
        [Display(Name = "会话窗口Id")]
        public long? ClientChatId { get; set; }
        /// <summary>
        /// 客户端用户名称
        /// </summary>
        [Display(Name = "客户端用户名称")]
        [MaxLength(20)]
        public string ClientChatName { get; set; }
        /// <summary>
        /// 客服Id (平台用户Id)
        /// </summary>
        [Display(Name = "客服Id")]
        public long? ServerChatId { get; set; }
        /// <summary>
        /// 在线状态：on 在线；off离线
        /// </summary>
        [Display(Name = "在线状态")]
        [MaxLength(20)]
        public string State { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        [Display(Name = "访问时间")]
        public DateTime? AccessTime { get; set; }
        /// <summary>
        /// 单号（箱东单号、租客单号）
        /// </summary>
        [Display(Name = "单号")]
        [MaxLength(50)]
        public string BillNO { get; set; }
        /// <summary>
        /// 会话框内容
        /// </summary>
        [Display(Name = "会话框内容")]
        [MaxLength(500)]
        public string InputContent { get; set; }
        /// <summary>
        /// 新消息数量
        /// </summary>
        [Display(Name = "新消息数量")]
        public int? NewMsgCount { get; set; }
        /// <summary>
        /// 是否关注（已关注的排在最前面并按最后一次时间倒序）
        /// </summary>
        [Display(Name = "是否关注")]
        public bool? IsFollow { get; set; }
        /// <summary>
        /// 最新一条消息时间
        /// </summary>
        [Display(Name = "最新一条消息时间")]
        public DateTime? LastMsgTime { get; set; }
        /// <summary>
        /// 最新一条消息
        /// </summary>
        [Display(Name = "最新一条消息")]
        [MaxLength(500)]
        public string LastMsgContent { get; set; }
        /// <summary>
        /// 最后一个消息的显示时间
        /// </summary>
        [Display(Name = "最后一个消息的显示时间")]
        public DateTime? LastMsgShowTime { get; set; }
    }
}
