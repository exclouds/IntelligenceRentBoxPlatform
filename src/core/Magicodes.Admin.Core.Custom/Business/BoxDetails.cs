﻿using System;
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
        public int BoxTenantInfo { get; set; }


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
        /// 箱号
        /// </summary>
        [Display(Name = "箱号")]
        [MaxLength(50)]
        public string BoxNO { get; set; }
        /// <summary>
        /// 箱龄
        /// </summary>
        [Display(Name = "箱龄")]
        public double BoxAge { get; set; }

        /// <summary>
        /// 集装箱照片
        /// </summary>
        [Display(Name = "集装箱照片")]
        [MaxLength(150)]
        public string ImageUrl { get; set; }
        /// <summary>
        /// 集装箱证书
        /// </summary>
        [Display(Name = "集装箱证书")]
        [MaxLength(150)]
        public string CertificateUrl { get; set; }

        /// <summary>
        /// 是否审核
        /// </summary>
        [Display(Name = "是否审核")]
        public bool IsVerify { get; set; }
    }
}
