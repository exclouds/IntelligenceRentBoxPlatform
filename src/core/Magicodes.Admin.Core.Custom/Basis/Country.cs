using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Basis
{
    /// <summary>
    /// 国家
    /// </summary>
    [Display(Name = "国家")]
    public class Country : EntityBase<int>
    {
        /// <summary>
        /// 国家代码
        /// </summary>
        [Display(Name = "国家代码")]
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        public bool IsEnable { get; set; }
    }
}
