using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueInfo.Dto
{
	public partial class CreateOrUpdateBaseKey_ValueDto
	{
		/// <summary>
		/// BaseKey_Value
		/// </summary>
		[Required]
		public BaseKey_ValueEditDto BaseKey_Value { get; set; }
	}
}
