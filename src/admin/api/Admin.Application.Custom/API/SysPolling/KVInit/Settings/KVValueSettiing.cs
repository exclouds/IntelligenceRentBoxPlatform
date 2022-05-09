using Admin.Application.Custom.API.SysPolling.KVInit.Dto;
using System.Collections.Generic;

namespace Admin.Application.Custom.API.SysPolling.KVInit.Settings
{
    /// <summary>
    /// 键值对值赋值
    /// </summary>
    public class KVValueSettiing
    {
        /// <summary>
        /// 键值对赋值
        /// </summary>
        /// <returns></returns>
        public List<FormatDto> GetKVValueFormatDtos()
        {
            List<FormatDto> list = new List<FormatDto>()
            {


                #region 预算类型
                new FormatDto("ADJUSTMENTSUM", "调整汇总表", "BUDGETTYPE", true),
                new FormatDto("PROFIT", "利润表", "BUDGETTYPE", true),
                new FormatDto("COSTBREAKDOWN", "成本明细表", "BUDGETTYPE", true),
                new FormatDto("FEESCHEDULE", "费用表", "BUDGETTYPE", true),
                new FormatDto("OPERATINGEXPENSES", "营业费用", "BUDGETTYPE", true),
                new FormatDto("SALESEXPENSE", "销售费用", "BUDGETTYPE", true),
                new FormatDto("MANAGEMENTCOSTS", "管理费用", "BUDGETTYPE", true),
                new FormatDto("R&D", "研发费用小计", "BUDGETTYPE", true),
                new FormatDto("CAPITAL", "资本化支出", "BUDGETTYPE", true),
                new FormatDto("EXPENDITURE", "费用化支出", "BUDGETTYPE", true),
                
                #endregion
                #region 会计科目方向
                new FormatDto("DEBIT", "借方", "DIRECTION", true),
                new FormatDto("CREDIT", "贷方", "DIRECTION", true),
                #endregion

                #region BU
                new FormatDto("箱子", "箱子", "BU", true),
                new FormatDto("盒子", "盒子", "BU", true),
                new FormatDto("链子", "链子", "BU", true),
                #endregion
                #region 科目余额
                new FormatDto("660105", "660105", "ACCOUNTBALANCE", true),
                new FormatDto("660205", "660205", "ACCOUNTBALANCE", true),
                new FormatDto("66040104", "66040104", "ACCOUNTBALANCE", true),
                new FormatDto("66040204", "66040204", "ACCOUNTBALANCE", true),
                new FormatDto("64010105", "64010105", "ACCOUNTBALANCE", true),
                new FormatDto("64010205", "64010205", "ACCOUNTBALANCE", true),
                new FormatDto("64010305", "64010305", "ACCOUNTBALANCE", true),
                new FormatDto("64010405", "64010405", "ACCOUNTBALANCE", true),
                new FormatDto("64010505", "64010505", "ACCOUNTBALANCE", true),
                new FormatDto("64010605", "64010605", "ACCOUNTBALANCE", true),
                new FormatDto("64010705", "64010705", "ACCOUNTBALANCE", true),
                new FormatDto("64010805", "64010805", "ACCOUNTBALANCE", true),
                new FormatDto("64010905", "64010905", "ACCOUNTBALANCE", true),
                new FormatDto("64011005", "64011005", "ACCOUNTBALANCE", true),
	                #endregion
                #region 星期
                //new FormatDto("MON", "星期一", "XINGQIINFO", true),
                //         new FormatDto("TUE", "星期二", "XINGQIINFO", true),
                //         new FormatDto("WED", "星期三", "XINGQIINFO", true),
                //         new FormatDto("THU", "星期四", "XINGQIINFO", true),
                //         new FormatDto("FRI", "星期五", "XINGQIINFO", true),
                //         new FormatDto("SAT", "星期六", "XINGQIINFO", true),
                //         new FormatDto("SUN", "星期日", "XINGQIINFO", true),

                #endregion

                #region 时区
                //new FormatDto("UTC", "零时区", "SHIQU", true),
                //         new FormatDto("UTC1", "东一区", "SHIQU", true),
                //         new FormatDto("UTC2", "东二区", "SHIQU", true),
                //         new FormatDto("UTC3", "东三区", "SHIQU", true),
                //         new FormatDto("UTC4", "东四区", "SHIQU", true),
                //         new FormatDto("UTC5", "东五区", "SHIQU", true),
                //         new FormatDto("UTC6", "东六区", "SHIQU", true),
                //         new FormatDto("UTC7", "东七区", "SHIQU", true),
                //         new FormatDto("UTC8", "东八区", "SHIQU", true),
                //         new FormatDto("UTC9", "东九区", "SHIQU", true),
                //         new FormatDto("UTC10", "东十区", "SHIQU", true),
                //         new FormatDto("UTC11", "东十一区", "SHIQU", true),
                //         new FormatDto("UTC12", "东西十二区", "SHIQU", true),
                //         new FormatDto("UTC-11", "西十一区", "SHIQU", true),
                //         new FormatDto("UTC-10", "西十区", "SHIQU", true),
                //         new FormatDto("UTC-9", "西九区", "SHIQU", true),
                //         new FormatDto("UTC-8", "西八区", "SHIQU", true),
                //         new FormatDto("UTC-7", "西七区", "SHIQU", true),
                //         new FormatDto("UTC-6", "西六区", "SHIQU", true),
                //         new FormatDto("UTC-5", "西五区", "SHIQU", true),
                //         new FormatDto("UTC-4", "西四区", "SHIQU", true),
                //         new FormatDto("UTC-3", "西三区", "SHIQU", true),
                //         new FormatDto("UTC-2", "西二区", "SHIQU", true),
                //         new FormatDto("UTC-1", "西一区", "SHIQU", true),
                #endregion

            };

            return list;
        }

    }
}
