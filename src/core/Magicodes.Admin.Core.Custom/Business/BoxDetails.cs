using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Core.Custom.Business
{
    /// <summary>
    /// 集装箱信息
    /// </summary>
    [Display(Name = "集装箱信息")]
    public class BoxDetails : EntityBase<int>
    {
        /// <summary>
        /// 箱东信息
        /// </summary>
        [Display(Name = "箱东信息")]
        [Required]
        [MaxLength(50)]
        public string BoxTenantInfoNO { get; set; }


        /// <summary>
        /// 箱型
        /// </summary>
        [Display(Name = "箱型")]
        [Required]
        [MaxLength(50)]
        public string Box { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        [Display(Name = "尺寸")]
        [Required]
        [MaxLength(50)]
        public string Size { get; set; }

        /// <summary>
        /// 箱量
        /// </summary>
        [Display(Name = "箱量")]
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// 预计到站时间(在途箱时间必填)
        /// </summary>
        [Display(Name = "预计到站时间")]
        public DateTime? PredictTime { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        [Display(Name = "箱号")]
        [MaxLength(50)]
        public string BoxNO { get; set; }
        /// <summary>
        /// 箱龄
        /// </summary>
        [Display(Name = "箱龄")]
        public double? BoxAge { get; set; }

        /// <summary>
        /// 是否审核
        /// </summary>
        [Display(Name = "是否审核")]
        public bool IsVerify { get; set; }
        /// <summary>
        /// 最大载重
        /// </summary>
        [Display(Name = "最大载重")]
        public double? MaxWeight { get; set; }
        /// <summary>
        /// 冻柜型号
        /// </summary>
        [Display(Name = "冻柜型号")]
        [MaxLength(100)]
        public string FreezerModel { get; set; }
        /// <summary>
        /// 箱标
        /// </summary>
        [Display(Name = "箱标")]
        [MaxLength(100)]
        public string BoxLabel { get; set; }
       
        /// <summary>
        /// 生产年限
        /// </summary>
        [Display(Name = "生产年限")]
        [MaxLength(50)]
        public DateTime? BoxTime { get; set; }
    }
}
