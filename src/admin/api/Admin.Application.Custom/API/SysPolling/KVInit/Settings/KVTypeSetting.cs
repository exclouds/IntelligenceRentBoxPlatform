using Admin.Application.Custom.API.SysPolling.KVInit.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.SysPolling.KVInit.Settings
{
    /// <summary>
    /// 键值对类型对照
    /// </summary>
    public class KVTypeSetting
    {
        public List<FormatDto> GetKVTypeFormatDtos()
        {
            List<FormatDto> list = new List<FormatDto>()
            { 
			#region 跟节点
                new FormatDto("BUDGETTYPE", "预算类型", "", true),
                new FormatDto("DIRECTION", "会计科目方向", "", true),
                new FormatDto("BU", "BU", "", true),
                new FormatDto("ACCOUNTBALANCE", "科目辅助余额", "", true),
                new FormatDto("OTHERS", "其他", "", true),
			#endregion
                
            };
            return list;
        }
    }
}
