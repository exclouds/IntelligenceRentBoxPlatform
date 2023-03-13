namespace Magicodes.Admin.Core.Custom.Authorization
{
	/// <summary>
	/// 定义应用程序权限名称常量
	/// <see cref="Core.Custom.Authorization.AppCustomAuthorizationProvider"/> 权限定义.
	/// </summary>
	public partial class AppCustomPermissions
	{
		public const string Customs = "Customs";
		#region 新开发的都放这里面，下面如果有用，剪切过来，按页面分好类

		#region 基础数据
		//基础数据
		public const string Customs_BasicInfo = "Customs.BasicInfo";
		//公共信息
		public const string Customs_PublicInfo = "Customs.PublicInfo";
		//系统设置
		public const string Customs_SystemInfo = "Customs.SystemInfo";
		
		//财务计费基础信息
		public const string Customs_FinancialInfo = "Customs.FinancialInfo";
		//流程控制状态信息
		public const string Custom_ProcessControlInfo = "Customs.ProcessControlInfo";
        //会计科目
        public const string Custom_AccountSubject = "Customs.AccountSubject";
        public const string Customs_AccountSubject_Create = "Customs.AccountSubject.Create";
        public const string Customs_AccountSubject_Edit = "Customs.AccountSubject.Edit";
        public const string Customs_AccountSubject_Delete = "Customs.AccountSubject.Delete";

        public const string Custom_ImportDataSituation = "Customs.ImportDataSituation";


        #endregion

        #region 字典类型和字典
        //字典类型
        public const string Customs_DictionaryType = "Customs.DictionaryType";
		public const string Customs_DictionaryType_Create = "Customs.DictionaryTypeCreate";
		public const string Customs_DictionaryType_Edit = "Customs.DictionaryTypeEdit";
		public const string Customs_DictionaryType_Delete = "Customs.DictionaryTypeDelete";
		//字典
		public const string Customs_Dictionary = "Customs.Dictionary";
		public const string Customs_Dictionary_Create = "Customs.DictionaryCreate";
		public const string Customs_Dictionary_Edit = "Customs.DictionaryEdit";
		public const string Customs_Dictionary_Delete = "Customs.DictionaryDelete";
        #endregion

        #region 数据导入

        #region 预算导入
        //字典
        public const string Customs_BudgetImport = "Customs.BudgetImport";
        public const string Customs_BudgetImport_Create = "Customs.BudgetImport.Create";
        public const string Customs_BudgetImport_Edit = "Customs.BudgetImport.Edit";
        public const string Customs_BudgetImport_Delete = "Customs.BudgetImport.Delete";
        public const string Customs_BudgetImport_Submit = "Customs.BudgetImport.Submit";
        public const string Customs_BudgetImport_CancelSubmit = "Customs.BudgetImport.CancelSubmit";
        public const string Customs_BudgetImport_AgreeCancelSubmit = "Customs.BudgetImport.AgreeCancelSubmit";
        #endregion

        #region 科目辅助约数数据导入
        //字典
        public const string Customs_AccountBalanceImport = "Customs.AccountBalanceImport";
        public const string Customs_AccountBalanceImport_Import = "Customs.AccountBalanceImport.import";
        public const string Customs_AccountBalanceImport_Create = "Customs.AccountBalanceImport.Create";
        public const string Customs_AccountBalanceImport_Edit = "Customs.AccountBalanceImport.Edit";
        public const string Customs_AccountBalanceImport_Delete = "Customs.AccountBalanceImport.Delete";
        public const string Customs_AccountBalanceImport_Submit = "Customs.AccountBalanceImport.Submit";
        public const string Customs_AccountBalanceImport_CancelSubmit = "Customs.AccountBalanceImport.CancelSubmit";
        public const string Customs_AccountBalanceImport_AgreeCancelSubmit = "Customs.AccountBalanceImport.AgreeCancelSubmit";
        #endregion

        #region 合资公司经营分析报表导入
        public const string Customs_AnalysisReportImport = "Customs.AnalysisReportImport";
        public const string Customs_AnalysisReportImport_Create = "Customs.AnalysisReportImport.Create";
        public const string Customs_AnalysisReportImport_Edit = "Customs.AnalysisReportImport.Edit";
        public const string Customs_AnalysisReportImport_Delete = "Customs.AnalysisReportImport.Delete";
        public const string Customs_AnalysisReportImport_Submit = "Customs.AnalysisReportImport.Submit";
        public const string Customs_AnalysisReportImport_CancelSubmit = "Customs.AnalysisReportImport.CancelSubmit";
        public const string Customs_AnalysisReportImport_AgreeCancelSubmit = "Customs.AnalysisReportImport.AgreeCancelSubmit";
        #endregion

        #endregion

        #region 数据维护
       
        #region 业务数据维护
        public const string Customs_BusinessData = "Customs.BusinessData";
        public const string Customs_BusinessData_Create = "Customs.BusinessData.Create";
        public const string Customs_BusinessData_Edit = "Customs.BusinessData.Edit";
        public const string Customs_BusinessData_Delete = "Customs.BusinessData.Delete";
        public const string Customs_BusinessData_Submit = "Customs.BusinessData.Submit";
        public const string Customs_BusinessData_CancelSubmit = "Customs.BusinessData.CancelSubmit";
        public const string Customs_BusinessData_AgreeCancelSubmit = "Customs.BusinessData.AgreeCancelSubmit";
        #endregion

        #region 特殊费用单据维护
        public const string Customs_SpecialFee = "Customs.SpecialFee";
        public const string Customs_SpecialFee_Create = "Customs.SpecialFee.Create";
        public const string Customs_SpecialFee_Edit = "Customs.SpecialFee.Edit";
        public const string Customs_SpecialFee_Delete = "Customs.SpecialFee.Delete";
        public const string Customs_SpecialFee_Submit = "Customs.SpecialFee.Submit";
        public const string Customs_SpecialFee_CancelSubmit = "Customs.SpecialFee.CancelSubmit";
        public const string Customs_SpecialFee_AgreeCancelSubmit = "Customs.SpecialFee.AgreeCancelSubmit";
        #endregion

        #region 集团报表特殊费用单据维护
        public const string Customs_ReportSpecialFee = "Customs.ReportSpecialFee";
        public const string Customs_ReportSpecialFee_Create = "Customs.ReportSpecialFee.Create";
        public const string Customs_ReportSpecialFee_Edit = "Customs.ReportSpecialFee.Edit";
        public const string Customs_ReportSpecialFee_Delete = "Customs.ReportSpecialFee.Delete";
        public const string Customs_ReportSpecialFee_Submit = "Customs.ReportSpecialFee.Submit";
        public const string Customs_ReportSpecialFee_CancelSubmit = "Customs.ReportSpecialFee.CancelSubmit";
        public const string Customs_ReportSpecialFee_AgreeCancelSubmit = "Customs.ReportSpecialFee.AgreeCancelSubmit";
        #endregion

        #endregion

        #region 报表导出

        //报表查询
        public const string Customs_ReportExport = "Customs.ReportExport";
        public const string Customs_ReportExport_Export = "Customs.ReportExport.Export";

        //BU报表查询
        public const string Customs_BUReportExport = "Customs.BUReportExport";
        public const string Customs_BUReportExport_Export = "Customs.BUReportExport.Export";

        #endregion



        #endregion



        //TODO:用户自定义


        #region BaseKey_ValueType【字典类型对照表】
        public const string Customs_BaseKey_ValueType = "Customs.BaseKey_ValueType";
		public const string Customs_BaseKey_ValueType_Create = "Customs.BaseKey_ValueType.Create";
		public const string Customs_BaseKey_ValueType_Edit = "Customs.BaseKey_ValueType.Edit";
		public const string Customs_BaseKey_ValueType_Delete = "Customs.BaseKey_ValueType.Delete";
		public const string Customs_BaseKey_ValueType_BatchDelete = "Customs.BaseKey_ValueType.BatchDelete";
		#endregion

		#region BaseKey_Value【字典项对照表】
		public const string Customs_BaseKey_Value = "Customs.BaseKey_Value";
		public const string Customs_BaseKey_Value_Create = "Customs.BaseKey_Value.Create";
		public const string Customs_BaseKey_Value_Edit = "Customs.BaseKey_Value.Edit";
		public const string Customs_BaseKey_Value_Delete = "Customs.BaseKey_Value.Delete";
		public const string Customs_BaseKey_Value_BatchDelete = "Customs.BaseKey_Value.BatchDelete";
        #endregion

        #region 基础信息
        //国家资料
        public const string Customs_BaseCountry = "Customs.BaseCountry";
        //站点维护
        public const string Customs_SiteTable = "Customs.SiteTable";
        //路线维护
        public const string Customs_CountryLine = "Customs.CountryLine";
        //站点路线维护
        public const string Customs_LinSite = "Customs.LinSite";

        #endregion

        #region 信息审核
        //注册信息审核
        public const string Customs_RegisterCheck = "Customs.RegisterCheck";
        //注册信息审核
        public const string Customs_XDBoxCheck = "Customs.XDBoxCheck";
        //租客发布审核
        public const string Customs_ZKTenantCheck = "Customs.ZKTenantCheck";

        #endregion

        #region 信息查询
        //船东信息查询
        public const string Customs_BoxRelease = "Customs.BoxRelease";
        //租客信息查询
        public const string Customs_TenantRelease = "Customs.TenantRelease";
        //提单信息确认
        public const string Customs_BoxTenantOrder = "Customs.BoxTenantOrder";
        //智能推荐
        public const string Customs_InterRecomQuery = "Customs.InterRecomQuery";

        #endregion

        #region 客服中心
        //客服中心
        public const string Customs_ImChatServer = "Customs.ImChatServer";
       
        #endregion

    }
}