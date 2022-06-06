using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.IMChat
{
    /// <summary>
    /// 客户上线记录
    /// </summary>
    [Display(Name = "客户上线记录")]
    public class IMServerEn : EntityBase<int>
    {
        /// <summary>
        /// 客服Id (平台用户Id)
        /// </summary>
        [Display(Name = "客服Id")]
        public long? ServerChatId { get; set; }
        /// <summary>
        /// 平台用户名称
        /// </summary>
        [Display(Name = "平台用户名称")]
        [MaxLength(20)]
        public string ServerChatName { get; set; }
        /// <summary>
        /// 在线状态 ：on 在线；off离线
        /// </summary>
        [Display(Name = "在线状态")]
        public string State { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        [Display(Name = "访问时间")]
        public DateTime? AccessTime { get; set; }
    }
}
