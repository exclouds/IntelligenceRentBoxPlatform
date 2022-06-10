using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.InterRecom.Dto
{
    public class InteRcommenDto : PagedAndSortedInputDto, IShouldNormalize
    {
        public string billNo { get; set; }
        public string belong { get; set; }
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
        /// 路线
        /// </summary>
        public int? Line { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
