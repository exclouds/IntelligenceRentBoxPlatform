using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.BoxReleaseReview.Dto
{
    public class GetBoxInfoInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 单号
        /// </summary>
        public int? BillNO { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool? IsVerify { get; set; }
        /// <summary>
        /// 起运站
        /// </summary>
        public string StartStation { get; set; }
        /// <summary>
        /// 目的站
        /// </summary>
        public string EndStation { get; set; }
        /// <summary>
        /// 有效时间起
        /// </summary>
        public DateTime? EffectiveSTime { get; set; }
        /// <summary>
        /// 有效时间止
        /// </summary>
        public DateTime? EffectiveETime { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 箱型
        /// </summary>
        public string Box { get; set; }
        /// <summary>
        /// 单据是否完成（界面打完成标记）
        /// </summary>
        public bool? Finish { get; set; }
        /// <summary>
        /// 筛选
        /// </summary>
        public string Filter { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime ASC";
            }
        }
    }
}
