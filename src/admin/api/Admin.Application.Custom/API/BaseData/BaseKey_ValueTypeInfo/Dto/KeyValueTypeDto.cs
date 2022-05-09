using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueTypeInfo.Dto
{
	
	public class KeyValueTypeDto : EntityDto<string>
	{
		/// <summary>
		/// 类型代码
		/// </summary>
		public string TypeCode { get; set; }

		/// <summary>
		/// 类型名称
		/// </summary>
		public string TypeName { get; set; }

		/// <summary>
		/// 父级Code
		/// </summary>
		public string ParentCode { get; set; }

		/// <summary>
		/// 是否为系统参数
		/// </summary>
		public bool? SystemSetting { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreationTime { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string Remarks { get; set; }
		/// <summary>
		/// 是否删除
		public bool IsDeleted { get; set; }
	}
}
