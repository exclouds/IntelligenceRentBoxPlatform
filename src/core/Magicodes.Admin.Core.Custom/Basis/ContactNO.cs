using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Basis
{
    /// <summary>
    /// 业务提单号生成规则对照表
    /// </summary>
    [Display(Name = "业务提单号生成规则对照表")]
    public class ContactNO : EntityBase<int>
    {

        /// <summary>
        /// 单号
        /// </summary>
        [Display(Name = "单号")]
        [Required]
        [MaxLength(50)]
        public string ConNo { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        [MaxLength(5)]
        public string Type { get; set; }
    }
}
