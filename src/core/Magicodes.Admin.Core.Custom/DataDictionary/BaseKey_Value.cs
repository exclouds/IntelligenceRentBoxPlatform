using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.DataDictionary
{
    /// <summary>
    /// 键值对对照表
    /// </summary>
    [Display(Name = "键值对对照表")]
    public class BaseKey_Value : EntityBase<long>
    {
        /// <summary>
        /// 类型对照表
        /// </summary>
        [Display(Name = "类型对照表")]
        [MaxLength(20)]
        public string BaseKey_ValueTypeCode { get; set; }
        /// <summary>
        /// 键
        /// </summary>
        [Display(Name = "键")]
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [Display(Name = "值")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 系统参数
        /// </summary>
        [Display(Name = "系统参数")]
        public bool? SystemSetting { get; set; } = false;

    }
}
