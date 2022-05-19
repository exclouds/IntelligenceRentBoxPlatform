using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Business
{
    /// <summary>
    /// 信息确认
    /// </summary>
    [Display(Name = "信息确认")]
    public class BusinessConfirm : EntityBase<int>
    {
        #region 箱东和租客单号不可同时为空
        /// <summary>
        /// 箱东单号（当箱东单号为空时可用用户Id替代）
        /// </summary>
        [Display(Name = "箱东单号")]
        [MaxLength(50)]
        public string BoxInfoBillNO { get; set; }
        /// <summary>
        /// 箱东用户Id
        /// </summary>
        [Display(Name = "箱东用户Id")]
        public int BoxId { get; set; }

        /// <summary>
        /// 租客单号(租客Id)
        /// </summary>
        [Display(Name = "租客单号")]
        [MaxLength(50)]
        public string TenantInfoBillNO { get; set; }

        /// <summary>
        /// 租客Id
        /// </summary>
        [Display(Name = "租客Id")]
        public string TenantInfoId { get; set; }
        #endregion

        /// <summary>
        /// 租客保证金
        /// </summary>
        [Display(Name = "租客保证金")]

        public decimal TenantMargin { get; set; }
        /// <summary>
        /// 实际卖价
        /// </summary>
        [Display(Name = "实际卖价")]

        public decimal SellingPrice { get; set; }

        /// <summary>
        /// 实际买价
        /// </summary>
        [Display(Name = "实际买价")]

        public decimal PurchasePrice { get; set; }


    }
}
