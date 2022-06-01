using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Basis
{
    /// <summary>
    /// 站点
    /// </summary>
    [Display(Name = "站点")]
    public class SiteTable: EntityBase<int>
    {
        /// <summary>
        /// 站点代码
        /// </summary>
        [Display(Name = "站点代码")]
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        [Display(Name = "站点名称")]
        [Required]
        [MaxLength(100)]
        public string SiteName { get; set; }

        /// <summary>
        /// 英文站点名称
        /// </summary>
        [Display(Name = "英文站点名称")]
        [MaxLength(100)]
        public string ENSiteName { get; set; }



        /// <summary>
        /// 国家代码
        /// </summary>
        [Display(Name = "国家代码")]
        [MaxLength(50)]
        public string CountryCode { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>

        [Display(Name = "是否启用")]
        public bool IsEnable { get; set; }
    }
}
