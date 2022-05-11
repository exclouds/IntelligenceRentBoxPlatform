using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Business
{
    /// <summary>
    /// 租客
    /// </summary>
    [Display(Name = "租客")]
    public class TenantInfo: EntityBase<int>
    {
        /// <summary>
        /// 单号（租客首字母ZK）
        /// </summary>
        [Display(Name = "单号")]
        [Required]
        [MaxLength(50)]
        public string BillNO { get; set; }

        /// <summary>
        /// 起运站
        /// </summary>
        [Display(Name = "起运站")]
        [Required]
        [MaxLength(50)]
        public string StartStation { get; set; }

        /// <summary>
        /// 目的站
        /// </summary>
        [Display(Name = "目的站")]
        [Required]
        [MaxLength(50)]
        public string EndStation { get; set; }

        /// <summary>
        /// 所属路线（可根据站点自动关联，可选）
        /// </summary>
        [Display(Name = "所属路线")]
        [MaxLength(50)]
        public int? Line { get; set; }

        /// <summary>
        /// 有效时间起
        /// </summary>
        [Display(Name = "有效时间起")]
        [Required]
        public DateTime EffectiveSTime { get; set; }

        /// <summary>
        /// 有效时间止
        /// </summary>
        [Display(Name = "有效时间止")]
        [Required]
        public DateTime EffectiveETime { get; set; }
        /// <summary>
        /// 期望成交价
        /// </summary>
        [Display(Name = "期望成交价")]
        public decimal HopePrice { get; set; }

        /// <summary>
        /// 是否审核
        /// </summary>
        [Display(Name = "是否审核")]
        public bool IsVerify { get; set; }

        /// <summary>
        /// 审核评语
        /// </summary>
        [Display(Name = "审核评语")]
        [MaxLength(500)]
        public string VerifyRem { get; set; }

        /// <summary>
        /// 询价次数
        /// </summary>
        [Display(Name = "询价次数")]
        public int? InquiryNum { get; set; }

        /// <summary>
        /// 单据是否完成（界面打完成标记）
        /// </summary>
        [Display(Name = "单据是否完成")]
        public bool Finish { get; set; }

    }
}
