using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.OnlineSearch.Dto
{
    public class XDSearchDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 单号（箱东首字母XD）
        /// </summary>     
        public string BillNO { get; set; }

        /// <summary>
        /// 起运站
        /// </summary>    
        public string StartStation { get; set; }

        /// <summary>
        /// 目的站
        /// </summary>   
        public string EndStation { get; set; }

        /// <summary>
        /// 还箱地
        /// </summary>
        public string ReturnStation { get; set; }

        /// 是否库存
        /// </summary>
        public bool? IsInStock { get; set; }
        /// <summary>
        /// 租金范围
        /// </summary>
        public decimal? startprice { get; set; }
        public decimal? endprice { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
