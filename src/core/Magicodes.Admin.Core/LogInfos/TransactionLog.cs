﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;

namespace Magicodes.Admin.LogInfos
{
    /// <summary>
    /// 交易日志
    /// </summary>
    [Description("交易日志")]
    [Display(Name = "交易日志")]
    public class TransactionLog : Entity<long>, IHasCreationTime, IMayHaveTenant, ICreationAudited
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [Display(Name = "金额")]
        public Currency Currency { get; set; }

        /// <summary>
        /// 创建者UserId
        /// </summary>
        [Display(Name = "创建者UserId")]
        public long? CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Display(Name = "租户Id")]
        public int? TenantId { get; set; }

        /// <summary>
        /// 客户端Ip
        /// </summary>
        [MaxLength(64)]
        [Display(Name = "客户端Ip")]
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        [Display(Name = "客户端名称")]
        [MaxLength(128)]
        public string ClientName { get; set; }

        /// <summary>
        /// 是否冻结
        /// </summary>
        [Display(Name = "是否冻结")]
        public bool IsFreeze { get; set; }

        /// <summary>
        /// 支付渠道
        /// </summary>
        [Display(Name = "支付渠道")]
        public PayChannels PayChannel { get; set; }

        /// <summary>
        /// 终端
        /// </summary>
        [Display(Name = "终端")]
        public Terminals Terminal { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        [Display(Name = "交易状态")]
        public TransactionStates TransactionState { get; set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "自定义数据")]
        public string CustomData { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "交易单号")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 支付订单号（比如微信支付订单号）
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "支付订单号")]
        public string TransactionId { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        [Display(Name = "支付完成时间")]
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [MaxLength(2000)]
        [Display(Name = "异常信息")]
        public string Exception { get; set; }

        

        /// <summary>
        /// 创建交易日志
        /// </summary>
        /// <param name="transactionLog"></param>
        /// <returns></returns>

        public static TransactionLog CreateTransactionLog(TransactionLog transactionLog)
        {
            if (transactionLog == null)
                transactionLog = new TransactionLog();
            transactionLog.ClientIpAddress = transactionLog.ClientIpAddress.TruncateWithPostfix(64);
            transactionLog.Exception = transactionLog.Exception.TruncateWithPostfix(2000);
            transactionLog.ClientName = transactionLog.ClientName.TruncateWithPostfix(128);
            transactionLog.CustomData = transactionLog.CustomData.TruncateWithPostfix(500);
            transactionLog.Name = transactionLog.Name.TruncateWithPostfix(50);
            return transactionLog;
        }
    }
}
