using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Magicodes.Admin.MultiTenancy.Accounting
{
    [Table("AppInvoices")]
    public class Invoice : Entity<int>
    {
        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }
        /// <summary>
        /// 开票时间
        /// </summary>
        public DateTime InvoiceDate { get; set; }
        /// <summary>
        /// 租户名
        /// </summary>
        public string TenantLegalName { get; set; }
        /// <summary>
        /// 租户地址
        /// </summary>
        public string TenantAddress { get; set; }
        /// <summary>
        /// 租户税号
        /// </summary>
        public string TenantTaxNo { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { get; set; }
    }
}
