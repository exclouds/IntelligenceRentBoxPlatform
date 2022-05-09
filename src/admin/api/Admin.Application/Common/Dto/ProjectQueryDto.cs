using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Common.Dto
{
    public class ProjectQueryDto
    {
        /// <summary>
        /// 是否显示父级
        /// </summary>
        public bool? IsShowParent { get; set; }
        /// <summary>
        /// 预算类别
        /// </summary>   
        public string BudgetType { get; set; }
        /// <summary>
        /// 特殊费用维护是否显示
        /// </summary>
        public bool? IsSpecialFee { get; set; }
        /// <summary>
        /// 业务量是否显示
        /// </summary>
        public bool? IsBusiness { get; set; }
        /// <summary>
        /// 合资公司经营报表是否显示
        /// </summary>
        public bool? IsReport { get; set; }
    }
}
