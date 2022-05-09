using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Organizations.Dto
{
    public class UpdateDept
    {
        public int Id { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [Display(Name = "公司编码")]
        public string Code { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [Display(Name = "部门编码")]
        public string DepCode { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Display(Name = "部门名称")]
        public string DepName { get; set; }
    }
}
