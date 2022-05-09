using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.Text;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueTypeInfo.Dto
{
	
	public class CreateUpdateKeyValueTypeDto
	{
		[Required]
		public KeyValueTypeDto KeyValueType { get; set; }
	}
}
