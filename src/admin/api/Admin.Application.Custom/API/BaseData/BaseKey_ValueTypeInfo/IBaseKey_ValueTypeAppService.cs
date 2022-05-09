using Abp.Application.Services;
using Admin.Application.Custom.API.BaseData.BaseKey_ValueTypeInfo.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueTypeInfo
{
    public interface IBaseKey_ValueTypeAppService : IApplicationService
	{
		/// <summary>
		/// 获取字典类型数据结构
		/// </summary>
		/// <returns></returns>
		List<GetAllTypeListDto> GetAllTypeList(string code);

		/// <summary>
		/// 创建或修改键值对类型对照表
		/// </summary>
		/// <param name="input">input parameter</param>
		/// <returns></returns>
		Task CreateOrUpdateBaseKey_ValueType(CreateUpdateKeyValueTypeDto input);

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="input">Primary key parameter</param>
		/// <returns></returns>
		Task DeleteBaseKey_ValueType(string input);

		/// <summary>
		/// 查询单个明细
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<SingleDto> GetSingle(string id);
	}
}
