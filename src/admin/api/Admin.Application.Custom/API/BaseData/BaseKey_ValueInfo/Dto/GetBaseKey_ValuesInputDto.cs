using Abp.Extensions;
using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueInfo.Dto
{
	public partial class GetBaseKey_ValuesInputDto : PagedAndSortedInputDto, IShouldNormalize
	{

		/// <summary>
		/// ID
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 类型代码检索
		/// </summary>
		public string TypeCode { get; set; }
		/// <summary>
		/// 代码检索
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 值检索
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 排序
		/// </summary>
		public void Normalize()
		{
			if (Sorting.IsNullOrWhiteSpace())
			{
				Sorting = "CreationTime DESC";
			}
		}
	}
}
