using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Organizations.Dto
{
    public class ComDepartmentDto : EntityBase<int>
    {
        /// <summary>
        /// 公司编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DepCode { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepName { get; set; }

        /// <summary>
        ///部门归属
        /// </summary>
        public string DepNCCName { get; set; }
        
        public string parentId { get; set; }
        public string parentName { get; set; }
        public DateTime? CreationTime { get; set; }
    }
    public class CompanyDto : EntityBase<long>
    {
        public string DisplayName { get; set; }
        public long? ParentId { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// 对应上级code
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// 上级公司名称
        /// </summary>
        public string parentName { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; set; }
        
    }
}
