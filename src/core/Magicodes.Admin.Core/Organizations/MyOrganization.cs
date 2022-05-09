using Abp.Organizations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Magicodes.Admin.Organizations
{
    public class MyOrganization : OrganizationUnit
    {
        /// <summary>
        /// 父级公司编码
        /// </summary>
        [Display(Name = "简称父级公司编码")]
        [MaxLength(50)]
        public string ParentCode { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        [Display(Name = "简称")]
        [MaxLength(100)]
        public string ShortName { get; set; }

        /// <summary>
        /// 营业执照存储路径
        /// </summary>
        [Display(Name = "营业执照存储路径")]
        [MaxLength(200)]
        public string BusinessLicense { get; set; }

        /// <summary>
        /// 公司类型(1:平台；2：箱东；3：租客)
        /// </summary>
        [Display(Name = "公司类型")]
        public int CompanyType { get; set; }
    }
}
