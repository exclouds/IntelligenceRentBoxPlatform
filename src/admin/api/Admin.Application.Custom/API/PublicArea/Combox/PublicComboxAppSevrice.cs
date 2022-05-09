using Abp.Domain.Repositories;
using Admin.Application.Custom.API.PublicArea.Combox.Dto;
using Magicodes.Admin;
using Magicodes.Admin.Core.Custom.DataDictionary;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.UI;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Authorization;

namespace Admin.Application.Custom.API.PublicArea.Combox
{

    /// <summary>
    /// 公共下拉服务
    /// </summary>
    [AbpAllowAnonymous]
    public class PublicComboxAppSevrice : AppServiceBase
    {
        #region 依赖注入
        
        //字典表
        private readonly IRepository<BaseKey_Value, long> _BaseKey_ValueRepository;


        /// <summary>
        /// 构造函数
        /// </summary>
        public PublicComboxAppSevrice(
            IRepository<BaseKey_Value, long> BaseKey_ValueRepository

            ) : base()
        {
            _BaseKey_ValueRepository = BaseKey_ValueRepository;

        }
        #endregion

        /// <summary>
        /// 根据字典类型获取字典项下拉
        /// </summary>
        /// <returns></returns>
        public async Task<PublicComboBoxDto> GetBaseKeyValueComboBox(BaseKeyValueComboBoxQueryDto input)
        {
            var output = new PublicComboBoxDto();
            if (input.BaseKeyValueTypeCode.IsNullOrEmpty())
            {
                return output;
            }
            output.Comboxs = _BaseKey_ValueRepository.GetAll().Where(p => p.BaseKey_ValueTypeCode == input.BaseKeyValueTypeCode)
                .Select(p => new ComboxStringDto(p.Code.ToString(), p.Name))
                .ToList();
            return output;
        }
    }
}