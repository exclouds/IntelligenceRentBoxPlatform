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
        [Display(Name = "站点代码")]
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Display(Name = "站点名称")]
        [Required]
        [MaxLength(100)]
        public string SiteName { get; set; }

        [Display(Name = "国家")]
        [Required]
        [MaxLength(50)]
        public string Country { get; set; }


        [Display(Name = "是否启用")]
        public bool IsEnable { get; set; }
    }
}
