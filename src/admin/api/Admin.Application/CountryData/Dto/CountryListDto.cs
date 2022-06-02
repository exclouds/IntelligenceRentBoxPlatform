using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.CountryData.Dto
{
    public class CountryListDto : EntityBase<int>
    {
        /// <summary>
        /// 国家代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
