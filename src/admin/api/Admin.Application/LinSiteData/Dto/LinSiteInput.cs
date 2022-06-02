using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.LinSiteData.Dto
{
    public class LinSiteInput
    {
        /*
        如出现两个站点对应多个线路前台可选 
        */
        public int? Id { get; set; }

        /// <summary>
        /// 站点代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 航线
        /// </summary>
        public string LineId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
