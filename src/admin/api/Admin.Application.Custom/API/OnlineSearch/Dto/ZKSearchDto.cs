using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.OnlineSearch.Dto
{
    public class ZKSearchDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 单号
        /// </summary>     
        public string BillNO { get; set; }

        /// <summary>
        /// 起运站
        /// </summary>    
        public string StartStation { get; set; }
        /// <summary>
        /// 航线
        /// </summary>    
        public string Line { get; set; }

        /// <summary>
        /// 目的站
        /// </summary>   
        public string EndStation { get; set; }
        /// 用箱时间
        /// </summary>
        public DateTime? EffectiveSTime { get; set; }
        /// <summary>
        /// 用箱时间
        /// </summary>
        public DateTime? EffectiveETime { get; set; }
        /// <summary>
        /// 箱型尺寸箱量
        /// </summary>
        public string XXCC { get; set; }
        /// <summary>
        /// 单据是否完成（界面打完成标记）
        /// </summary>
        public bool? Finish { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
        
    }
}
