using Magicodes.Admin.Core.Custom;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.Core.Custom.Business;
using Magicodes.Admin.Core.Custom.DataDictionary;
using Magicodes.Admin.Core.Custom.IMChat;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public partial class AdminDbContext
	{
        public virtual DbSet<Organizations.MyOrganization> MyOrganizations { get; set; }

        #region 基础信息

        /// 数据字典类型
		/// </summary>
		public virtual DbSet<BaseKey_ValueType> BaseKey_ValueType { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        public virtual DbSet<BaseKey_Value> BaseKey_Value { get; set; }
        
        public virtual DbSet<ContactNO> ContactNO { get; set; }

        public virtual DbSet<Line> Lines { get; set; }
        public virtual DbSet<LinSite> LinSites { get; set; }

        public virtual DbSet<Country> Countries { get; set; }
        #endregion

        #region 业务

        public virtual DbSet<BoxInfo> BoxInfos { get; set; }

        public virtual DbSet<BoxDetails> BoxDetailses { get; set; }

        public virtual DbSet<TenantInfo> TenantInfos { get; set; }

        public virtual DbSet<SiteTable> SiteTables { get; set; }

        public virtual DbSet<BusinessConfirm> BusinessConfirms { get; set; }

        public virtual DbSet<IMChatMsg> IMChatMsgs { get; set; }
        public virtual DbSet<IMClientEn> IMClientEns { get; set; }
        public virtual DbSet<IMServerEn> IMServerEns { get; set; }
        /// <summary>
        /// 部门信息
        /// </summary>
        public virtual DbSet<Department> Department { get; set; }
       

        #endregion


    }
}
