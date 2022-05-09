using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.SysPolling.KVInit.Dto
{
	public class FormatDto
	{
		public FormatDto(string key, string value, string parentkey, bool sys)
		{
			this.Key = key;
			this.Value = value;
			this.ParentKey = parentkey;
			this.SysSet = sys;
		}
		/// <summary>
		/// 键
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// 值
		/// </summary>

		public string Value { get; set; }

		/// <summary>
		/// 上级Key
		/// </summary>
		public string ParentKey { get; set; }

		/// <summary>
		/// 是否系统设置
		/// </summary>
		public bool SysSet { get; set; }
	}
}
