using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Basis
{
    /// <summary>
    /// 路线
    /// </summary>
    [Display(Name = "路线")]
    public class Line: EntityBase<int>
    {
        /// <summary>
        /// 航线名称
        /// </summary>
        [Display(Name = "航线名称")]
        [MaxLength(50)]
        public string LineName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        public bool IsEnable { get; set; } = true;
    }
}
