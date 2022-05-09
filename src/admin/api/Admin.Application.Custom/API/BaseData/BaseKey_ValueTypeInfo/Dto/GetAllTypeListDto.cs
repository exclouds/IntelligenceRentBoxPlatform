using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueTypeInfo.Dto
{
	public class GetAllTypeListDto
	{
		public string Id { get; set; }
		public string TypeCode { get; set; }

		public string TypeName { get; set; }

		public string ParentCode { get; set; }

		public List<GetAllTypeListDto> Children { get; set; }
	}
}
