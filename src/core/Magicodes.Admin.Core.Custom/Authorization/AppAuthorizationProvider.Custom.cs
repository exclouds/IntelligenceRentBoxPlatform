using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Magicodes.Admin.Authorization;

namespace Magicodes.Admin.Core.Custom.Authorization
{
	/// <summary>
	/// Application's authorization provider.
	/// Defines permissions for the application.
	/// See <see cref="AppCustomPermissions"/> for all permission names.
	/// </summary>
	public partial class AppCustomAuthorizationProvider : AuthorizationProvider
	{
		private readonly bool _isMultiTenancyEnabled;

		public AppCustomAuthorizationProvider(bool isMultiTenancyEnabled)
		{
			_isMultiTenancyEnabled = isMultiTenancyEnabled;
		}

		public AppCustomAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
		{
			_isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
		}

		public override void SetPermissions(IPermissionDefinitionContext context)
		{
            var customs = context.GetPermissionOrNull(AppCustomPermissions.Customs) ?? context.CreatePermission(AppCustomPermissions.Customs, L("管理"));
            #region  新开发的都放这里面，下面如果有用，剪切过来，按页面分好类
            #region 基础数据
           
            #region 会计科目
            var AccountSubject = customs.CreateChildPermission(AppCustomPermissions.Custom_AccountSubject, L("Custom_AccountSubject"));

            AccountSubject.CreateChildPermission(AppCustomPermissions.Customs_AccountSubject_Create, L("CreateNew"));
            AccountSubject.CreateChildPermission(AppCustomPermissions.Customs_AccountSubject_Edit, L("Edit"));
            AccountSubject.CreateChildPermission(AppCustomPermissions.Customs_AccountSubject_Delete, L("Delete"));
            #endregion

            //导入情况查询
            var ImportDataSituation = customs.CreateChildPermission(AppCustomPermissions.Custom_ImportDataSituation, L("Custom_ImportDataSituation"));


            #endregion
            #region  数据字典
            var Dictionary = customs.CreateChildPermission(AppCustomPermissions.Customs_Dictionary, L("Customs_Dictionary"));
			Dictionary.CreateChildPermission(AppCustomPermissions.Customs_Dictionary_Create, L("CreateNew"));
			Dictionary.CreateChildPermission(AppCustomPermissions.Customs_Dictionary_Edit, L("Edit"));
			Dictionary.CreateChildPermission(AppCustomPermissions.Customs_Dictionary_Delete, L("Delete"));
			#endregion

			#region  数据字典类型

			var DictionaryType = customs.CreateChildPermission(AppCustomPermissions.Customs_DictionaryType, L("Customs_DictionaryType"));
			DictionaryType.CreateChildPermission(AppCustomPermissions.Customs_DictionaryType_Create, L("CreateNew"));
			DictionaryType.CreateChildPermission(AppCustomPermissions.Customs_DictionaryType_Edit, L("Edit"));
			DictionaryType.CreateChildPermission(AppCustomPermissions.Customs_DictionaryType_Delete, L("Delete"));

            #endregion

          
            #region 基础信息
            //国家资料
            var BaseCountry = customs.CreateChildPermission(AppCustomPermissions.Customs_BaseCountry, L("Customs_BaseCountry"));
            //站点维护
            var SiteTable = customs.CreateChildPermission(AppCustomPermissions.Customs_SiteTable, L("Customs_SiteTable"));
            //路线维护
            var CountryLine = customs.CreateChildPermission(AppCustomPermissions.Customs_CountryLine, L("Customs_CountryLine"));
            //站点路线维护
            var LinSite = customs.CreateChildPermission(AppCustomPermissions.Customs_LinSite, L("Customs_LinSite"));

            #endregion


            #region 信息审核
            //注册信息审核
            var RegisterCheck = customs.CreateChildPermission(AppCustomPermissions.Customs_RegisterCheck, L("Customs_RegisterCheck"));
            //注册信息审核
            var XDBoxCheck = customs.CreateChildPermission(AppCustomPermissions.Customs_XDBoxCheck, L("Customs_XDBoxCheck"));
            //租客发布审核
            var ZKTenantCheck = customs.CreateChildPermission(AppCustomPermissions.Customs_ZKTenantCheck, L("Customs_ZKTenantCheck"));

            #endregion

            #region 信息查询
            //船东信息查询
            var BoxRelease = customs.CreateChildPermission(AppCustomPermissions.Customs_BoxRelease, L("Customs_BoxRelease"));
            //租客信息查询
            var TenantRelease = customs.CreateChildPermission(AppCustomPermissions.Customs_TenantRelease, L("Customs_TenantRelease"));
            //提单信息确认
            var BoxTenantOrder = customs.CreateChildPermission(AppCustomPermissions.Customs_BoxTenantOrder, L("Customs_BoxTenantOrder"));
            //智能推荐
            var InterRecomQuery = customs.CreateChildPermission(AppCustomPermissions.Customs_InterRecomQuery, L("Customs_InterRecomQuery"));

            #endregion

            #region 客服中心
            //客服中心
            var ImChatServer = customs.CreateChildPermission(AppCustomPermissions.Customs_ImChatServer, L("Customs_ImChatServer"));

            #endregion


        #endregion


        //TODO：用户自定义

        var BasicInfo = customs.CreateChildPermission(AppCustomPermissions.Customs_BasicInfo, L("基础信息"));
			var PublicInfo = BasicInfo.CreateChildPermission(AppCustomPermissions.Customs_PublicInfo, L("公共信息"));
			var SystemInfo = BasicInfo.CreateChildPermission(AppCustomPermissions.Customs_SystemInfo, L("系统设置"));

            

        }

        private static ILocalizableString L(string name)
		{
			return new LocalizableString(name, AdminConsts.LocalizationSourceName);
		}
	}
}