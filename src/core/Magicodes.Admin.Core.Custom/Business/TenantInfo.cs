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
        /// 所属路线
        /// </summary>
        [Display(Name = "所属路线")]
        [MaxLength(50)]
        public string Line { get; set; }

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
        
    }
}
