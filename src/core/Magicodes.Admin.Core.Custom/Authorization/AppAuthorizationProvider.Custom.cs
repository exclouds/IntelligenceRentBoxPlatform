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

            #region 数据导入

            #region 预算导入
            var BudgetImport = customs.CreateChildPermission(AppCustomPermissions.Customs_BudgetImport, L("Customs_BudgetImport"));
            BudgetImport.CreateChildPermission(AppCustomPermissions.Customs_BudgetImport_Create, L("CreateNew"));
            BudgetImport.CreateChildPermission(AppCustomPermissions.Customs_BudgetImport_Edit, L("Edit"));
            BudgetImport.CreateChildPermission(AppCustomPermissions.Customs_BudgetImport_Delete, L("Delete"));
            BudgetImport.CreateChildPermission(AppCustomPermissions.Customs_BudgetImport_Submit, L("Submit"));
            BudgetImport.CreateChildPermission(AppCustomPermissions.Customs_BudgetImport_CancelSubmit, L("CancelSubmit"));
            BudgetImport.CreateChildPermission(AppCustomPermissions.Customs_BudgetImport_AgreeCancelSubmit, L("AgreeCancelSubmit"));
            #endregion

            #region 科目辅助约数数据导入
            var AccountBalanceImport = customs.CreateChildPermission(AppCustomPermissions.Customs_AccountBalanceImport, L("Customs_AccountBalanceImport"));
            AccountBalanceImport.CreateChildPermission(AppCustomPermissions.Customs_AccountBalanceImport_Import, L("Import"));
            AccountBalanceImport.CreateChildPermission(AppCustomPermissions.Customs_AccountBalanceImport_Create, L("CreateNew"));
            AccountBalanceImport.CreateChildPermission(AppCustomPermissions.Customs_AccountBalanceImport_Edit, L("Edit"));
            AccountBalanceImport.CreateChildPermission(AppCustomPermissions.Customs_AccountBalanceImport_Delete, L("Delete"));
            AccountBalanceImport.CreateChildPermission(AppCustomPermissions.Customs_AccountBalanceImport_Submit, L("Submit"));
            AccountBalanceImport.CreateChildPermission(AppCustomPermissions.Customs_AccountBalanceImport_CancelSubmit, L("CancelSubmit"));
            AccountBalanceImport.CreateChildPermission(AppCustomPermissions.Customs_AccountBalanceImport_AgreeCancelSubmit, L("AgreeCancelSubmit"));
            #endregion

            #region 合资公司经营分析报表导入
            var AnalysisReportImport = customs.CreateChildPermission(AppCustomPermissions.Customs_AnalysisReportImport, L("Customs_AnalysisReportImport"));
            AnalysisReportImport.CreateChildPermission(AppCustomPermissions.Customs_AnalysisReportImport_Create, L("CreateNew"));
            AnalysisReportImport.CreateChildPermission(AppCustomPermissions.Customs_AnalysisReportImport_Edit, L("Edit"));
            AnalysisReportImport.CreateChildPermission(AppCustomPermissions.Customs_AnalysisReportImport_Delete, L("Delete"));
            AnalysisReportImport.CreateChildPermission(AppCustomPermissions.Customs_AnalysisReportImport_Submit, L("Submit"));
            AnalysisReportImport.CreateChildPermission(AppCustomPermissions.Customs_AnalysisReportImport_CancelSubmit, L("CancelSubmit"));
            AnalysisReportImport.CreateChildPermission(AppCustomPermissions.Customs_AnalysisReportImport_AgreeCancelSubmit, L("AgreeCancelSubmit"));
            #endregion

            #endregion

            #region 数据维护

            #region 业务数据维护
            var BusinessData = customs.CreateChildPermission(AppCustomPermissions.Customs_BusinessData, L("Customs_BusinessData"));
            BusinessData.CreateChildPermission(AppCustomPermissions.Customs_BusinessData_Create, L("CreateNew"));
            BusinessData.CreateChildPermission(AppCustomPermissions.Customs_BusinessData_Edit, L("Edit"));
            BusinessData.CreateChildPermission(AppCustomPermissions.Customs_BusinessData_Delete, L("Delete"));
            BusinessData.CreateChildPermission(AppCustomPermissions.Customs_BusinessData_Submit, L("Submit"));
            BusinessData.CreateChildPermission(AppCustomPermissions.Customs_BusinessData_CancelSubmit, L("CancelSubmit"));
            BusinessData.CreateChildPermission(AppCustomPermissions.Customs_BusinessData_AgreeCancelSubmit, L("AgreeCancelSubmit"));
            #endregion

            #region 特殊费用单据维护
            var SpecialFee = customs.CreateChildPermission(AppCustomPermissions.Customs_SpecialFee, L("Customs_SpecialFee"));
            SpecialFee.CreateChildPermission(AppCustomPermissions.Customs_SpecialFee_Create, L("CreateNew"));
            SpecialFee.CreateChildPermission(AppCustomPermissions.Customs_SpecialFee_Edit, L("Edit"));
            SpecialFee.CreateChildPermission(AppCustomPermissions.Customs_SpecialFee_Delete, L("Delete"));
            SpecialFee.CreateChildPermission(AppCustomPermissions.Customs_SpecialFee_Submit, L("Submit"));
            SpecialFee.CreateChildPermission(AppCustomPermissions.Customs_SpecialFee_CancelSubmit, L("CancelSubmit"));
            SpecialFee.CreateChildPermission(AppCustomPermissions.Customs_SpecialFee_AgreeCancelSubmit, L("AgreeCancelSubmit"));
            #endregion

            #region 特殊费用单据维护
            var ReportSpecialFee = customs.CreateChildPermission(AppCustomPermissions.Customs_ReportSpecialFee, L("Customs_ReportSpecialFee"));
            ReportSpecialFee.CreateChildPermission(AppCustomPermissions.Customs_ReportSpecialFee_Create, L("CreateNew"));
            ReportSpecialFee.CreateChildPermission(AppCustomPermissions.Customs_ReportSpecialFee_Edit, L("Edit"));
            ReportSpecialFee.CreateChildPermission(AppCustomPermissions.Customs_ReportSpecialFee_Delete, L("Delete"));
            ReportSpecialFee.CreateChildPermission(AppCustomPermissions.Customs_ReportSpecialFee_Submit, L("Submit"));
            ReportSpecialFee.CreateChildPermission(AppCustomPermissions.Customs_ReportSpecialFee_CancelSubmit, L("CancelSubmit"));
            ReportSpecialFee.CreateChildPermission(AppCustomPermissions.Customs_ReportSpecialFee_AgreeCancelSubmit, L("AgreeCancelSubmit"));
            #endregion

            #endregion

            #region 报表导出

            //报表查询
            var ReportExport = customs.CreateChildPermission(AppCustomPermissions.Customs_ReportExport, L("Customs_ReportExport"));
            ReportExport.CreateChildPermission(AppCustomPermissions.Customs_ReportExport_Export, L("Export"));

            //BU报表查询
            var BUReportExport = customs.CreateChildPermission(AppCustomPermissions.Customs_BUReportExport, L("Customs_BUReportExport"));
            BUReportExport.CreateChildPermission(AppCustomPermissions.Customs_BUReportExport_Export, L("Export"));

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