using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using Magicodes.Admin.Core.Custom.DataDictionary;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueTypeInfo.Dto
{
	[AutoMapFrom(typeof(BaseKey_ValueType))]
	public class SingleDto
	{
		public string Id { get; set; }
		public string TypeCode { get; set; }

		public string TypeName { get; set; }

		public string ParentCode { get; set; }

		public string ParentName { get; set; }


	}
}
