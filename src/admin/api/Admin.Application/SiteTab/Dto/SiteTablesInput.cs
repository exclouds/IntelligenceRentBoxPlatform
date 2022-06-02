﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.SiteTab.Dto
{
    public class SiteTablesInput
    {
        public int? Id { get; set; }
        /// <summary>
        /// 站点代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 国家代码
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
