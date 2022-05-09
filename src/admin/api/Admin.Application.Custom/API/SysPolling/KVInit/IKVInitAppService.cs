using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.SysPolling.KVInit
{
	public interface IKVInitAppService : IApplicationService
	{
		/// <summary>
		/// 数据库初始化，键值对类型初始化
		/// </summary>
		/// <param name="TenantId"></param>
		/// <returns></returns>
		Task InitBaseKVType(int TenantId);
	}
}
