using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Admin.Application.Custom.API.PublicArea.Combox.Dto;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.PublicArea.Combox
{
    public interface IPublicComboxAppSevrice : IApplicationService
    {
        /// <summary>
        /// 根据字典类型获取字典项下拉
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PublicComboBoxDto> GetBaseKeyValueComboBox(BaseKeyValueComboBoxQueryDto input);

        /// <summary>
        /// 获取国家下拉
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PublicComboBoxDto> GetCountryComboBox(NullableIdDto input);

        /// <summary>
        /// 获取城市下拉
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PublicComboBoxDto> GetCityComboBox();
        /// <summary>
        /// 获取城市联动下拉
        /// </summary>
        /// <returns></returns>
        Task<PublicComboBoxDto> GetCityByComBox(string ParentCode);

        /// <summary>
        /// 获取港口下拉
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PublicComboBoxDto> GetPortComboBox(NullableIdDto input);

        /// <summary>
        /// 获取等级下拉
        /// </summary>
        /// <returns></returns>
        Task<PublicComboBoxDto> GetGradeComboBox(NullableIdDto input);

        /// <summary>
        /// 根据Code值获取省市区信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<LinkageValuesDto> GetLinkageValues(string code);
    }
}