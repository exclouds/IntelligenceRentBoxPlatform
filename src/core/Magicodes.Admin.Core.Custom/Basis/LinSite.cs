using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Basis
{
    /// <summary>
    /// 航线站点
    /// </summary>
    [Display(Name = "航线站点")]
    public class LinSite : EntityBase<int>
    {
        /*
        如出现两个站点对应多个线路前台可选 
        */

        /// <summary>
        /// 站点代码
        /// </summary>
        [Display(Name = "站点代码")]
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 航线
        /// </summary>
        [Display(Name = "航线")]
        [Required]
        public string LineId { get; set; }
    }
}
