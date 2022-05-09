using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Admin.Application.Custom.API.SysPolling.KVInit.Settings;
using Magicodes.Admin;
using Magicodes.Admin.Core.Custom.DataDictionary;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.SysPolling.KVInit
{
    [AbpAllowAnonymous]
    public class KVInitAppService : AppServiceBase, IKVInitAppService
    {
        private readonly IRepository<BaseKey_ValueType, int> _kvTypeRepository;
        private readonly IRepository<BaseKey_Value, long> _kvRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public KVInitAppService(IRepository<BaseKey_ValueType, int> kvTypeRepository
            , IRepository<BaseKey_Value, long> kvRepository
            , IUnitOfWorkManager unitOfWorkManager)
        {
            _kvTypeRepository = kvTypeRepository;
            _kvRepository = kvRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        /// <summary>
        /// 初始化键值对类型/键值对表
        /// </summary>
        /// <param name="TenantId"></param>
        /// <returns></returns>
        public async Task InitBaseKVType(int TenantId)
        {
            _unitOfWorkManager.Current.SetTenantId(TenantId);
            var allKVType = _kvTypeRepository.GetAll().ToList();
            KVTypeSetting kvTypeSet = new KVTypeSetting();
            foreach (var item in kvTypeSet.GetKVTypeFormatDtos())
            {
                if (allKVType.Where(x => x.TypeCode == item.Key).FirstOrDefault() != null)
                {
                    continue;
                }
                BaseKey_ValueType kvType = new BaseKey_ValueType()
                {
                    TypeCode = item.Key,
                    TypeName = item.Value,
                    ParentCode = item.ParentKey,
                    SystemSetting = item.SysSet,
                    TenantId = TenantId,
                    Remarks = item.SysSet ? "系统默认" : "",
                    CreationTime = DateTime.Now
                };
                await _kvTypeRepository.InsertAsync(kvType);
            }
            //初始化键值对表
            var allKVValue = _kvRepository.GetAll().ToList();
            KVValueSettiing lvValue = new KVValueSettiing();
            foreach (var item in lvValue.GetKVValueFormatDtos())
            {
                if (allKVValue.Where(x => x.Code == item.Key).FirstOrDefault() != null)
                {
                    continue;
                }
                else
                {
                    BaseKey_Value kv = new BaseKey_Value()
                    {
                        Code = item.Key,
                        Name = item.Value,
                        BaseKey_ValueTypeCode = item.ParentKey,
                        TenantId = TenantId,
                        SystemSetting = item.SysSet,
                        Remarks = item.SysSet ? "系统默认" : "",
                        CreationTime = DateTime.Now
                    };
                    await _kvRepository.InsertAsync(kv);
                }
            }
        }
    }
}
