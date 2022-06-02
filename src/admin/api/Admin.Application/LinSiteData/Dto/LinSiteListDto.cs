using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.LinSiteData.Dto
{
    public class LinSiteListDto : EntityBase<int>
    {
        /*
        如出现两个站点对应多个线路前台可选 
        */

        /// <summary>
        /// 站点代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 航线
        /// </summary>
        public string LineId { get; set; }
    }
}
