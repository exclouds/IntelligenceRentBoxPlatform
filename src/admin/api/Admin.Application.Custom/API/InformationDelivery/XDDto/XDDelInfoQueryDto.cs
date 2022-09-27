using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.InformationDelivery.XDDto
{
    public class XDDelInfoQueryDto : PagedAndSortedInputDto, IShouldNormalize
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
        /// 航线
        /// </summary>    
        public string line { get; set; }
        
        /// <summary>
        /// 目的站
        /// </summary>   
        public List<string> EndStation { get; set; }

        /// <summary>
        /// 还箱地
        /// </summary>

        public string ReturnStation { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsEnable { get; set; } 
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool? IsVerify { get; set; }
         /// 是否库存
        /// </summary>
        public bool? IsInStock { get; set; }    
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
