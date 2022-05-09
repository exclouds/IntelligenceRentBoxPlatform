using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.DataDictionary
{
    /// <summary>
    /// 键值对类型对照表
    /// </summary>
    [Display(Name = "键值对类型对照表")]
    public class BaseKey_ValueType : EntityBase<int>
    {
        /// <summary>
        /// 类型代码
        /// </summary>
        [Display(Name = "类型代码")]
        [Required]
        [MaxLength(20)]
        public string TypeCode { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        [Display(Name = "类型名称")]
        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; }
        /// <summary>
        /// 父级Code
        /// </summary>
        [Display(Name = "父级Code")]
        [MaxLength(20)]
        public string ParentCode { get; set; }
        /// <summary>
        /// 系统参数
        /// </summary>
        [Display(Name = "系统参数")]
        public bool? SystemSetting { get; set; } = false;
    }
}
