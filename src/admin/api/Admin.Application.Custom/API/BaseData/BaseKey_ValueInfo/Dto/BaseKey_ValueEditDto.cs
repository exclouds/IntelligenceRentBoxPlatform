using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.Admin.Core.Custom.DataDictionary;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueInfo.Dto
{
	/// <summary>
	///  编辑键值对Dto
	/// </summary>
	[AutoMapFrom(typeof(BaseKey_Value))]
	public class BaseKey_ValueEditDto : EntityDto<string>
	{
		/// <summary>
		/// 所属类别
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
		/// 备注
		/// </summary>
		public string Remarks { get; set; }

	}
}
