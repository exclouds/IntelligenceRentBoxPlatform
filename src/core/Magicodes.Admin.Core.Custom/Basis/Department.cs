using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Basis
{
    /// <summary>
    /// 部门信息
    /// </summary>
    [Display(Name = "部门信息")]
    public class Department : EntityBase<int>
    {
        /// <summary>
        /// 公司编码
        /// </summary>
        [Display(Name = "公司编码")]
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [Display(Name = "部门编码")]
        [Required]
        [MaxLength(50)]
        public string DepCode { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Display(Name = "部门名称")]
        [Required]
        [MaxLength(50)]
        public string DepName { get; set; }
    }
}
