using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Admin.Application.Custom.API.BaseData.BaseKey_ValueInfo.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueInfo
{
	public interface IBaseKey_ValueAppService : IApplicationService
	{
		/// <summary>
		/// 获取键值对列表
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		Task<PagedResultDto<BaseKey_ValueDto>> GetBaseKey_Value(GetBaseKey_ValuesInputDto input);

		/// <summary>
		/// 创建或修改键值对
		/// </summary>
		/// <param name="input">input parameter</param>
		/// <returns></returns>
		Task CreateOrUpdateBaseKey_Value(CreateOrUpdateBaseKey_ValueDto input);

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id">主键值</param>
		/// <returns></returns>
		Task DeleteBaseKey_Value(string id);

		//Task<List<BaseKey_ValueDto>> GetValueByKyeType(string TypeCode);
	


	}
}
