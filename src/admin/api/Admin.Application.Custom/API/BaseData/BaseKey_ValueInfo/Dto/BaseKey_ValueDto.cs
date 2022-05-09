using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.Admin.Core.Custom.DataDictionary;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueInfo.Dto
{
	[AutoMapFrom(typeof(BaseKey_Value))]
	public partial class BaseKey_ValueDto : EntityDto<string>
	{
		/// <summary>
		/// 键值对类型
		/// </summary>
		public string BaseKey_ValueTypeCode { get; set; }
		/// <summary>
		/// 键
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 值
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreationTime { get; set; }
		/// <summary>
		/// 最后修改时间
		/// </summary>
		public DateTime LastModificationTime { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string Remarks { get; set; }
		/// <summary>
		/// 是否删除
		/// </summary>
		public bool IsDeleted { get; set; }

		/// <summary>
		/// 是否为系统参数
		/// </summary>
		public bool? SystemSetting { get; set; }
	}
}
